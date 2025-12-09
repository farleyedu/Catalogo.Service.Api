using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalogo.Service.Api;

/// <summary>
/// Regras de negócio para Grupo Similar.
/// </summary>
public class GrupoSimilarBLL
{
    private readonly GrupoSimilarDAL _grupoSimilarDal = new();

    /// <summary>
    /// Aplica o contexto de grupo similar ao detalhe do produto.
    /// </summary>
    public async Task<ProductDetail> ApplyGrupoSimilarAsync(ProductDAL productDal, ProductDetail productDetail,
        string requestedSku, ushort codCnl)
    {
        if (productDetail is null) return null;

        productDetail.HasGrupoSimilar = false;
        productDetail.GrupoSimilarDetalhe = null;

        if (codCnl != (int)PlataformasCatalogoCorp.B2B) return productDetail;

        List<GrupoSimilarProdutoAtributo> rawGrupoSimilar =
            await _grupoSimilarDal.GetGrupoSimilarProdutoAtributoAsync(productDetail.CODPRD);

        if (rawGrupoSimilar == null || rawGrupoSimilar.Count == 0) return productDetail;

        GrupoSimilarProdutoAtributo firstRaw = rawGrupoSimilar.First();
        int codGrupo = (int)firstRaw.CODGRPSMRMER;
        string tituloGrupo = string.IsNullOrWhiteSpace(firstRaw.DESGRPSMRMER)
            ? firstRaw.DESPRDPAD
            : firstRaw.DESGRPSMRMER;

        List<GrupoSimilarProdutoInfo> produtosGrupo = await _grupoSimilarDal.GetGrupoProdutosAsync(codGrupo);
        if (produtosGrupo == null || produtosGrupo.Count == 0) return productDetail;

        (long codigoAtributo, string nomeAtributo, Dictionary<long, string> valoresAtributo) =
            await ObterAtributoDefinicaoAsync(produtosGrupo);

        List<ProductGrupoSimilar> grupoSimilarList = BuildGrupoSimilarEntries(
            produtosGrupo, codGrupo, tituloGrupo, codigoAtributo, nomeAtributo, valoresAtributo);
        if (grupoSimilarList.Count == 0) return productDetail;

        ProductDetailGrupoSimilar grupoSimilar = BuildGrupoSimilarPayload(grupoSimilarList);
        if (!string.IsNullOrWhiteSpace(grupoSimilar.TituloProdutoPai))
            productDetail.DESMER = grupoSimilar.TituloProdutoPai;

        productDetail.HasGrupoSimilar = true;
        productDetail.GrupoSimilarDetalhe = grupoSimilar;

        bool isCurrentPrincipal = grupoSimilarList.Any(x =>
            NormalizeSku(x.Sku) == NormalizeSku(productDetail.CODMER) && x.IsPrincipal);

        if (isCurrentPrincipal)
            return productDetail;

        ProductGrupoSimilar principalEntry = grupoSimilarList.FirstOrDefault(x => x.IsPrincipal);
        if (principalEntry == null) return productDetail;

        ProductDetail parentDetail = await GetParentProductDetailAsync(productDal, principalEntry.Sku, codCnl);
        if (parentDetail == null) return productDetail;

        parentDetail.PSKU = requestedSku ?? productDetail.PSKU ?? productDetail.CODMER;
        parentDetail.HasGrupoSimilar = true;
        parentDetail.GrupoSimilarDetalhe = grupoSimilar;

        if (!string.IsNullOrWhiteSpace(grupoSimilar.TituloProdutoPai))
            parentDetail.DESMER = grupoSimilar.TituloProdutoPai;

        return parentDetail;
    }

    private static ProductDetailGrupoSimilar BuildGrupoSimilarPayload(List<ProductGrupoSimilar> grupoSimilarList)
    {
        ProductGrupoSimilar firstEntry = grupoSimilarList.First();

        return new ProductDetailGrupoSimilar
        {
            TituloProdutoPai = firstEntry.TituloProdutoPai,
            CodigoAtributo = firstEntry.CodigoAtributo,
            DescricaoAtributo = firstEntry.DescricaoAtributo,
            Produtos = grupoSimilarList.Select(x => new ProductDetailGrupoSimilarProductEntry
            {
                CodMer = x.Sku,
                IsPrincipal = x.IsPrincipal,
                ValorAtributo = x.ValorAtributo
            }).OrderByDescending(x => x.IsPrincipal)
                .ThenBy(x => x.CodMer)
                .ToList()
        };
    }

    private static List<ProductGrupoSimilar> BuildGrupoSimilarEntries(
        List<GrupoSimilarProdutoInfo> produtosGrupo,
        int codGrupo,
        string tituloGrupo,
        long codigoAtributo,
        string nomeAtributo,
        Dictionary<long, string> valoresAtributo)
    {
        if (produtosGrupo == null || produtosGrupo.Count == 0) return new List<ProductGrupoSimilar>();

        valoresAtributo ??= new Dictionary<long, string>();

        return produtosGrupo.Select(produto =>
        {
            string valor = valoresAtributo.TryGetValue(produto.CODPRD, out string valorEncontrado)
                ? valorEncontrado
                : null;

            return new ProductGrupoSimilar
            {
                CODGRPSMRMER = codGrupo,
                TituloProdutoPai = tituloGrupo,
                Sku = BuildB2BSku(produto),
                IsPrincipal = produto.INDPRDPCP == 1,
                CodigoAtributo = codigoAtributo,
                DescricaoAtributo = nomeAtributo,
                ValorAtributo = valor
            };
        }).ToList();
    }

