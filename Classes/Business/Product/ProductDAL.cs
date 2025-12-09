using Catalogo.Service.Api.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Catalogo.Service.Api;

/// <summary>
/// Camada de Acesso - Produto.
/// </summary>
public class ProductDAL : OraAccess
{
    /// <summary>
    /// Obtem o detalhe do produto martins.
    /// </summary>
    /// <param name="codMer">Código da Mercadoria.</param>
    /// <returns>Detalhe do produto.</returns>
    public async Task<ProductDetail> GetProductDetailB2B1P(long codMer)
    {
            return (await Command.UseQuery(ProductSQL.GetProductDetailB2B1P())
                .AddParameter("CODMER", codMer)
                .ExecuteList<ProductDetail>()).FirstOrDefault();
    }

    /// <summary>
    /// Obtem o detalhe do produto marketplace.
    /// </summary>
    /// <param name="codMerSrr">Código da Mercadoria Seller.</param>
    /// <param name="nomSrr">Nome do Seller. (Vendedor)</param>
    /// <returns>Detalhe do produto.</returns>
    public async Task<ProductDetail> GetProductDetailB2B3P(string codMerSrr, string nomSrr)
    {
            return (await Command.UseQuery(ProductSQL.GetProductDetailB2B3P())
            .AddParameter("DESABVNOMSRR", nomSrr)
            .AddParameter("CODMERSRR", codMerSrr)
            .ExecuteList<ProductDetail>()).FirstOrDefault();        
    }

    /// <summary>
    /// Obtém o detalhe do produto e-Fácil 3P.
    /// </summary>
    /// <param name="codCli">Código do cliente.</param>
    /// <param name="codErpLja">Código Erp do produto</param>
    /// <returns>Detalhe do produto.</returns>
    public async Task<ProductDetail> GetProductDetailEfacil3PAsync(long codCli, string codErpLja)
    {
        return (await Command.UseQuery(ProductSQL.GetProductDetailEfacil3P())
            .AddParameter("CODCLI", codCli)
            .AddParameter("CODERPLJA", $"{codErpLja}%")
            .ExecuteList<ProductDetail>()).FirstOrDefault();
    }

    /// <summary>
    /// Obtém o detalhe do produto e-Fácil 1P.
    /// </summary>
    /// <param name="codCli">Código do cliente.</param>
    /// <param name="codMer">Código da Mercadoria</param>
    /// <returns>Detalhe do produto.</returns>
    public async Task<ProductDetail> GetProductDetailEfacil1P(long codCli, decimal codMer)
    {
        return (await Command.UseQuery(ProductSQL.GetProductDetailEfacil1P())
            .AddParameter("CODCLI", codCli)
            .AddParameter("CODPRDLJA", codMer.ToString(CultureInfo.InvariantCulture).PadRight(18))
            .ExecuteList<ProductDetail>()).FirstOrDefault();
    }

    /// <summary>
    /// Obtem as imagens do produto martins.
    /// </summary>
    /// <param name="codPrd">Código SKU do produto.</param>
    /// <returns>Imagens do produto.</returns>
    public async Task<List<ProductImage>> GetProductImageB2B1P(long codPrd)
    {
        return await Command.UseQuery(ProductSQL.GetProductImageB2B1P())
            .AddParameter("CODPRD", codPrd)
            .ExecuteList<ProductImage>();
    }

    /// <summary>
    /// Obtem as imagens do produto marketplace.
    /// </summary>
    /// <param name="codPrd">Código SKU do produto.</param>
    /// <returns>Imagens do produto.</returns>
    public async Task<List<ProductImage>> GetProductImageB2B3P(long codPrd)
    {
        return await Command.UseQuery(ProductSQL.GetProductImageB2B3P())
            .AddParameter("CODPRD", codPrd)
            .ExecuteList<ProductImage>();
    }

    /// <summary>
    /// Obtem os atributos do produto.
    /// </summary>
    /// <param name="codPrd">Código SKU do produto.</param>
    /// <param name="codCnl">Código da Plataforma</param>
    /// <returns>Atributos do produto.</returns>
    public async Task<List<ProductAttribute>> GetProductAttributes(long codPrd, int codCnl)
    {
        return await Command.UseQuery(ProductSQL.GetProductAttributes())
            .AddParameter("CODPRD", Convert.ToDecimal(codPrd))
            .AddParameter("CODCNL", codCnl)
            .ExecuteList<ProductAttribute>();
    }

    /// <summary>
    /// Obtem as informações do SEO Martins.
    /// </summary>
    /// <param name="codMer">Código da Mercadoria.</param>
    /// <returns>Informações do SEO.</returns>
    public async Task<InfoSEO> GetInfoSEOB2B1P(long codMer)
    {
        return (await Command.UseQuery(ProductSQL.GetInfoSEOB2B1P())
            .AddParameter("CODMER", codMer)
            .ExecuteList<InfoSEO>()).FirstOrDefault();
    }

