using Catalogo.Service.Api.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace Catalogo.Service.Api;

/// <summary>
/// Camada de Negócio - Produto.
/// </summary>
public class ProductBLL
{
    private readonly GrupoSimilarBLL _grupoSimilarBll = new();
    #region Detalhe Produto

    /// <summary>
    /// Obtem o detalhe de produtos.
    /// </summary>
    /// <param name="filter">Filtro.</param>
    /// <param name="codOpdTrcEtn">Tipo da operação.</param>
    /// <param name="codCnl">Código do canal. (plataforma)</param>
    /// <returns>Detalhe do produto.</returns>
    /// Leon Denis @ByteOn
    /// Douglas Antunes @ByteOn - Incluindo variação de plataforma
    public async Task<ProductDetail> GetProductDetail(string filter, int codOpdTrcEtn, ushort codCnl)
    {
        if (string.IsNullOrEmpty(filter)) return null;

        string requestedSku = filter?.Trim();
        bool forceLoadImages = false;

        ProductDAL dal = new();
        ProductDetail productDetail;

        switch (codCnl)
        {
            case (int)PlataformasCatalogoCorp.B2B:
                {
                    ManagedFilter managedFilter = Util.GetManagedFilter(filter);

                    if (managedFilter.IsMartins)
                    {
                        productDetail = await dal.GetProductDetailB2B1P(managedFilter.CodMerLng);
                        if (productDetail is null) break;

                        productDetail.Group = new(productDetail);
                        //productDetail.Hierarchy = await dal.GetHierarchyProductB2B1P(Convert.ToInt64(productDetail.CODMER));
                        break;
                    }

                    productDetail = await dal.GetProductDetailB2B3P(managedFilter.CodMer, managedFilter.Seller);
                    if (productDetail is null) break;

                    //productDetail.Hierarchy = await dal.GetHierarchyProductB2B3P(managedFilter.CodMer, managedFilter.Seller);
                    // Não é necessário pesquisar duas vezes caso código operação tipo 9 ou 10.
                    codOpdTrcEtn = (int)TipoOperacaoDetalheProduto.Nenhum;
                    forceLoadImages = true;
                    break;
                }
            case (int)PlataformasCatalogoCorp.EFacil:
                {
                    EfacilSkuParser skuParsed = EfacilSkuParser.Parse(filter);
                    if (skuParsed is null) throw new Exception("Falha no parse do Sku e-Fácil");
                    switch (skuParsed.Origem)
                    {
                        case EfacilProductOrigin.Seller3P:
                            productDetail = await dal.GetProductDetailEfacil3PAsync(skuParsed.CODCLI, skuParsed.ErpCode);
                            break;
                        case EfacilProductOrigin.Kit1P:
                        case EfacilProductOrigin.Fracionado1P:
                            productDetail = await dal.GetProductDetailEfacil1P(skuParsed.CODCLI, skuParsed.CodMer!.Value);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(skuParsed.Origem), "Origem de produto e-Fácil não mapeado");
                    }

                    //// Grupo Similar e-Fácil
                    //if (productDetail is not null)
                    //{
                    //    int codTipPrd = skuParsed.Origem switch
                    //    {
                    //        EfacilProductOrigin.Seller3P => 3,
                    //        EfacilProductOrigin.Kit1P => 4,
                    //        EfacilProductOrigin.Fracionado1P => 4,
                    //        _ => throw new NotImplementedException(
                    //            $"a tradução de CODTIPPRD para a origem {skuParsed.Origem} não foi implementada")
                    //    };
                    //    List<ProductGrupoSimilar> dadosGrupoSimilarList = await dal.GetProductGrupoSimilarListAsync(
                    //        productDetail.CODPRD, codTipPrd, skuParsed.CODCLI);
                    //    if (dadosGrupoSimilarList.Count > 0)
                    //    {
                    //        ProductGrupoSimilar dadosFirst = dadosGrupoSimilarList.First();
                    //        productDetail.GrupoSimilar = new ProductDetailGrupoSimilar
                    //        {
                    //            TituloProdutoPai = dadosFirst.TituloProdutoPai,
                    //            CodigoAtributo = dadosFirst.CodigoAtributo,
                    //            DescricaoAtributo = dadosFirst.DescricaoAtributo,
                    //            Produtos = dadosGrupoSimilarList.Select(x => new ProductDetailGrupoSimilarProductEntry
                    //            {
                    //                CodMer = x.Sku, IsPrincipal = x.IsPrincipal
                    //            }).OrderByDescending(x => x.IsPrincipal)
                    //                .ThenBy(x => x.CodMer)
                    //                .ToList()
                    //        };

                    //        productDetail.TipoProdutoEfacil = dadosGrupoSimilarList
                    //            .FirstOrDefault(x => x.Sku == skuParsed.ValidSku)?.IsPrincipal ?? false ?
                    //                TipoProdutoEfacil.Pai : TipoProdutoEfacil.FilhoOuSimples;
                    //    }
                    //}

                    break;
                }
            default: throw new NotImplementedException($"O código da plataforma \"{codCnl}\" não foi implementado");
        }