    private static string BuildB2BSku(GrupoSimilarProdutoInfo entry)
    {
        if (entry.CODMERMRT.HasValue && entry.CODMERMRT > 0)
            return $"martins_{entry.CODMERMRT}";

        return entry.CODPRD.ToString();
    }

    private async Task<(long codigoAtributo, string nomeAtributo, Dictionary<long, string> valoresAtributo)>
        ObterAtributoDefinicaoAsync(List<GrupoSimilarProdutoInfo> produtosGrupo)
    {
        Dictionary<long, List<GrupoSimilarAtributoProduto>> atributosPorProduto = new();

        foreach (GrupoSimilarProdutoInfo produto in produtosGrupo)
            atributosPorProduto[produto.CODPRD] = await _grupoSimilarDal.GetAtributosProdutoAsync(produto.CODPRD);

        GrupoSimilarProdutoInfo produtoPai = produtosGrupo.FirstOrDefault(p => p.INDPRDPCP == 1) ?? produtosGrupo.First();
        if (!atributosPorProduto.TryGetValue(produtoPai.CODPRD, out List<GrupoSimilarAtributoProduto> atributosPai) ||
            atributosPai.Count == 0)
            return (0, null, new Dictionary<long, string>());

        foreach (GrupoSimilarAtributoProduto atrPai in atributosPai)
        {
            bool todosTem = true;
            bool valoresDiferem = false;

            foreach (GrupoSimilarProdutoInfo produto in produtosGrupo)
            {
                if (produto.CODPRD == produtoPai.CODPRD) continue;

                List<GrupoSimilarAtributoProduto> atributosProduto = atributosPorProduto[produto.CODPRD];
                GrupoSimilarAtributoProduto atributoProduto =
                    atributosProduto.FirstOrDefault(a => a.CODATRPRD == atrPai.CODATRPRD);

                if (atributoProduto == null)
                {
                    todosTem = false;
                    break;
                }

                if (atributoProduto.CODOPCATR != atrPai.CODOPCATR)
                    valoresDiferem = true;
            }

            if (todosTem && valoresDiferem)
            {
                string nomeAtributo = await _grupoSimilarDal.GetNomeAtributoAsync(atrPai.CODATRPRD) ?? "Variacao";
                Dictionary<long, string> valores = new();

                foreach (GrupoSimilarProdutoInfo produto in produtosGrupo)
                {
                    GrupoSimilarAtributoProduto atributoProduto =
                        atributosPorProduto[produto.CODPRD].FirstOrDefault(a => a.CODATRPRD == atrPai.CODATRPRD);

                    if (atributoProduto == null || !atributoProduto.CODOPCATR.HasValue)
                        continue;

                    string valor = await _grupoSimilarDal.GetDescricaoOpcaoAtributoAsync(
                        atributoProduto.CODATRPRD, atributoProduto.CODOPCATR.Value);

                    valores[produto.CODPRD] = valor ?? string.Empty;
                }

                return (atrPai.CODATRPRD, nomeAtributo, valores);
            }
        }

        return (0, null, new Dictionary<long, string>());
    }

    private async Task<ProductDetail> GetParentProductDetailAsync(ProductDAL dal, string sku, ushort codCnl)
    {
        if (string.IsNullOrWhiteSpace(sku)) return null;

        if (codCnl == (int)PlataformasCatalogoCorp.B2B)
        {
            ManagedFilter managedFilter = Util.GetManagedFilter(sku);
            if (managedFilter.IsMartins)
            {
                ProductDetail parent1P = await dal.GetProductDetailB2B1P(managedFilter.CodMerLng);
                if (parent1P != null)
                    parent1P.Group ??= new Group(parent1P);
                return parent1P;
            }

            return await dal.GetProductDetailB2B3P(managedFilter.CodMer, managedFilter.Seller);
        }

        if (codCnl == (int)PlataformasCatalogoCorp.EFacil)
        {
            EfacilSkuParser parentSkuParsed = EfacilSkuParser.Parse(sku.Trim());
            if (parentSkuParsed is null) return null;

            ProductDetail efacilParent = parentSkuParsed.Origem switch
            {
                EfacilProductOrigin.Seller3P => await dal.GetProductDetailEfacil3PAsync(parentSkuParsed.CODCLI,
                    parentSkuParsed.ErpCode),
                EfacilProductOrigin.Kit1P or EfacilProductOrigin.Fracionado1P when parentSkuParsed.CodMer.HasValue =>
                    await dal.GetProductDetailEfacil1P(parentSkuParsed.CODCLI, parentSkuParsed.CodMer.Value),
                _ => null
            };

            return efacilParent;
        }

        return null;
    }

    private static string NormalizeSku(string sku) =>
        string.IsNullOrWhiteSpace(sku) ? string.Empty : sku.Trim().ToLowerInvariant();
}
