using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Catalogo.Service.Api;

/// <summary>
/// Camada de query's - Preferência.
/// </summary>
public class PreferenceSQL
{
    /// <summary>
    /// Obtem todas as preferências.
    /// </summary>
    /// <param name="codAti">Código de atividade do cliente.</param>
    /// <returns>Query</returns>
    public static string GetPreference(int? codAti)
    {
        StringBuilder builder = new(
            @"SELECT
                       PFR.CODPFR ,
                       PFR.DESTIT
                  FROM
                       MRT.CADPFRDPT PFR ");

        if (codAti != null)
            builder.AppendLine(
                @"WHERE EXISTS (
                                SELECT
                                        1
                                FROM
                                        MRT.RLCPFRCATATI RLC
                                WHERE
                                        PFR.CODPFR = RLC.CODPFR
                                AND     RLC.CODATI = :CODATI
                            )");

        return builder.ToString();
    }

    /// <summary>
    /// Obtem as opções da preferência.
    /// </summary>
    /// <param name="codPfrs">Códigos de preferência.</param>
    /// <param name="codCnl">Código do canal.</param>
    /// <param name="codCli">Código do cliente.</param>
    /// <param name="codAti">Código de atividade do cliente.</param>
    /// <returns>Query</returns>
    public static string GetOptions(List<long> codPfrs, int? codCnl, long? codCli, int? codAti)
    {
        StringBuilder builder = new(
            $@"SELECT
                           RLC.CODRLC              ,
                           RLC.CODPFR              ,
                           RLC.CODCNL              ,
                           CAT.CODCTG              ,
                           TRIM(CAT.DESCTG) DESCTG ,
                           ATI.CODATI              ,
                           TRIM(ATI.NOMATI) NOMATI ,
                           CAT.DESICNDPTBTB        ,
                           CAT.DESURLIMG           ,
                           {(codCli != null ?
                               @"(SELECT
                                    DECODE(SLC.CODPFR, NULL, 0, 1)
                               FROM
                                    MRT.RLCPFRCATCLI SLC
                               WHERE
                                    SLC.CODPFR     = RLC.CODPFR
                                    AND SLC.CODCAT = RLC.CODRLC
                                    AND SLC.CODCLI = :CODCLI)"
                               : "0")} SELECTED
                       FROM
                           MRT.RLCPFRCATATI RLC
                           JOIN
                                MRT.CADCTGCLTCSMSMA CAT
                                ON
                                     CAT.CODCTG = RLC.CODCTG
                           JOIN
                                MRT.T0100019 ATI
                                ON
                                     ATI.CODATI = RLC.CODATI ");

        if (codCli != null && codAti == null)
            builder.AppendLine(
                @"JOIN
                           MRT.RLCPFRCATCLI CLI
                           ON
                                CLI.CODPFR     = RLC.CODPFR
                                AND CLI.CODCAT = RLC.CODRLC
                                AND CLI.CODCLI = :CODCLI");

        builder.AppendLine("WHERE 1 = 1");

        if (codCnl != null)
            builder.AppendLine("AND RLC.CODCNL = :CODCNL");

        if (codAti != null)
            builder.AppendLine("AND RLC.CODATI = :CODATI");

        if (codPfrs != null)
            builder.AppendOr(codPfrs, "RLC.CODPFR", ":CODPFR");

        return builder.ToString();
    }

    /// <summary>
    /// Insere a opção selecionada pelo cliente.
    /// </summary>
    /// <returns>Query</returns>
    public static string InsertClientPreference()
    {
        return @"INSERT INTO MRT.RLCPFRCATCLI
                          ( 
                               CODRLC ,
                               CODPFR ,
                               CODCAT ,
                               CODCLI ,
                               DATCAD
                          )
                          VALUES
                          (
                               (SELECT NVL(MAX(CODRLC), 0) + 1 FROM MRT.RLCPFRCATCLI NEXT) ,
                               (SELECT CODPFR FROM MRT.RLCPFRCATATI WHERE CODCNL = :CODCNL AND CODRLC = :CODRLC) ,
                               :CODRLC ,
                               :CODCLI ,
                               SYSDATE
                          )";
    }

    /// <summary>
    /// Remove a opção selecionada pelo cliente.
    /// </summary>
    /// <returns>Query</returns>
    public static string RemoveClientPreference()
    {
        return @"DELETE
                          MRT.RLCPFRCATCLI
                     WHERE
                          CODPFR     = (SELECT CODPFR FROM MRT.RLCPFRCATATI WHERE CODCNL = :CODCNL AND CODRLC = :CODRLC)
                          AND CODCAT = :CODRLC
                          AND CODCLI = :CODCLI";
    }

    /// <summary>
    /// Atualiza o status do cliente.
    /// </summary>
    /// <returns>Query</returns>
    public static string UpdateClientStatus()
    {
        return @"UPDATE
                          MRT.CADCLIBTB
                     SET  TIPCLI = :TIPCLI
                     WHERE
                          CODCLI = :CODCLI";
    }

    /// <summary>
    /// Obtem a contagem de preferências.
    /// </summary>
    /// <returns>Query</returns>
    public static string GetPreferenceCount()
    {
        return "SELECT COUNT(1) FROM MRT.RLCPFRCATATI WHERE CODRLC = :CODRLC AND CODCNL = :CODCNL";
    }

    /// <summary>
    /// Obtem a contagem de preferências já selecionadas pelo cliente.
    /// </summary>
    /// <param name="codRlc">Código da relação.</param>
    /// <returns>Query</returns>
    public static string GetClientPreferenceCount(long? codRlc)
    {
        return @$"SELECT
                          COUNT(1)
                      FROM
                          MRT.RLCPFRCATCLI
                      WHERE
                          CODCLI     = :CODCLI
                          {(codRlc.HasValue ? "AND CODCAT = :CODRLC" : string.Empty)}";
    }

}
