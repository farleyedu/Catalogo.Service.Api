using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalogo.Service.Api;

/// <summary>
/// Classe para métodos genéricos.
/// </summary>
public static class Util
{
    /// <summary>
    /// Erro Padrão do Controller.
    /// </summary>
    /// <param name="exception">Exceção.</param>
    /// <returns>WebReturn</returns>
    public static WebReturn ControllerError(Exception exception)
    {
        // Retornando o erro.
        return new WebReturn()
        {
            Code = 1,
            Message = $"Erro: {exception.Message}"
        };
    }

    /// <summary>
    /// Erro Padrão do Controller.
    /// </summary>
    /// <param name="exception">Exceção.</param>
    /// <param name="result">Objeto de retorno.</param>
    /// <returns>WebReturn</returns>
    public static WebReturn<T> ControllerError<T>(Exception exception, T result)
    {
        return new WebReturn<T>()
        {
            Code = 1,
            Message = $"Erro: {exception.Message}",
            Result = result
        };
    }

    /// <summary>
    /// Faz a pesquisa com o item no cache.
    /// </summary>
    /// <typeparam name="I">Tipo do item no cache.</typeparam>
    /// <param name="cacheKey">Chave do Cache.</param>
    /// <param name="searchFunction">Função para pesquisa do item.</param>
    /// <param name="searchAction">Ação de pesquisa do item.</param>
    /// <returns>Item do cache.</returns>
    /// <remarks>Leon Denis @ByteOn</remarks>
    public static async Task<I> PerformanceSearch<I>(string cacheKey, Func<List<I>, I> searchFunction, Func<Task<I>> searchAction)
    {
        if (!Constants.IsApplicationCacheEnabled) return await searchAction();

        if (HandCache.Get<List<I>>(cacheKey) is List<I> cachedItem && cachedItem != null)
        {
            if (searchFunction(cachedItem) is I validatedItem && validatedItem != null) return validatedItem;

            I searchedItem = await searchAction();
            cachedItem.Add(searchedItem);
            HandCache.Set(cacheKey, cachedItem);

            return searchedItem;
        }

        I searchedOutItem = await searchAction();
        List<I> newCachedList = new() { searchedOutItem };
        HandCache.Set(cacheKey, newCachedList);

        return searchedOutItem;
    }

    /// <summary>
    /// Gerencia o filtro informado.
    /// </summary>
    /// <param name="filter">Filtro.</param>
    /// <returns>Filtro gerenciado.</returns>
    /// <remarks>Leon Denis @ByteOn</remarks>
    public static ManagedFilter GetManagedFilter(string filter)
    {
        // Constante martins.
        const string martins = "martins";
        // Código do produto. / Seller.
        string codMer, seller;

        // Verificando parâmetro.
        if (filter.Split('_', StringSplitOptions.RemoveEmptyEntries) is string[] parameters && parameters.Length > 1)
        {
            seller = parameters[0].ToLower().Trim();
            codMer = seller.Equals(martins) ? parameters[1] : filter;
        }
        else
        {
            seller = martins;
            codMer = filter;
        }

        return new ManagedFilter(seller.Equals(martins), seller, codMer);
    }

    /// <summary>
    /// Normaliza uma string para formato de URL.
    /// </summary>
    /// <param name="value">Texto a ser normalizado</param>
    /// <returns>Texto normalizado, espaços convertidos para simbolos de menos '-'</returns>
    public static string NormalizeStringForUrl(string value)
    {
        // Se não tiver valor ou nulo, retornar o valor sem processar.
        if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
            return value;

        string normalizedString = value.Normalize(NormalizationForm.FormD);
        StringBuilder stringBuilder = new();

        foreach (char c in normalizedString)
        {
            switch (CharUnicodeInfo.GetUnicodeCategory(c))
            {
                case UnicodeCategory.LowercaseLetter:
                case UnicodeCategory.UppercaseLetter:
                case UnicodeCategory.DecimalDigitNumber:
                    stringBuilder.Append(c);
                    break;
                case UnicodeCategory.SpaceSeparator:
                case UnicodeCategory.ConnectorPunctuation:
                case UnicodeCategory.DashPunctuation:
                case UnicodeCategory.OtherPunctuation:
                    stringBuilder.Append('-');
                    break;
            }
        }
        string result = stringBuilder.ToString();
        // Remove traços/simbolo de menos '-' duplicados
        return string.Join("-", result.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries)).ToLower();
    }

    /// <summary>
    /// Verifica o status do cliente baseado na regra.
    /// </summary>
    /// <param name="optionCount">Quantidade de opções.</param>
    /// <param name="throwException">Lançar exceção?</param>
    /// <returns>Tipo do cliente.</returns>
    /// <exception cref="Exception">A contagem de preferências excedeu o limite.</exception>
    /// <remarks>Leon Denis @ByteOn</remarks>
    public static int GetClientType(int? optionCount, bool throwException = false)
    {
        return optionCount switch
        {
            0 => 2,
            (> 0 and <= 5) => 3,
            > 5 => throwException ? throw new Exception("Não é possível marcar mais do que 5 opções(preferências).") : 3,
            _ => throw new Exception("Não foi possível identificar a contagem de opções(preferências) do cliente.")
        };
    }

    /// <summary>
    /// Adiciona na lista a clausula OR parametrizada independente da quantidade de parâmetros.
    /// (Excedente de 1000)
    /// </summary>
    /// <typeparam name="T">Tipo da lista de parâmetro.</typeparam>
    /// <param name="stringBuilder">Construtor de String.</param>
    /// <param name="parameters">Parâmetros.</param>
    /// <param name="columnName">Nome da coluna de comparação.</param>
    /// <param name="parameterName">Nome do parâmetro.</param>
    /// <returns>StrinbBuilder.</returns>
    /// <remarks>Leon Denis @ByteOn</remarks>
    public static StringBuilder AppendOr<T>(this StringBuilder stringBuilder, List<T> parameters, string columnName, string parameterName)
    {
        if (parameters.Count < 1001)
            stringBuilder.AppendLine(string.Format("AND {0} IN ({1})", columnName, string.Join(",", parameters.Select(_ => parameterName))));
        else
        {
            List<string> orClauses = new();
            List<T> parametersAlt = parameters.ToList();
            stringBuilder.AppendLine("AND (");
            do
            {
                List<T> currentParameters = parametersAlt.Take(1000).ToList();
                orClauses.Add(string.Format("{0} IN ({1})", columnName, string.Join(",", currentParameters.Select(_ => parameterName))));
                parametersAlt.RemoveAll(parameter => currentParameters.Contains(parameter));
            } while (parametersAlt.Count > 0);
            stringBuilder.AppendLine(string.Format("{0} )", string.Join(" OR\n ", orClauses)));
        }
        return stringBuilder;
    }

}
