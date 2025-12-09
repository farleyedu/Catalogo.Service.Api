namespace Catalogo.Service.Api;

/// <summary>
/// Estrutura auxiliar para leitura do SQL de grupo similar.
/// </summary>
public class GrupoSimilarProdutoAtributo
{
    public long CODGRPSMRMER { get; set; }
    public string DESGRPSMRMER { get; set; }
    public long CODPRD { get; set; }
    public int INDPRDPCP { get; set; }
    public long CODMERMRT { get; set; }
    public string CODBRRUNDVNDCSM { get; set; }
    public string DESPRDPAD { get; set; }
    public long? CODSECCSM { get; set; }
    public string NOMSECCSM { get; set; }
    public long? CODCTGCSM { get; set; }
    public string DESCTGCSM { get; set; }
    public long? CODSUBCTGCSM { get; set; }
    public string DESSUBCTGPRDCSM { get; set; }
    public long? CODMRCCTL { get; set; }
    public string DESMRCCTL { get; set; }
    public long CODATRPRD { get; set; }
    public string DESATRPRD { get; set; }
    public long? CODOPCATR { get; set; }
    public string DESOPCATR { get; set; }
    public int FLOPC { get; set; }

    public bool IsPrincipal => INDPRDPCP == 1;
}