    /// <summary>
    /// Obtem as informações do SEO Seller.
    /// </summary>
    /// <param name="codMerSrr">Código da Mercadoria Seller.</param>
    /// <param name="nomSrr">Nome do Seller. (Vendedor)</param>
    /// <returns>Informações do SEO.</returns>
    public async Task<InfoSEO> GetInfoSEOB2B3P(string codMerSrr, string nomSrr)
    {
        return (await Command.UseQuery(ProductSQL.GetInfoSEOB2B3P())
            .AddParameter("DESABVNOMSRR", nomSrr)
            .AddParameter("CODMERSRR", codMerSrr)
            .ExecuteList<InfoSEO>()).FirstOrDefault();
    }

    /// <summary>
    /// Obtem as informações do SEO e-Fácil 3P.
    /// </summary>
    /// <param name="codCli">Código do cliente.</param>
    /// <param name="codErpLja">Código Erp do produto</param>
    /// <returns>Informações do SEO.</returns>
    public async Task<InfoSEO> GetInfoSEOEfacil3PAsync(long codCli, string codErpLja)
    {
        return (await Command.UseQuery(ProductSQL.GetInfoSEOEfacil3P())
            .AddParameter("CODCLI", codCli)
            .AddParameter("CODERPLJA", codErpLja.PadRight(50))
            .ExecuteList<InfoSEO>()).FirstOrDefault();
    }

    /// <summary>
    /// Obtem as informações do SEO e-Fácil 1P.
    /// </summary>
    /// <param name="codCli">Código do cliente.</param>
    /// <param name="codMer">Código da Mercadoria</param>
    /// <returns>Informações do SEO.</returns>
    public async Task<InfoSEO> GetInfoSEOEfacil1PAsync(long codCli, decimal codMer)
    {
        return (await Command.UseQuery(ProductSQL.GetInfoSEOEfacil1P())
            .AddParameter("CODCLI", codCli)
            .AddParameter("CODPRDLJA", codMer.ToString(CultureInfo.InvariantCulture).PadRight(18))
            .ExecuteList<InfoSEO>()).FirstOrDefault();
    }

    /// <summary>
    /// Obtem hierarquia do produto Martins.
    /// </summary>
    /// <param name="codMer">Código da Mercadoria.</param>
    /// <returns>Hierarquia.</returns>
    public async Task<Hierarchy> GetHierarchyProductB2B1P(long codMer)
    {
        return (await Command.UseQuery(ProductSQL.GetHierarchyProductB2B1P())
            .AddParameter("CODMER", codMer)
            .ExecuteList<Hierarchy>()).FirstOrDefault();
    }

    /// <summary>
    /// Obtem hierarquia do produto Seller.
    /// </summary>
    /// <param name="codMerSrr">Código da Mercadoria Seller.</param>
    /// <param name="nomSrr">Nome do Seller. (Vendedor)</param>
    /// <returns>Hierarquia.</returns>
    public async Task<Hierarchy> GetHierarchyProductB2B3P(string codMerSrr, string nomSrr)
    {
        return (await Command.UseQuery(ProductSQL.GetHierarchyProductB2B3P())
            .AddParameter("DESABVNOMSRR", nomSrr)
            .AddParameter("CODMERSRR", codMerSrr)
            .ExecuteList<Hierarchy>()).FirstOrDefault();
    }

    /// <summary>
    /// Obtem hierarquia do produto e-Fácil 3P.
    /// </summary>
    /// <param name="codCli">Código do cliente.</param>
    /// <param name="codErpLja">Código Erp do produto</param>
    /// <returns>Hierarquia.</returns>
    public async Task<Hierarchy> GetHierarchyProductEfacil3PAsync(long codCli, string codErpLja)
    {
        return (await Command.UseQuery(ProductSQL.GetHierarchyProductEfacil3P())
            .AddParameter("CODCLI", codCli)
            .AddParameter("CODERPLJA", codErpLja.PadRight(50))
            .ExecuteList<Hierarchy>()).FirstOrDefault();
    }

    /// <summary>
    /// Obtem hierarquia do produto e-Fácil 1P.
    /// </summary>
    /// <param name="codCli">Código do cliente.</param>
    /// <param name="codMer">Código da Mercadoria</param>
    /// <returns>Hierarquia.</returns>
    public async Task<Hierarchy> GetHierarchyProductEfacil1PAsync(long codCli, decimal codMer)
    {
        return (await Command.UseQuery(ProductSQL.GetHierarchyProductEfacil1P())
            .AddParameter("CODCLI", codCli)
            .AddParameter("CODPRDLJA", codMer.ToString(CultureInfo.InvariantCulture).PadRight(18))
            .ExecuteList<Hierarchy>()).FirstOrDefault();
    }

