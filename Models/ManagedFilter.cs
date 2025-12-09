using System;

namespace Catalogo.Service.Api;

/// <summary>
/// Gestão de filtro.
/// </summary>
public class ManagedFilter
{
    /// <summary>
    /// É seller martins?
    /// </summary>
    public bool IsMartins { get; set; }

    /// <summary>
    /// Seller.
    /// </summary>
    public string Seller { get; set; }

    /// <summary>
    /// Código da mercadoria.
    /// </summary>
    public string CodMer { get; set; }

    /// <summary>
    /// Código da mercadoria. (long)
    /// </summary>
    public long CodMerLng { get; set; }

    /// <summary>
    /// Construtor.
    /// </summary>
    public ManagedFilter() { }

    /// <summary>
    /// Construtor.
    /// </summary>
    /// <param name="isMartins">É seller martins?</param>
    /// <param name="seller">Seller.</param>
    /// <param name="codMer">Código da mercadoria.</param>
    public ManagedFilter(bool isMartins, string seller, string codMer)
    {
        IsMartins = isMartins;
        Seller = seller;
        CodMer = codMer;
        if (IsMartins) CodMerLng = Convert.ToInt64(codMer);
    }

}
