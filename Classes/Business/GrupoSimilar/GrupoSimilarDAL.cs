using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalogo.Service.Api;

/// <summary>
/// Camada de acesso para recursos de grupo similar.
/// </summary>
public class GrupoSimilarDAL : OraAccess
{
    /// <summary>
    /// Obtém estrutura de grupo similar + atributos a partir do código do produto do catálogo.
    /// </summary>
    public Task<List<GrupoSimilarProdutoAtributo>> GetGrupoSimilarProdutoAtributoAsync(long codPrd)
    {
        return Command.UseQuery(GrupoSimilarSQL.GetGrupoSimilarProdutoAtributo())
            .AddParameter("CODPRD", codPrd)
            .ExecuteList<GrupoSimilarProdutoAtributo>();
    }

    public Task<List<GrupoSimilarProdutoInfo>> GetGrupoProdutosAsync(int codGrpsmrmer)
    {
        return Command.UseQuery(GrupoSimilarSQL.GetGrupoProduto())
            .AddParameter("CODGRPSMRMER", codGrpsmrmer)
            .ExecuteList<GrupoSimilarProdutoInfo>();
    }

    public Task<List<GrupoSimilarAtributoProduto>> GetAtributosProdutoAsync(long codPrd)
    {
        return Command.UseQuery(GrupoSimilarSQL.GetAtributosProduto())
            .AddParameter("CODPRD", codPrd)
            .ExecuteList<GrupoSimilarAtributoProduto>();
    }

    public async Task<string> GetDescricaoOpcaoAtributoAsync(long codAtrPrd, long codOpcAtr)
    {
        return (await Command.UseQuery(GrupoSimilarSQL.GetDescricaoOpcaoAtributo())
            .AddParameter("CODATRPRD", codAtrPrd)
            .AddParameter("CODOPCATR", codOpcAtr)
            .ExecuteList<string>()).FirstOrDefault();
    }

    public async Task<string> GetNomeAtributoAsync(long codAtrPrd)
    {
        return (await Command.UseQuery(GrupoSimilarSQL.GetNomeAtributo())
            .AddParameter("CODATRPRD", codAtrPrd)
            .ExecuteList<string>()).FirstOrDefault();
    }
}
