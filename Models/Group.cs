using Newtonsoft.Json;

namespace Catalogo.Service.Api;

/// <summary>
/// Grupo do produto martins.
/// </summary>
public class Group
{
    /// <summary>
    /// Grupo. (Martins)
    /// </summary>
    [JsonProperty("codigo_grupo_mrt")]
    public int? GRUPO { get; set; }

    /// <summary>
    /// Descrição Grupo. (Martins)
    /// </summary>
    [JsonProperty("descricao_grupo_mrt")]
    public string DESGRUPO { get; set; }

    /// <summary>
    /// Categoria.
    /// </summary>
    [JsonProperty("categoria_mrt")]
    public Category Category { get; set; }

    /// <summary>
    /// Grupo.
    /// </summary>
    public Group() { }

    /// <summary>
    /// Grupo.
    /// </summary>
    /// <param name="product">Detalhe do produto.</param>
    public Group(ProductDetail product)
    {
        GRUPO = product.GRUPO;
        DESGRUPO = product.DESGRPMER?.Trim();

        Category = new Category
        {
            CATEGORIA = product.CATEGORIA,
            DESCATEGORIA = product.DESFMLMER?.Trim(),
            Subcategory = new Subcategory
            {
                SUBCATEGORIA = product.SUBCATEGORIA,
                DESSUBCATEGORIA = product.DESCLSMER?.Trim()
            }
        };
    }
}

/// <summary>
/// Categoria do produto martins.
/// </summary>
public class Category
{
    /// <summary>
    /// Categoria. (Martins)
    /// </summary>
    [JsonProperty("codigo_categoria_mrt")]
    public int? CATEGORIA { get; set; }

    /// <summary>
    /// Descrição Categoria. (Martins)
    /// </summary>
    [JsonProperty("descricao_categoria_mrt")]
    public string DESCATEGORIA { get; set; }

    /// <summary>
    /// Subcategoria.
    /// </summary>
    [JsonProperty("subcategoria_mrt")]
    public Subcategory Subcategory { get; set; }
}

/// <summary>
/// Subcategoria.
/// </summary>
public class Subcategory
{
    /// <summary>
    /// Subcategoria.  (Martins)
    /// </summary>
    [JsonProperty("codigo_subcategoria_mrt")]
    public long? SUBCATEGORIA { get; set; }

    /// <summary>
    /// Descrição Subcategoria. (Martins)
    /// </summary>
    [JsonProperty("descricao_subcategoria_mrt")]
    public string DESSUBCATEGORIA { get; set; }
}
