namespace Catalogo.Service.Api;

/// <summary>
/// Retorno Web padrão.
/// </summary>
/// <remarks>Leon Denis @ByteOn</remarks>
public class WebReturn
{
    /// <summary>
    /// Código de retorno.
    /// </summary>
    public int Code { get; set; }

    /// <summary>
    /// Mensagem de retorno.
    /// </summary>
    public string Message { get; set; }
}

/// <summary>
/// Retorno Web padrão.
/// </summary>
/// <remarks>Leon Denis @ByteOn</remarks>
public class WebReturn<T> : WebReturn
{
    /// <summary>
    /// Resultado do retorno.
    /// </summary>
    public T Result { get; set; }
}
