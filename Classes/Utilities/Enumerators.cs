namespace Catalogo.Service.Api;

public enum TipoOperacaoDetalheProduto
{
    Nenhum = 0, // NOTE: Talvez seja um tipo DUMMY (Inválido)
    App = 9,
    PortalCliente = 10,
    PortalVendas = 14,
}

public enum PlataformasCatalogoCorp
{
    Smart = 1,
    EFacil = 2,
    B2B = 3,
    CampoConstrucao = 4
}

public enum TipoProdutoEfacil
{
    /// <summary>
    /// Tipo "F" | Filho ou Simples
    /// </summary>
    FilhoOuSimples = 'F',

    /// <summary>
    /// Tipo "P" | Pai ou Agrupador
    /// </summary>
    Pai = 'P'
}