        if (productDetail == null) return null;

        productDetail.PSKU = "p" + requestedSku ?? productDetail.PSKU ?? productDetail.CODMER;

        if (codCnl == (int)PlataformasCatalogoCorp.B2B)
            productDetail = await _grupoSimilarBll.ApplyGrupoSimilarAsync(dal, productDetail, requestedSku, codCnl);

        // Atribuindo imagens ao produto.
        int[] tiposOperacoesImagem =
        {
            (int) TipoOperacaoDetalheProduto.App, (int) TipoOperacaoDetalheProduto.PortalCliente,
            (int) TipoOperacaoDetalheProduto.PortalVendas
        };
        if (tiposOperacoesImagem.Contains(codOpdTrcEtn) || forceLoadImages)
            await SetImagesOnProduct(dal, productDetail, codCnl);

        // Atributos.
        if (productDetail != null)
        {
            productDetail.Attributes = await dal.GetProductAttributes(productDetail.CODPRD, codCnl);
            RemoveDefinitionAttribute(productDetail);

            // Atributo extra de diretoria do e-Fácil
            if (codCnl == (int)PlataformasCatalogoCorp.EFacil && productDetail.CODDRT.GetValueOrDefault() != 0
                && !string.IsNullOrEmpty(productDetail.DESDRT))
            {
                productDetail.Attributes.Add(new ProductAttribute
                {
                    NAME = "Diretoria",
                    VALUE = productDetail.DESDRT,
                    INDATRDEF = -999,
                    CODATRPRDEXT = productDetail.CODDRT!.Value,
                    CODATRPRD = -1 // Não existe este atributo cadastrado no Catálogo
                });
            }
        }

