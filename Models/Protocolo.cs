using Newtonsoft.Json;
using System;
using System.Text.Json.Serialization;

namespace Catalogo.Service.Api;

/// <summary>
/// Modelo protocolo.
/// </summary>
public class Protocolo
{
    /// <summary>
    /// Sku do produto.
    /// </summary>
    [JsonPropertyName("sku"), JsonProperty("sku")]
    public string CODMERSRR { get; set; }

    /// <summary>
    /// Código de barras do produto.
    /// </summary>
    [JsonPropertyName("ean"), JsonProperty("ean")]
    public long CODBRRUNDVNDCSM { get; set; }

    /// <summary>
    /// Data de cadastramento.
    /// </summary>
    [JsonPropertyName("data"), JsonProperty("data")]
    public DateTime DATPCS { get; set; }

    /// <summary>
    /// Hash único para controle customizado. (opcional)
    /// </summary>
    [JsonPropertyName("hash"), JsonProperty("hash")]
    public string CODSKUHSH { get; set; }
}
