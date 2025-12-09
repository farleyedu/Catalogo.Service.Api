using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace Catalogo.Service.Api;

/// <summary>
/// Camada de Negócio - Preferência.
/// </summary>
public class PreferenceBLL
{

    /// <summary>
    /// Obtem todas as preferências.
    /// </summary>
    /// <param name="codCli">Código do cliente.</param>
    /// <param name="codAti">Código de atividade do cliente.</param>
    /// <param name="codCnl">Código do canal. (plataforma)</param>
    /// <returns>Preferências.</returns>
    public async Task<List<Preference>> GetPreference(int? codAti, long? codCli, int? codCnl)
    {
        PreferenceDAL dal = new();

        List<Preference> preferences = await dal.GetPreference(codAti);

        if (preferences?.Count > 0)
        {
            List<PreferenceOption> options = await dal.GetOptions(preferences.Select(p => p.CODPFR).ToList(), codCnl, codCli, codAti);

            if (options?.Count > 0) foreach (Preference preference in preferences)
                preference.Options = options.Where(o => o.CODPFR == preference.CODPFR).ToList();
        }

        return preferences;
    }

    /// <summary>
    /// Insere a opção selecionada pelo cliente.
    /// </summary>
    /// <param name="preference">Preferência do cliente.</param>
    /// <returns>Quantidade de linhas afetadas.</returns>
    public async Task<int> InsertClientPreference(ClientPreference preference)
    {
        if (preference.Options?.Count == 0)
            throw new Exception("Favor informar as opções do cliente.");

        // Transação.
        using TransactionScope scope = new(TransactionScopeOption.Required);
        // Access layer.
        PreferenceDAL dal = new();
        // Count.
        if (await dal.GetClientPreferenceCount(preference.CODCLI) is int optionCount && optionCount == 5)
            throw new Exception("Não é possível marcar mais do que 5 opções(preferências).");
        // Obtendo o status do cliente.
        preference.TIPCLI = Util.GetClientType((preference.Options?.Count ?? 0) + optionCount, true);
        // Resultado.
        int result = 0;
        // Inserindo preferências.
        foreach (ClientOption option in preference.Options)
        {
            if (await dal.GetPreferenceCount(option.CODRLC, option.CODCNL) == 0)
                throw new Exception($"A relação {option.CODRLC} adstrita com o canal {option.CODCNL} não existe.");
            if (await dal.GetClientPreferenceCount(preference.CODCLI, option.CODRLC) > 0)
                throw new Exception($"A relação {option.CODRLC} adstrita com o cliente {preference.CODCLI} já existe.");

            result += await dal.InsertClientPreference(option.CODRLC, preference.CODCLI, option.CODCNL);
        }
        // Atualizando status do cliente.
        result += await dal.UpdateClientStatus(preference.CODCLI, preference.TIPCLI);
        // Complete.
        scope.Complete();

        return result;
    }

    /// <summary>
    /// Remove a opção selecionada pelo cliente.
    /// </summary>
    /// <param name="preference">Preferência do cliente.</param>
    /// <returns>Quantidade de linhas afetadas.</returns>
    public async Task<int> RemoveClientPreference(ClientPreference preference)
    {
        if (preference.Options?.Count > 0)
            preference.Options.ForEach(opt => opt.CODCLI = preference.CODCLI);
        else
            throw new Exception("Favor informar as opções do cliente.");

        // Transação.
        using TransactionScope scope = new(TransactionScopeOption.Required);

        PreferenceDAL dal = new();
        // Removendo a preferência.
        int result = await dal.RemoveClientPreference(preference.Options);

        if (result == 0) throw new Exception("O canal, cliente e/ou preferência(opção) informados não existem.");

        // Obtendo o tipo do cliente.
        int clientType = Util.GetClientType(await dal.GetClientPreferenceCount(preference.CODCLI));
        // Atualizando status do cliente.
        result += await dal.UpdateClientStatus(preference.CODCLI, clientType);

        // Complete.
        scope.Complete();

        return result;
    }

    /// <summary>
    /// Seta o estado do modal para o cliente.
    /// </summary>
    /// <param name="modalState">Estado do modal.</param>
    /// <returns>Quantidade de linhas afetadas.</returns>
    public async Task<int> SetModalState(ClientModalState modalState)
    {
        if (!modalState.OpenModal) throw new Exception("Parametrização 'openModal = false' é inválida.");

        int result = await new PreferenceDAL().UpdateClientStatus(modalState.CODCLI, 2);

        if (result == 0) throw new Exception("O cliente informado não existe.");

        return result;
    }

}
