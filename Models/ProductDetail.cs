using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Catalogo.Service.Api;

/// <summary>
/// Detalhe produto.
/// </summary>
public class ProductDetail
{
    /// <summary>
    /// CODIGO MERCADORIA.
    /// </summary>
    [JsonProperty("codMer")]
    public string CODMER { get; set; }

    /// <summary>
    /// DESCRIÇÃO DA FICHA TECNICA DO PRODUTO.
    /// </summary>
    [JsonProperty("ficha")]
    public string DESFCATCNMERCMCETN { get; set; } // DESFCATCNPRD

    /// <summary>
    /// DESCRICAO MERCADORIA.
    /// </summary>
    [JsonProperty("titulo")]
    public string DESMER { get; set; }

    /// <summary>
    /// CODIGO DE BARRA UNDIDADE VENDA CONSUMO.
    /// </summary>
    [JsonProperty("codigoBarra")]
    public string CODBRRUNDVNDCSM { get; set; }

    /// <summary>
    /// SEM DESCRIÇÃO.
    /// </summary>
    [JsonProperty("vendido_entregue")]
    public string NOMSRR { get; set; }

    /// <summary>
    /// CODIGO GRUPO DE MERCADORIAS NCM.
    /// </summary>
    [JsonProperty("codigoNcm")]
    public string CODGRPMERNCM { get; set; }

    /// <summary>
    /// DESCRICAO URL LINK.
    /// </summary>
    [JsonProperty("enderecoSEO")]
    public string DESURLLNK { get; set; }

    /// <summary>
    /// CODIGO ESTADO UNIAO UTILIZADO P/ FATURAMENTO SELLER B2B.
    /// </summary>
    [JsonProperty("ufFaturamento")]
    public string CODESTUNIFAT { get; set; }

    /// <summary>
    /// DESCRIÇÃO DE MARCA DO CATALOGO.
    /// </summary>
    [JsonProperty("marca")]
    public string DESMRCCTL { get; set; }

    #region Categoria (Hierarquia)
    
    /// <summary>
    /// Categoria.
    /// </summary>
    [JsonProperty("grupo_mrt")]
    public Group Group { get; set; }

    /// <summary>
    /// Grupo. (Martins)
    /// </summary>
    [JsonIgnore]
    public int? GRUPO { get; set; }

    /// <summary>
    /// DESCRICAO GRUPO MERCADORIA.
    /// </summary>
    [JsonIgnore]
    public string DESGRPMER { get; set; }

    /// <summary>
    /// Categoria. (Martins)
    /// </summary>
    [JsonIgnore]
    public int? CATEGORIA { get; set; }

    /// <summary>
    /// DESCRICAO FAMILIA MERCADORIA.
    /// </summary>
    [JsonIgnore]
    public string DESFMLMER { get; set; }

    /// <summary>
    /// Subcategoria.  (Martins)
    /// </summary>
    [JsonIgnore]
    public long? SUBCATEGORIA { get; set; }

    /// <summary>
    /// DESCRICAO CLASSE MERCADORIA.
    /// </summary>
    [JsonIgnore]
    public string DESCLSMER { get; set; }

    #endregion

    /// <summary>
    /// Código do fornecedor. (Martins)
    /// </summary>
    [JsonProperty("codigo_fornecedor_mrt")]
    public long? CODFRN { get; set; }

    /// <summary>
    /// Descrição Grupo. (Martins)
    /// </summary>
    [JsonIgnore]
    public string NOMFRN { get; set; }

    /// <summary>
    /// Descrição Grupo. (Martins)
    /// </summary>
    [JsonProperty("nome_fornecedor_mrt")]
    public string DESNOMFRN { get { return NOMFRN?.Trim(); } }

    /// <summary>
    /// Código da marca. (Martins)
    /// </summary>
    [JsonProperty("codigo_marca_mrt")]
    public long? CODMRCMER { get; set; }

    /// <summary>
    /// Descrição da marca. (Martins)
    /// </summary>
    [JsonProperty("descricao_marca_mrt")]
    public string DESMRCMER { get; set; }

    /// <summary>
    /// CÓDIGO DA SEÇÃO DO PRODUTO.
    /// </summary>
    [JsonProperty("codigo_secao")]
    public string CODSECAO { get; set; }

    /// <summary>
    /// DESCRIÇÃO DA SEÇÃO DO PRODUTO.
    /// </summary>
    [JsonProperty("secao")]
    public string SECAO { get; set; }

