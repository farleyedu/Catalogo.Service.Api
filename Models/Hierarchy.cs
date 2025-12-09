using Newtonsoft.Json;

namespace Catalogo.Service.Api;

/// <summary>
/// Hierarquia do produto.
/// </summary>
public class Hierarchy
{
    /// <summary>
    /// CODIGO CATEGORIA.
    /// </summary>
    [JsonProperty("codDptBtb")]
    public long CODCTG { get; set; }

    /// <summary>
    /// DESCRICAO CATEGORIA.
    /// </summary>
    [JsonProperty("titulo")]
    public string DESCTG { get; set; }

    /// <summary>
    /// DESCRICAO CATEGORIA. (Tratada)
    /// </summary>
    [JsonProperty("termo")]
    public string DESCTGTRT
    {
        get
        {
            return Util.NormalizeStringForUrl(this.DESCTG);
        }
    }

    /// <summary>
    /// CODIGO DA SUBCATEGORIA DE PRODUTOS.
    /// </summary>
    [JsonIgnore]
    public long CODSUBCTGPRD { get; set; }

    /// <summary>
    /// DESCRICAO DA SUBCATEGORIA DE PRODUTOS.
    /// </summary>
    [JsonIgnore]
    public string DESSUBCTGPRD { get; set; }

    /// <summary>
    /// Submenus da categoria.
    /// </summary>
    [JsonProperty("subCategoria")]
    public HierarchySubcategory SubHierarchy
    {
        get
        {
            return new HierarchySubcategory()
            {
                CODCTG = this.CODCTG,
                CODSUBCTGPRD = this.CODSUBCTGPRD,
                DESSUBCTGPRD = this.DESSUBCTGPRD
            };
        }
    }
}

/// <summary>
/// Subcategoria para Hierarquia do produto.
/// </summary>
public class HierarchySubcategory
{
    /// <summary>
    /// CODIGO CATEGORIA.
    /// </summary>
    [JsonProperty("codDptBtb")]
    public long CODCTG { get; set; }

    /// <summary>
    /// CODIGO DA SUBCATEGORIA DE PRODUTOS.
    /// </summary>
    [JsonProperty("codSubDptBtb")]
    public long CODSUBCTGPRD { get; set; }

    /// <summary>
    /// DESCRICAO DA SUBCATEGORIA DE PRODUTOS.
    /// </summary>
    [JsonProperty("nome")]
    public string DESSUBCTGPRD { get; set; }

    /// <summary>
    /// DESCRICAO DA SUBCATEGORIA DE PRODUTOS. (Tratada)
    /// </summary>
    [JsonProperty("termo")]
    public string DESSUBCTGPRDTRT
    {
        get
        {
            return Util.NormalizeStringForUrl(this.DESSUBCTGPRD);
        }
    }

}

/// <summary>
/// Pesquisa cacheada da Hierarquia produto.
/// </summary>
public class HierarchySearch
{
    /// <summary>
    /// Construtor.
    /// </summary>
    public HierarchySearch() { }

    /// <summary>
    /// Construtor.
    /// </summary>
    /// <param name="filter">Fitlro.</param>
    /// <param name="hierarchy">Hierarquia do produto.</param>
    public HierarchySearch(string filter, Hierarchy hierarchy)
    {
        Filter = filter;
        Hierarchy = hierarchy;
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
    /// Hierarquia.
    /// </summary>
    public Hierarchy Hierarchy { get; set; }
}