        return productDetail;
    }    
    
    /// <summary>
    /// Atribui as imagens no produto.
    /// </summary>
    /// <param name="dal">Camada de acesso do produto.</param>
    /// <param name="productDetail">Detalhes do produto.</param>
    /// <param name="codCnl">Código do canal. (plataforma)</param>
    /// Leon Denis @ByteOn
    /// Douglas Antunes @ByteOn - Incluindo variação de plataforma
    private async Task SetImagesOnProduct(ProductDAL dal, ProductDetail productDetail, ushort codCnl)
    {
        if (productDetail == null) return;

        switch (codCnl)
        {
            case (int)PlataformasCatalogoCorp.B2B:
                {
                    if (productDetail.CODSLR == 0)
                        productDetail.ProductImages = await dal.GetProductImageB2B1P(productDetail.CODPRD);
                    else
                        productDetail.ProductImages = await dal.GetProductImageB2B3P(productDetail.CODPRD);
                    break;
                }
            case (int)PlataformasCatalogoCorp.EFacil:
                productDetail.ProductImages = await dal.GetProductImageB2B3P(productDetail.CODPRD);
                break;
            default: throw new NotImplementedException($"O código da plataforma \"{codCnl}\" não foi implementado");
        }

        if (productDetail.ProductImages.Count > 0)
        {
            ProductImage hero = productDetail.ProductImages.FirstOrDefault(img => img.IsHero);
            // Enquanto a YDI013 não roda full manter a reordenação hard-code.
            if (hero != null)
            {
                productDetail.IMGLNK = hero.IMGLNK;

                productDetail.ProductImages.Remove(hero);
                productDetail.ProductImages.Insert(0, hero);
            }
            else
                productDetail.IMGLNK = productDetail.ProductImages.FirstOrDefault(img => img.CODTIPIMG == 1)?.IMGLNK ?? string.Empty;
             
          
            ushort index = 0;
            productDetail.ProductImages.ForEach(img =>
            {
                var (domain, uri, version) = ExtrairDadosImagem(img.IMGLNK);
                img.DOMAIN = domain;
                img.URI = uri;
                img.VERSION = version;
                img.SKU_3P = productDetail.CODMER;
                img.Index = index++;
            });
        }
    }


    private static void RemoveDefinitionAttribute(ProductDetail productDetail)
    {
        long? definitionAttribute = productDetail?.GrupoSimilarDetalhe?.CodigoAtributo;
        if (!definitionAttribute.HasValue || productDetail?.Attributes == null) return;

        productDetail.Attributes.RemoveAll(attribute => attribute.CODATRPRD == definitionAttribute.Value);
    }


    /// <summary>
    /// Obtêm dados de mercadorias 1P e 3P DIVINO
    /// </summary>
    /// <returns></returns>
    public async Task<RetornoMercadoriasSeller> GetMercadoria1P_3P(int numInicioItem, int numFimItem)
    {

        ProductDAL dal = new();
        RetornoMercadoriasSeller retornoMercadoriasSeller = new RetornoMercadoriasSeller();



        retornoMercadoriasSeller.lstMercadoriaSeller = await dal.GetMercadoria1P_3P(numInicioItem, numFimItem);


        return retornoMercadoriasSeller;
    }
    /// <summary>
    /// Obtêm dados de mercadorias E FILIAIS 1P e 3P DIVINO
    /// </summary>
    /// <param name="codigoMercadoria"></param>
    /// <returns></returns>
    public async Task<RetornoMercadoriasFiliais> GetMercadoriaFiliais1P_3P(string codigoMercadoria)
    {

        ProductDAL dal = new();
        RetornoMercadoriasFiliais retornoMercadoriasSeller = new RetornoMercadoriasFiliais();

        if (codigoMercadoria.ToString().ToUpper().Contains("MARTINS_"))
        {
            long codigoMercadoriaMartins = Convert.ToInt64(codigoMercadoria.ToString().Trim().Split('_')[1]);
            retornoMercadoriasSeller.lstMercadoriaFiliaisSeller = await dal.GetMercadoriaFiliais1P(codigoMercadoriaMartins);
        }
        else
        {
            retornoMercadoriasSeller.lstMercadoriaFiliaisSeller = await dal.GetMercadoriaFiliais3P(codigoMercadoria.ToString().ToUpper().Trim());
        }
        return retornoMercadoriasSeller;
    }
    #endregion Detalhe Produto

    #region SEO

    /// <summary>
    /// Obtem as informações do SEO.
    /// </summary>
    /// <param name="filter">Filtro.</param>
    /// <param name="codCnl">Código do canal. (plataforma)</param>
    /// <returns>Informações do SEO.</returns>
    /// Leon Denis @ByteOn
    /// Douglas Antunes @ByteOn - Incluindo variação de plataforma
    public Task<InfoSEO> GetInfoSEO(string filter, ushort codCnl)
    {
        if (string.IsNullOrEmpty(filter)) return null;

        ProductDAL dal = new();
        Task<InfoSEO> infoSeo;

        switch (codCnl)
        {
            case (int)PlataformasCatalogoCorp.B2B:
                {
                    ManagedFilter managedFilter = Util.GetManagedFilter(filter);

                    if (managedFilter.IsMartins)
                        infoSeo = dal.GetInfoSEOB2B1P(managedFilter.CodMerLng);
                    else
                        infoSeo = dal.GetInfoSEOB2B3P(managedFilter.CodMer, managedFilter.Seller);
                    break;
                }
            case (int)PlataformasCatalogoCorp.EFacil:
                EfacilSkuParser skuParsed = EfacilSkuParser.Parse(filter);
                if (skuParsed is null) throw new Exception("Falha no parse do Sku e-Fácil");
                switch (skuParsed.Origem)
                {
                    case EfacilProductOrigin.Seller3P:
                        infoSeo = dal.GetInfoSEOEfacil3PAsync(skuParsed.CODCLI, skuParsed.ErpCode);
                        break;
                    case EfacilProductOrigin.Kit1P:
                    case EfacilProductOrigin.Fracionado1P:
                        infoSeo = dal.GetInfoSEOEfacil1PAsync(skuParsed.CODCLI, skuParsed.CodMer!.Value);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(skuParsed.Origem), "Origem de produto e-Fácil não mapeado");
                }
                break;
            default: throw new NotImplementedException($"O código da plataforma \"{codCnl}\" não foi implementado");
        }

        return infoSeo;
    }

    #endregion SEO

    #region Hierarquia

    /// <summary>
    /// Obtem a hierarquia do produto.
    /// </summary>
    /// <param name="filter">Filtro.</param>
    /// <param name="codCnl">Código do canal. (plataforma)</param>
    /// <returns>Hieraquia.</returns>
    public Task<Hierarchy> GetHierarchyProduct(string filter, ushort codCnl)
    {
        if (string.IsNullOrEmpty(filter)) return null;

        ProductDAL dal = new();
        Task<Hierarchy> hierarchy;

        switch (codCnl)
        {
            case (int)PlataformasCatalogoCorp.B2B:
                {
                    ManagedFilter managedFilter = Util.GetManagedFilter(filter);

                    if (managedFilter.IsMartins)
                        hierarchy = dal.GetHierarchyProductB2B1P(managedFilter.CodMerLng);
                    else
                        hierarchy = dal.GetHierarchyProductB2B3P(managedFilter.CodMer, managedFilter.Seller);
                    break;
                }
            case (int)PlataformasCatalogoCorp.EFacil:
                EfacilSkuParser skuParsed = EfacilSkuParser.Parse(filter);
                if (skuParsed is null) throw new Exception("Falha no parse do Sku e-Fácil");
                switch (skuParsed.Origem)
                {
                    case EfacilProductOrigin.Seller3P:
                        hierarchy = dal.GetHierarchyProductEfacil3PAsync(skuParsed.CODCLI, skuParsed.ErpCode);
                        break;
                    case EfacilProductOrigin.Kit1P:
                    case EfacilProductOrigin.Fracionado1P:
                        hierarchy = dal.GetHierarchyProductEfacil1PAsync(skuParsed.CODCLI, skuParsed.CodMer!.Value);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(skuParsed.Origem), "Origem de produto e-Fácil não mapeado");
                }
                break;
            default: throw new NotImplementedException($"O código da plataforma \"{codCnl}\" não foi implementado");
        }

        return hierarchy;
    }

    #endregion Hierarquia

    #region Imagem
    /// <summary>
    /// Retornar os itens separados.
    /// </summary>
    /// <param name="imgLnk">.</param>
    /// <returns>Imagens.</returns>
    private (string Domain, string Uri, string Version) ExtrairDadosImagem(string imgLnk)
    {
        if (string.IsNullOrWhiteSpace(imgLnk))
            return ("", "", "");

        try
        {
            var uri = new Uri(imgLnk);
            var query = System.Web.HttpUtility.ParseQueryString(uri.Query);

            return (
                $"{uri.Scheme}://{uri.Host}",
                uri.AbsolutePath,
                query["v"] ?? ""
            );
        }
        catch
        {
            return ("", "", "");
        }
    }

    /// <summary>
    /// Obtem uma lista de imagens de acordo com os filtros.
    /// </summary>
    /// <param name="filters">Filtros.</param>
    /// <param name="codCnl">Código do canal. (plataforma)</param>
    /// <returns>Imagens.</returns>
    public async Task<List<ProductImage>> GetImageList(List<string> filters, ushort codCnl)
    {
        if (filters == null || filters.Count == 0)
            throw new Exception("Favor informar no mínimo um SKU para filtrar.");

        if (filters!.Count > 50)
            throw new Exception("Favor informar no máximo 50 SKU's.");

        List<ProductImage> images = new();
        ProductDAL dal = new();

        ImmutableList<string> filtersDistinct = filters.Distinct().ToImmutableList();
        foreach (string filter in filtersDistinct)
        {
            ProductImage image;

            switch (codCnl)
            {
                case (int)PlataformasCatalogoCorp.B2B:
                    {
                        ManagedFilter managedFilter = Util.GetManagedFilter(filter);
                        if (managedFilter.IsMartins)
                            image = await dal.GetProductImageB2B1PUnique(managedFilter.CodMerLng);
                        else
                            image = await dal.GetProductImageB2B3PUnique(managedFilter.CodMer, managedFilter.Seller);
                        break;
                    }
                case (int)PlataformasCatalogoCorp.EFacil:
                    EfacilSkuParser skuParsed = EfacilSkuParser.Parse(filter);
                    if (skuParsed is null) throw new Exception("Falha no parse do Sku e-Fácil");
                    switch (skuParsed.Origem)
                    {
                        case EfacilProductOrigin.Seller3P:
                            image = await dal.GetProductImageEfacil3PUniqueAsync(skuParsed.CODCLI, skuParsed.ErpCode);
                            break;
                        case EfacilProductOrigin.Kit1P:
                        case EfacilProductOrigin.Fracionado1P:
                            image = await dal.GetProductImageEfacil1PUniqueAsync(skuParsed.CODCLI, skuParsed.CodMer!.Value);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(skuParsed.Origem), "Origem de produto e-Fácil não mapeado");
                    }
                    break;
                default: throw new NotImplementedException($"O código da plataforma \"{codCnl}\" não foi implementado");
            }

            if (image != null) images.Add(image);
        }

        return images;
    }

    #endregion Imagem

    #region Protocolo

    /// <summary>
    /// Salva o protocolo do produto.
    /// </summary>
    /// <param name="protocolo">Protocolo do produto.</param>
    /// <returns>Quantidade de linhas afetadas.</returns>
    public async Task<int> InsertProtocolo(Protocolo protocolo)
    {
        return await new ProductDAL().InsertProtocolo(protocolo);
    }

    #endregion
}
