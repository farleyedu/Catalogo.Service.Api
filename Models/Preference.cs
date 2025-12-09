using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Catalogo.Service.Api;

/// <summary>
/// Preferência.
/// </summary>
public class Preference
{
    /// <summary>
    /// Código preferência.
    /// </summary>
    [JsonProperty("preferenceCode")]
    public long CODPFR { get; set; }

    /// <summary>
    /// Descricão título.
    /// </summary>
    [JsonProperty("preferenceDescription")]
    public string DESTIT { get; set; }

    /// <summary>
    /// Opções.
    /// </summary>
    [JsonProperty("options")]
    public List<PreferenceOption> Options { get; set; }
}

/// <summary>
/// Opção da preferência.
/// </summary>
public class PreferenceOption
{
    /// <summary>
    /// Código da relação.
    /// </summary>
    [JsonProperty("relationCode")]
    public long CODRLC { get; set; }

    /// <summary>
    /// Código preferência.
    /// </summary>
    [JsonProperty("preferenceCode")]
    public long CODPFR { get; set; }

    /// <summary>
    /// Código do canal. (plataforma)
    /// </summary>
    [JsonProperty("channelCode")]
    public int CODCNL { get; set; }

    /// <summary>
    /// Código da categoria.
    /// </summary>
    [JsonProperty("categoryCode")]
    public long CODCTG { get; set; }

    /// <summary>
    /// Descrição da categoria.
    /// </summary>
    [JsonProperty("categoryDescription")]
    public string DESCTG { get; set; }

    /// <summary>
    /// Código da atividade.
    /// </summary>
    [JsonProperty("activityCode")]
    public int CODATI { get; set; }

    /// <summary>
    /// Nome da atividade.
    /// </summary>
    [JsonProperty("activityName")]
    public string NOMATI { get; set; }

    /// <summary>
    /// Ícone da categoria.
    /// </summary>
    [JsonProperty("icon")]
    public string DESICNDPTBTB { get; set; }

    /// <summary>
    /// Imagem da categoria.
    /// </summary>
    [JsonProperty("image")]
    public string DESURLIMG { get; set; }

    /// <summary>
    /// Selecionado.
    /// </summary>
    [JsonProperty("selected")]
    public bool SELECTED { get; set; }
}

/// <summary>
/// Preferência do cliente.
/// </summary>
public class ClientPreference
{
    /// <summary>
    /// Tipo do cliente.
    /// </summary>
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public int TIPCLI { get; set; }

    /// <summary>
    /// Código do cliente.
    /// </summary>
    [JsonProperty("clientCode")]
    [JsonPropertyName("clientCode")]
    [BindProperty(Name = "clientCode")]
    public long CODCLI { get; set; }

    /// <summary>
    /// Opções do cliente.
    /// </summary>
    [JsonProperty("options")]
    [JsonPropertyName("options")]
    [BindProperty(Name = "options")]
    public List<ClientOption> Options { get; set; }
}

/// <summary>
/// Opção do cliente.
/// </summary>
public class ClientOption
{
    /// <summary>
    /// Código da relação.
    /// </summary>
    [JsonProperty("relationCode")]
    [JsonPropertyName("relationCode")]
    [BindProperty(Name = "relationCode")]
    public long CODRLC { get; set; }

    /// <summary>
    /// Código do canal. (plataforma)
    /// </summary>
    [JsonProperty("channelCode")]
    [JsonPropertyName("channelCode")]
    [BindProperty(Name = "channelCode")]
    public int CODCNL { get; set; }

    /// <summary>
    /// Código do cliente.
    /// </summary>
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public long CODCLI { get; set; }
}

/// <summary>
/// Modal App.
/// </summary>
public class ClientModalState
{
    /// <summary>
    /// Código do cliente.
    /// </summary>
    [JsonProperty("clientCode")]
    [JsonPropertyName("clientCode")]
    [BindProperty(Name = "clientCode")]
    public long CODCLI { get; set; }

    /// <summary>
    /// Status aberto/fechado.
    /// </summary>
    [JsonProperty("openModal")]
    [JsonPropertyName("openModal")]
    [BindProperty(Name = "openModal")]
    public bool OpenModal { get; set; }
}