    /// <summary>
    /// SIGLA DA REGIÃO DE FATURAMENTO.
    /// </summary>
    [JsonProperty("sigla_regiao_faturamento")]
    public string SIGLA_REGIAO_FATURAMENTO { get; set; }

    /// <summary>
    /// TIPO DO PRODUTO.
    /// </summary>
    [JsonProperty("tipo_produto")]
    public string TIPO_PRODUTO { get; set; }

    /// <summary>
    /// VALOR TIPO DO PRODUTO.
    /// </summary>
    [JsonProperty("valor_tipo_produto")]
    public string VALOR_TIPO_PRODUTO { get; set; }

    /// <summary>
    /// QUANTIDADE DE UNIDADE DE VENDA POR CAIXA DO FORNECEDOR.
    /// </summary>
    [JsonProperty("quantidade_unidade_venda_cx_fornecedor")]
    public int? QDE_UND_VENDA_CX_FORNECEDOR { get; set; }

    /// <summary>
    /// QUANTIDADE DE UNIDADE DE VENDA POR CAIXA DE KIT.
    /// </summary>
    [JsonProperty("quantidade_unidade_venda_cx_kit")]
    public int? QDE_UND_VENDA_CX_KIT { get; set; }

    /// <summary>
    /// DESCRIÇÃO DA APRESENTAÇÃO DA MERCADORIA MARKETING NOVA.
    /// </summary>
    [JsonProperty("html")]
    public string DESAPRMERMKTRVONVO { get; set; }

    /// <summary>
    /// URL DA IMAGEM.
    /// </summary>
    [JsonProperty("imagem")]
    public string IMGLNK { get; set; } = string.Empty;

   
    /// <summary>
    /// CODIGO DO SKU DO PRODUTO.
    /// </summary>
    [JsonProperty("codigoProdutoCatalogo")]
    public long CODPRD { get; set; }

    /// <summary>
    /// Código do Seller.
    /// </summary>
    [JsonIgnore]
    public long CODSLR { get; set; }

    /// <summary>
    /// Código de Divisão de Compras
    /// </summary>
    [JsonProperty("fabricanteEfacil")]
    public string DESDIVCMP { get; set; } = string.Empty;

    /// <summary>
    /// Código da Diretoria (1P B2B/Efacil)
    /// </summary>
    [JsonIgnore] public int? CODDRT { get; set; }

    /// <summary>
    /// Descrição da Diretoria (1P B2B/Efacil)
    /// </summary>
    [JsonIgnore] public string DESDRT { get; set; }

    [JsonIgnore] public TipoProdutoEfacil TipoProdutoEfacil { get; set; } = TipoProdutoEfacil.FilhoOuSimples;

    /// <summary>
    /// Indicador do tipo de produto e-Fácil
    /// </summary>
    [JsonProperty("tipoProdutoEfacil")]
    public string TipoProdutoEfacilStr => ((char)TipoProdutoEfacil).ToString();

    [JsonProperty("grupoSimilar")]
    public ProductDetailGrupoSimilar? GrupoSimilar { get; set; }

    /// <summary>
    /// Imagens do produto.
    /// </summary>
    [JsonProperty("imagens")]
    public List<ProductImage> ProductImages { get; set; } = new();

    /// <summary>
    /// Atributos. (somente marcados)
    /// </summary>
    [JsonProperty("atributos")]
    public List<ProductAttribute> Attributes { get; set; }
}

public class ProductDetailGrupoSimilar
{
    [JsonProperty("tituloProdutoPai")]
    public string TituloProdutoPai { get; set; }
    [JsonProperty("codigoAtributo")]
    public long CodigoAtributo { get; set; }
    [JsonProperty("descricaoAtributo")]
    public string DescricaoAtributo { get; set; }
    [JsonProperty("produtos")]
    public List<ProductDetailGrupoSimilarProductEntry> Produtos { get; set; } = new();
}

public class ProductDetailGrupoSimilarProductEntry
{
    [JsonProperty("codMer")] public string CodMer { get; set; }
    [JsonProperty("isPrincipal")] public bool IsPrincipal { get; set; }
}

/// <summary>
/// Imagem do Produto.
/// </summary>
public class ProductImage
{
    /// <summary>
    /// URL DA IMAGEM.
    /// </summary>
    [JsonProperty("image")]
    public string IMGLNK { get; set; }

