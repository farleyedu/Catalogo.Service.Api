using Newtonsoft.Json;

namespace Catalogo.Service.Api;

/// <summary>
/// Informações do SEO.
/// </summary>
public class InfoSEO
{
    /// <summary>
    /// DESCRICAO TITULO PRODUTO.
    /// </summary>
    [JsonProperty("tituloProduto")]
    public string DESTITPRD { get; set; }

    /// <summary>
    /// DESCRICAO URL LINK.
    /// </summary>
    [JsonProperty("url")]
    public string DESURLLNK { get; set; }

    /// <summary>
    /// DESCRICAO METADADO PRODUTO.
    /// </summary>
    [JsonProperty("metadescription")]
    public string DESMDDPRD { get; set; }

    /// <summary>
    /// DESCRICAO DE TERMOS CHAVE DO COMERCIO ELETRONICO - KEYWORDS.
    /// </summary>
    [JsonProperty("keyword")]
    public string DESTERCHV { get; set; }

    /// <summary>
    /// DESCRICAO TITULO IMAGEM PRODUTO.
    /// </summary>
    [JsonProperty("textoImagem")]
    public string DESTITIMGPRD { get; set; }

}

/// <summary>
/// Pesquisa cacheada informação SEO.
/// </summary>
public class InfoSEOSearch
{
    /// <summary>
    /// Construtor.
    /// </summary>
    public InfoSEOSearch() { }

    /// <summary>
    /// Construtor.
    /// </summary>
    /// <param name="filter">Filtro.</param>
    /// <param name="infoSEO">Informação do SEO.</param>
    public InfoSEOSearch(string filter, InfoSEO infoSEO)
    {
        Filter = filter;
        InfoSEO = infoSEO;
    }

    /// <summary>
    /// Filtro.
    /// </summary>
    public string Filter { get; set; }

    /// <summary>
    /// Código da plataforma
    /// </summary>
    public ushort CodPlt { get; set; }

    /// <summary>
    /// Informação do SEO.
    /// </summary>
    public InfoSEO InfoSEO { get; set; }
}
