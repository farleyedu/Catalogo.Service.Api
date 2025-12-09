using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalogo.Service.Api;

/// <summary>
/// Camada de Acesso - Preferência.
/// </summary>
public class PreferenceDAL : OraAccess
{

    /// <summary>
    /// Obtem todas as preferências.
    /// </summary>
    /// <param name="codAti">Código de atividade do cliente.</param>
    /// <returns>Preferências.</returns>
    public async Task<List<Preference>> GetPreference(int? codAti)
    {
        return await Command.UseQuery(PreferenceSQL.GetPreference(codAti))
            .AddParameter("CODATI", codAti, codAti.HasValue)
            .ExecuteList<Preference>();
    }

    /// <summary>
    /// Obtem as opções da preferência.
    /// </summary>
    /// <param name="codPfrs">Códigos de preferência.</param>
    /// <param name="codCnl">Código do canal.</param>
    /// <param name="codCli">Código do cliente.</param>
    /// <param name="codAti">Código de atividade do cliente.</param>
    /// <returns>Opções.</returns>
    public async Task<List<PreferenceOption>> GetOptions(List<long> codPfrs, int? codCnl, long? codCli, int? codAti)
    {
        return await Command.UseQuery(PreferenceSQL.GetOptions(codPfrs, codCnl, codCli, codAti))
            .AddParameter("CODCLI", codCli, codCli.HasValue)
            .AddParameter("CODCLI", codCli, codCli.HasValue && codAti == null)
            .AddParameter("CODCNL", codCnl, codCnl.HasValue)
            .AddParameter("CODATI", codAti, codAti.HasValue)
            .AddParameters("CODPFR", codPfrs)
            .ExecuteList<PreferenceOption>();
    }

    /// <summary>
    /// Insere a opção selecionada pelo cliente.
    /// </summary>
    /// <param name="codRlc">Código da relação.</param>
    /// <param name="codCli">Código do cliente.</param>
    /// <param name="codCnl">Código do canal.</param>
    /// <returns>Quantidade de linhas afetadas.</returns>
    public async Task<int> InsertClientPreference(long codRlc, long codCli, int codCnl)
    {
        return await Command.UseQuery(PreferenceSQL.InsertClientPreference())
            .AddParameter("CODCNL", codCnl)
            .AddParameter("CODRLC", codRlc, 2)
            .AddParameter("CODCLI", codCli)
            .ExecNonQuery();
    }

    /// <summary>
    /// Remove a opção selecionada pelo cliente.
    /// </summary>
    /// <param name="options">Opções de preferência do cliente.</param>
    /// <returns>Quantidade de linhas afetadas.</returns>
    public async Task<int> RemoveClientPreference(List<ClientOption> options)
    {
        return await Command.UseQuery(PreferenceSQL.RemoveClientPreference(), arrayBindCount: options.Count)
            .AddParameterArray(options.Select(o => o.CODCNL))
            .AddParameterArray(options.Select(o => o.CODRLC))
            .AddParameterArray(options.Select(o => o.CODRLC))
            .AddParameterArray(options.Select(o => o.CODCLI))
            .ExecNonQuery();
    }

    /// <summary>
    /// Atualiza o status do cliente.
    /// </summary>
    /// <param name="codCli">Código do cliente.</param>
    /// <param name="tipCli">Tipo do cliente.</param>
    /// <returns>Quantidade de linhas afetadas.</returns>
    public async Task<int> UpdateClientStatus(long codCli, int tipCli)
    {
        return await Command.UseQuery(PreferenceSQL.UpdateClientStatus())
            .AddParameter("TIPCLI", tipCli)
            .AddParameter("CODCLI", codCli)
            .ExecNonQuery();
    }

    /// <summary>
    /// Obtem a contagem de preferências.
    /// </summary>
    /// <param name="codRlc">Código da relação.</param>
    /// <param name="codCnl">Código do canal.</param>
    /// <returns>Quantidade de itens.</returns>
    public async Task<int> GetPreferenceCount(long codRlc, int codCnl)
    {
        return await Command.UseQuery(PreferenceSQL.GetPreferenceCount())
            .AddParameter("CODRLC", codRlc)
            .AddParameter("CODCNL", codCnl)
            .ExecScalar<int>();
    }

    /// <summary>
    /// Obtem a contagem de preferências.
    /// </summary>
    /// <param name="codCli">Código do cliente.</param>
    /// <param name="codRlc">Código da relação.</param>
    /// <returns>Quantidade de itens.</returns>
    public async Task<int> GetClientPreferenceCount(long codCli, long? codRlc = null)
    {
        return await Command.UseQuery(PreferenceSQL.GetClientPreferenceCount(codRlc))
            .AddParameter("CODCLI", codCli)
            .AddParameter("CODRLC", codRlc, codRlc.HasValue)
            .ExecScalar<int>();
    }

}