    /// <summary>
    /// Obtem a imagem única do produto martins.
    /// </summary>
    /// <param name="codMer">Código da Mercadoria.</param>
    /// <returns>Imagem principal do produto.</returns>
    public async Task<ProductImage> GetProductImageB2B1PUnique(long codMer)
    {
        return (await Command.UseQuery(ProductSQL.GetProductImageB2B1PUnique())
            .AddParameter("CODMER", codMer)
            .ExecuteList<ProductImage>()).FirstOrDefault();
    }

    /// <summary>
    /// Obtem a imagem única do produto marketplace.
    /// </summary>
    /// <param name="codMerSrr">Código da Mercadoria Seller.</param>
    /// <param name="nomSrr">Nome do Seller. (Vendedor)</param>
    /// <returns>Imagem principal do produto.</returns>
    public async Task<ProductImage> GetProductImageB2B3PUnique(string codMerSrr, string nomSrr)
    {
        return (await Command.UseQuery(ProductSQL.GetProductImageB2B3PUnique())
            .AddParameter("DESABVNOMSRR", nomSrr)
            .AddParameter("CODMERSRR", codMerSrr)
            .ExecuteList<ProductImage>()).FirstOrDefault();
    }

    /// <summary>
    /// Obtém a imagem única do produto e-Fácil 3P.
    /// </summary>
    /// <param name="codCli">Código do cliente.</param>
    /// <param name="codErpLja">Código Erp do produto</param>
    /// <returns>Imagem principal do produto.</returns>
    public async Task<ProductImage> GetProductImageEfacil3PUniqueAsync(long codCli, string codErpLja)
    {
        return (await Command.UseQuery(ProductSQL.GetProductImageEfacil3PUnique())
            .AddParameter("CODCLI", codCli)
            .AddParameter("CODERPLJA", codErpLja.PadRight(50))
            .ExecuteList<ProductImage>()).FirstOrDefault();
    }

    /// <summary>
    /// Obtém a imagem única do produto e-Fácil 1P.
    /// </summary>
    /// <param name="codCli">Código do cliente.</param>
    /// <param name="codMer">Código da Mercadoria</param>
    /// <returns>Imagem principal do produto.</returns>
    public async Task<ProductImage> GetProductImageEfacil1PUniqueAsync(long codCli, decimal codMer)
    {
        return (await Command.UseQuery(ProductSQL.GetProductImageEfacil1PUnique())
            .AddParameter("CODCLI", codCli)
            .AddParameter("CODPRDLJA", codMer.ToString(CultureInfo.InvariantCulture).PadRight(18))
            .ExecuteList<ProductImage>()).FirstOrDefault();
    }

    /// <summary>
    /// Obtêm dados de mercadorias 1P e 3P DIVINO
    /// </summary>
    /// <returns></returns>
    public async Task<List<MercadoriaSeller>> GetMercadoria1P_3P(int numInicioItem, int numFimItem)
    {
        return (await Command.UseQuery(ProductSQL.GetMercadoria1P_3P())
            .AddParameter("NUMFIMITE", numFimItem)
            .AddParameter("NUMINIITE", numInicioItem)
            .ExecuteList<MercadoriaSeller>());
    }

    /// <summary>
    /// Obtêm dados de mercadorias E FILIAIS 1P e 3P DIVINO
    /// </summary>
    /// <param name="codigoMercadoria"></param>
    /// <returns></returns>
    public async Task<List<MercadoriasFiliais>> GetMercadoriaFiliais3P(string codigoMercadoria)
    {
        return (await Command.UseQuery(ProductSQL.GetMercadoria_Filiais_3P())
               .AddParameter("CODMERSRR", codigoMercadoria)
            .ExecuteList<MercadoriasFiliais>());
    }
    /// <summary>
    /// Obtêm dados de mercadorias E FILIAIS 1P e 3P DIVINO
    /// </summary>
    /// <param name="codigoMercadoria"></param>
    /// <returns></returns>
    public async Task<List<MercadoriasFiliais>> GetMercadoriaFiliais1P( long codigoMercadoria)
    {
        return (await Command.UseQuery(ProductSQL.GetMercadoria_Filiais_1P())
            .AddParameter("CODMER", codigoMercadoria)
            .ExecuteList<MercadoriasFiliais>());
    }

    /// <summary>
    /// Salva o protocolo do produto.
    /// </summary>
    /// <param name="protocolo">Protocolo do produto.</param>
    /// <returns>Quantidade de linhas afetadas.</returns>
    public async Task<int> InsertProtocolo(Protocolo protocolo)
    {
        return await Command.UseQuery(ProductSQL.InsertProtocolo()).AddModel(protocolo).ExecNonQuery();
    }
}
