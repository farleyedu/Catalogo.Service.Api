using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Catalogo.Service.Api.Controllers;

/// <summary>
/// Controller de Preferência.
/// </summary>
/// <remarks>Leon Denis @ByteOn</remarks>
[ApiController, Route("api/[controller]/[action]"), Produces("application/json")]
public class PreferenceController : ControllerBase
{

    /// <summary>
    /// Obtem as preferências.
    /// </summary>
    /// <param name="codAti">Código de atividade do cliente. (Apenas para ver os que estão disponíveis para atividade)</param>
    /// <param name="codCli">Código do cliente. (Apenas para ver os que já foram selecionados)</param>
    /// <param name="codCnl">Código do canal. (plataforma)</param>
    /// <returns>Preferências.</returns>
    [HttpGet]
    public async Task<IActionResult> GetPreference([FromQuery(Name = "activityCode")] int? codAti, [FromQuery(Name = "clientCode")] long? codCli, [FromQuery(Name = "channelCode")] int? codCnl)
    {
        if (codAti == null && codCli == null)
            throw new Exception("É necessário informar activityCode e/ou clientCode para efetuar a consulta.");

        PreferenceBLL bll = new();
        return Ok(new WebReturn<List<Preference>> { Result = await bll.GetPreference(codAti, codCli, codCnl) });
    }

    /// <summary>
    /// Marca a opção selecionada pelo cliente.
    /// </summary>
    /// <param name="clientPreference">Preferências do cliente.</param>
    /// <returns>Status da seleção.</returns>
    [HttpPost]
    public async Task<IActionResult> SetClientPreference([FromBody] ClientPreference clientPreference)
    {
        PreferenceBLL bll = new();
        await bll.InsertClientPreference(clientPreference);
        return Ok(new WebReturn { Message = $"{(clientPreference.Options?.Count == 1 ? "Opção marcada" : "Opções marcadas")} com sucesso!" });
    }

    /// <summary>
    /// Desmarca a opção selecionada pelo cliente.
    /// </summary>
    /// <param name="clientPreference">Preferência do cliente.</param>
    /// <returns>Status da seleção.</returns>
    [HttpPut]
    public async Task<IActionResult> UnsetClientPreference([FromBody] ClientPreference clientPreference)
    {
        PreferenceBLL bll = new();
        await bll.RemoveClientPreference(clientPreference);
        return Ok(new WebReturn { Message = $"{(clientPreference.Options?.Count == 1 ? "Opção desmarcada" : "Opções desmarcadas")} com sucesso!" });
    }

    /// <summary>
    /// Muda o estado do modal para o cliente.
    /// </summary>
    /// <param name="modalState">Estado do modal.</param>
    /// <returns>Estado da atualização modal.</returns>
    [HttpPost]
    public async Task<IActionResult> SetModalState([FromBody] ClientModalState modalState)
    {
        PreferenceBLL bll = new();
        await bll.SetModalState(modalState);
        return Ok(new WebReturn { Message = "Estado do modal atualizado com sucesso." });
    }
}
