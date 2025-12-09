using Catalogo.Service.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Catalogo.Service.Api.Controllers;

/// <summary>
/// Controller de Produtos.
/// </summary>
/// <remarks>Leon Denis @ByteOn</remarks>
[ApiController, Route("api/Mercadoria"), Produces("application/json")]
public class ProductController : ControllerBase
{
    /// <summary>
    /// Obtem os detalhes do produto.
    /// </summary>
    /// <param name="codMer">Código da mercadoria.</param>
    /// <param name="codOpdTrcEtn">Tipo da operação.</param>
    /// <param name="codCnl">Código do canal. (plataforma)</param>
    /// <returns>Detalhe do produto.</returns>
    [HttpGet, Route("detalhe")]
    public async Task<IActionResult> GetProductDetail([FromQuery] string codMer, int codOpdTrcEtn = 0,
        [FromQuery(Name = "channelCode")] ushort codCnl = 3)
    {
        ProductDetailSearch search = await Util.PerformanceSearch(
            CacheKeys.ProductDetail,
            search => search.Find(srh =>
                srh.CodMer?.ToLower().Trim() == codMer?.ToLower().Trim() && srh.CodOpdTrcEtn == codOpdTrcEtn
                && srh.CodPlt == codCnl),
            async () => new ProductDetailSearch(
                codMer?.ToLower(), codOpdTrcEtn, codCnl,
                await new ProductBLL().GetProductDetail(codMer, codOpdTrcEtn, codCnl))
        );

        return Ok(search.ProductDetail);
    }

    /// <summary>
    /// Obtem as informações do SEO.
    /// </summary>
    /// <param name="filter">Código da mercadoria. (Código único ou Seller + Código)</param>
    /// <param name="codCnl">Código do canal. (plataforma)</param>
    /// <returns>Informações do SEO.</returns>
    [HttpGet, Route("SEO")]
    public async Task<IActionResult> GetInfoSEO([FromQuery(Name = "codMer")] string filter,
        [FromQuery(Name = "channelCode")] ushort codCnl = 3)
    {
        InfoSEOSearch search = await Util.PerformanceSearch(
            CacheKeys.InfoSEO,
            search => search.Find(srh =>
                srh.Filter?.ToLower().Trim() == filter?.ToLower().Trim() && srh.CodPlt == codCnl),
            async () => new InfoSEOSearch(filter?.ToLower(), await new ProductBLL().GetInfoSEO(filter, codCnl))
        );

        return Ok(search.InfoSEO);
    }

    /// <summary>
    /// Obtem a hierarquia do produto.
    /// </summary>
    /// <param name="filter">Código da mercadoria. (Código único ou Seller + Código)</param>
    /// <param name="codCnl">Código do canal. (plataforma)</param>
    /// <returns>Hieraquia.</returns>
    [HttpGet, Route("hierarquia")]
    public async Task<IActionResult> GetHierarchyProduct([FromQuery(Name = "codMer")] string filter,
        [FromQuery(Name = "channelCode")] ushort codCnl = 3)
    {
        HierarchySearch search = await Util.PerformanceSearch(
            CacheKeys.Hierarchy,
            search => search.Find(srh =>
                srh.Filter?.ToLower().Trim() == filter?.ToLower().Trim() && srh.CodPlt == codCnl),
            async () => new HierarchySearch(filter?.ToLower(), await new ProductBLL().GetHierarchyProduct(filter, codCnl))
        );

        return Ok(search.Hierarchy);
    }

    /// <summary>
    /// Obtem a lista de imagens de produtos.
    /// </summary>
    /// <param name="codSkus">Lista de códigos SKU de mercadorias.</param>
    /// <param name="codCnl">Código do canal. (plataforma)</param>
    /// <returns>Imagens.</returns>
    [HttpGet, Route("listaImagem")]
    public async Task<IActionResult> GetImageList([FromQuery] List<string> codSkus,
        [FromQuery(Name = "channelCode")] ushort codCnl = 3)
    {
        ImageListSearch search = await Util.PerformanceSearch(
            CacheKeys.ImageList,
            // FIXME: Caso utilização do cache, melhorar para cachear skus e não listas exatas de sku
            search => search.Find(srh => srh.CodSkus == codSkus && srh.CodPlt == codCnl),
            async () => new ImageListSearch(codSkus, await new ProductBLL().GetImageList(codSkus, codCnl))
        );

        return Ok(search.Images);
    }


    /// <summary>
    /// Obtêm dados de mercadorias 1P e 3P DIVINO
    /// </summary>
    /// <returns>Imagens.</returns>
    [HttpGet, Route("GetMercadoria1P_3P")]
    public async Task<IActionResult> GetMercadoria1P_3P(int numInicioItem, int numFimItem)
    {
        RetornoMercadoriasSeller search = new RetornoMercadoriasSeller();
        search = await new ProductBLL().GetMercadoria1P_3P(numInicioItem, numFimItem);

        return Ok(search);
    }

    /// <summary>
    /// Obtêm dados de mercadorias E FILIAIS 1P e 3P DIVINO
    /// </summary>
    /// <param name="codigoMercadoria"></param>
    /// <returns></returns>
    [HttpGet, Route("GetMercadoriaFiliais1P_3P")]
    public async Task<IActionResult> GetMercadoriaFiliais1P_3P(string codigoMercadoria)
    {
        RetornoMercadoriasFiliais search = new RetornoMercadoriasFiliais();
        search = await new ProductBLL().GetMercadoriaFiliais1P_3P(codigoMercadoria);
        return Ok(search);
    }

    /// <summary>
    /// Protocola o produto com os dados informados.
    /// </summary>
    /// <param name="protocolo">Dados do protocolo produto.</param>
    /// <returns>Resultado do processo.</returns>
    [HttpPost, Route("protocolar")]
    public async Task<IActionResult> Protocolar([FromBody] Protocolo protocolo)
    {
        await new ProductBLL().InsertProtocolo(protocolo);
        return Ok(new WebReturn { Message = "Protocolado com sucesso!" });
    }
}