    /// <summary>
    /// URI DA IMAGEM.
    /// </summary>
    [JsonProperty("URI")]
    public string URI { get; set; } = string.Empty;
    /// <summary>
    /// VERSION DA IMAGEM.
    /// </summary>
    [JsonProperty("version")]
    public string VERSION { get; set; } = string.Empty;
    /// <summary>
    /// DOMAIN DA IMAGEM.
    /// </summary>
    [JsonProperty("domain")]
    public string DOMAIN { get; set; } = string.Empty;
    /// <summary>
    /// Índice de sequência da imagem
    /// </summary>
    [JsonProperty("index")]
    public ushort Index { get; set; }

    /// <summary>
    /// CODIGO TIPO DA IMAGEM.
    /// </summary>
    [JsonProperty("type")]
    public int CODTIPIMG { get; set; }

    /// <summary>
    /// Indicador de imagem hero
    /// </summary>
    [JsonProperty("isHero")]
    public bool IsHero => CODTIPIMG == 5;

    /// <summary>
    /// Código Mercadoria Seller.
    /// </summary>
    [JsonProperty("codMerSrr")]
    public string SKU_3P { get; set; }
}

/// <summary>
/// Atributo do produto.
/// </summary>
public class ProductAttribute
{
    /// <summary>
    /// Nome do atributo.
    /// </summary>
    [JsonProperty("name")]
    public string NAME { get; set; }

    /// <summary>
    /// Valor do atributo.
    /// </summary>
    [JsonProperty("value")]
    public string VALUE { get; set; }

    /// <summary>
    /// Código do atributo no Catálogo corporativo
    /// </summary>
    [JsonProperty("codigoAtributoCatalogo")]
    public long CODATRPRD { get; set; }

    /// <summary>
    /// Indicador de atributo de definição
    /// </summary>
    [JsonIgnore]
    public int INDATRDEF { get; set; }

    /// <summary>
    /// Código do atributo de definição
    /// </summary>
    [JsonIgnore]
    public long? CODATRPRDEXT { get; set; }

    [JsonProperty("codigoAtributoEfacil")]
    public string CodigoAtributoEfacil
    {
        get
        {
            if (!CODATRPRDEXT.HasValue) return null;
            return $"{INDATRDEF switch
            {
                0 => "ATR_",
                1 => "DEF_",
                -999 => "DIR_",
                _ => throw new NotImplementedException($"O Valor \"{INDATRDEF}\" não foi implementado para {nameof(INDATRDEF)}.")
            }}{CODATRPRDEXT.Value}";
        }
    }
}

/// <summary>
/// Pesquisa cacheada produto.
/// </summary>
public class ProductDetailSearch
{
    /// <summary>
    /// Construtor.
    /// </summary>
    public ProductDetailSearch() { }

    /// <summary>
    /// Construtor.
    /// </summary>
    /// <param name="codMer">Código mercadoria.</param>
    /// <param name="codOpdTrcEtn">Tipo da operação.</param>
    /// <param name="codPlt">Código da plataforma</param>
    /// <param name="productDetail">Detalhes do produto.</param>
    public ProductDetailSearch(string codMer, int codOpdTrcEtn, ushort codPlt, ProductDetail productDetail)
    {
        CodMer = codMer;
        CodOpdTrcEtn = codOpdTrcEtn;
        CodPlt = codPlt;
        ProductDetail = productDetail;
    }

    /// <summary>
    /// Detalhes do produto.
    /// </summary>
    public ProductDetail ProductDetail { get; set; }

    /// <summary>
    /// Código mercadoria.
    /// </summary>
    public string CodMer { get; set; }

    /// <summary>
    /// Tipo da operação.
    /// </summary>
    public int CodOpdTrcEtn { get; set; }

    /// <summary>
    /// Código da plataforma
    /// </summary>
    public ushort CodPlt { get; set; }
}

/// <summary>
/// Pesquisa cacheada lista de imagens.
/// </summary>
public class ImageListSearch
{
    /// <summary>
    /// Construtor.
    /// </summary>
    public ImageListSearch() { }

    /// <summary>
    /// Construtor.
    /// </summary>
    /// <param name="codSkus">Lista de códigos SKU de mercadorias.</param>
    /// <param name="images">Imagens.</param>
    public ImageListSearch(List<string> codSkus, List<ProductImage> images)
    {
        CodSkus = codSkus;
        Images = images;
    }

    /// <summary>
    /// Imagens.
    /// </summary>
    public List<ProductImage> Images { get; set; }

    /// <summary>
    /// Lista de códigos SKU de mercadorias.
    /// </summary>
    public List<string> CodSkus { get; set; }

    /// <summary>
    /// Código da plataforma
    /// </summary>
    public ushort CodPlt { get; set; }
}
