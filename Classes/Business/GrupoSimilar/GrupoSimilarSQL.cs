namespace Catalogo.Service.Api;

/// <summary>
/// Consultas relacionadas a grupo similar.
/// </summary>
public static class GrupoSimilarSQL
{
    /// <summary>
    /// Consulta os dados de grupo similar/atributos partindo do código do produto.
    /// </summary>
    public static string GetGrupoSimilarProdutoAtributo()
    {
        return @"SELECT
                    GRP.CODGRPSMRMER,
                    GRP.DESGRPSMRMER,
                    RLC.CODPRD,
                    RLC.INDPRDPCP,
                    PRD.CODMERMRT,
                    PRD.CODBRRUNDVNDCSM,
                    PRD.DESPRDPAD,
                    SEC.CODSEC           AS CODSECCSM,
                    SEC.NOMSEC           AS NOMSECCSM,
                    CAT.CODCTG           AS CODCTGCSM,
                    CAT.DESCTG           AS DESCTGCSM,
                    PRD.CODSUBCTGCSM,
                    SUBCTG.DESSUBCTGPRD  AS DESSUBCTGPRDCSM,
                    MRC.CODMRCCTL,
                    MRC.DESMRCCTL,
                    GRPATR.CODATRPRD,
                    ATR.DESATRPRD,
                    OPC.CODOPCATR,
                    OPC.DESOPCATR,
                    DECODE(ATRPRD.CODPRD, NULL, 0, 1) AS FLOPC
                FROM MRT.CADGRPSMRCTLSMA GRP
                INNER JOIN MRT.RLCGRPSMRPRDCTLSMA RLC
                    ON RLC.CODGRPSMRMER = GRP.CODGRPSMRMER
                INNER JOIN MRT.CADPRDCTLSMA PRD
                    ON PRD.CODPRD = RLC.CODPRD
                LEFT JOIN MRT.CADSUBCTGCLTCSMSMA SUBCTG
                    ON SUBCTG.CODSUBCTGPRD = PRD.CODSUBCTGCSM
                LEFT JOIN MRT.CADSECCLTCSMSMA SEC
                    ON SEC.CODSEC = SUBCTG.CODSEC
                LEFT JOIN MRT.CADCTGCLTCSMSMA CAT
                    ON CAT.CODCTG = SUBCTG.CODCTG
                LEFT JOIN MRT.CADMRCPRDCTLSMA MRC
                    ON MRC.CODMRCCTL = PRD.CODMRCCTL
                INNER JOIN MRT.RLCATRHIRCTLSMA GRPATR
                    ON GRPATR.CODGRPSMRMER = GRP.CODGRPSMRMER
                INNER JOIN MRT.CADATRCTLSMA ATR
                    ON ATR.CODATRPRD = GRPATR.CODATRPRD
                INNER JOIN MRT.RLCOPCATRCTLSMA OPC
                    ON OPC.CODATRPRD = ATR.CODATRPRD
                LEFT JOIN MRT.RLCATRPRDCTLSMA ATRPRD
                    ON ATRPRD.CODATRPRD = ATR.CODATRPRD
                    AND ATRPRD.CODOPCATR = OPC.CODOPCATR
                    AND ATRPRD.CODPRD = PRD.CODPRD
            WHERE EXISTS
                (
                    SELECT 1
                    FROM MRT.RLCGRPSMRPRDCTLSMA X
                    WHERE X.CODGRPSMRMER = GRP.CODGRPSMRMER
                        AND X.CODPRD = :CODPRD
                )";
    }

    public static string GetGrupoProduto()
    {
        return @"SELECT RLC.CODGRPSMRMER,
                        RLC.CODPRD,
                        RLC.INDPRDPCP,
                        PRD.CODMERMRT,
                        PRD.CODBRRUNDVNDCSM,
                        PRD.DESPRDPAD,
                        SEC.CODSEC CODSECCSM,
                        SEC.NOMSEC NOMSECCSM,
                        CAT.CODCTG CODCTGCSM,
                        CAT.DESCTG DESCTGCSM,
                        PRD.CODSUBCTGCSM,
                        SUBCTG.DESSUBCTGPRD DESSUBCTGPRDCSM,
                        MRC.CODMRCCTL,
                        MRC.DESMRCCTL
                 FROM MRT.RLCGRPSMRPRDCTLSMA RLC
                 INNER JOIN MRT.CADPRDCTLSMA PRD ON PRD.CODPRD = RLC.CODPRD
                 LEFT JOIN MRT.CADSUBCTGCLTCSMSMA SUBCTG ON SUBCTG.CODSUBCTGPRD = PRD.CODSUBCTGCSM
                 LEFT JOIN MRT.CADSECCLTCSMSMA SEC ON SEC.CODSEC = SUBCTG.CODSEC
                 LEFT JOIN MRT.CADCTGCLTCSMSMA CAT ON CAT.CODCTG = SUBCTG.CODCTG
                 LEFT JOIN MRT.CADMRCPRDCTLSMA MRC ON MRC.CODMRCCTL = PRD.CODMRCCTL
                 WHERE RLC.CODGRPSMRMER = :CODGRPSMRMER";
    }

    public static string GetAtributosProduto()
    {
        return @"SELECT 
                    CODATRPRD,
                    CODOPCATR,
                    CODPRD
                FROM MRT.RLCATRPRDCTLSMA
                WHERE CODPRD = :CODPRD
                ORDER BY CODATRPRD";
    }

    public static string GetDescricaoOpcaoAtributo()
    {
        return @"SELECT DESOPCATR
                 FROM MRT.RLCOPCATRCTLSMA
                 WHERE CODATRPRD = :CODATRPRD
                   AND CODOPCATR = :CODOPCATR";
    }

    public static string GetNomeAtributo()
    {
        return @"SELECT DESATRPRD
                 FROM MRT.CADATRCTLSMA
                 WHERE CODATRPRD = :CODATRPRD";
    }
}
