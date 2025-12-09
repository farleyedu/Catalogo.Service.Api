namespace Catalogo.Service.Api;

/// <summary>
/// Camada de query's - Produto.
/// </summary>
public class ProductSQL
{
    /// <summary>
    /// Obtem o detalhe do produto martins.
    /// </summary>
    /// <returns>Query.</returns>
    public static string GetProductDetailB2B1P()
    {
        return @"WITH UF_REG AS
                      (
                           SELECT
                                S.CODESTUNIFATSRRBTB AS UF,
                                R.CODDIVREG          AS REGIAO
                           FROM
                                MRT.CADPRDLJACTLSMA LJA
                                JOIN
                                     MRT.CADPRDCTLSMA CTL
                                     ON
                                          CTL.CODBRRUNDVNDCSM = LJA.CODBRRUNDVNDCSM
                                          AND CTL.CODMERMRT   = :CODMER
                                JOIN
                                     MRT.CADEMPSRR S
                                     ON
                                          S.NUMIDTEMPSRRAPIMKP = LJA.CODCLI
                                LEFT JOIN
                                     MRT.T0100213 R
                                     ON
                                          R.CODESTUNI = S.CODESTUNIFATSRRBTB
                           WHERE
                                S.CODESTUNIFATSRRBTB IS NOT NULL
                      )
                 SELECT
                      PRD.CODMERMRT CODMER                                                  ,
                      ETN.DESFCATCNMERCMCETN                                                ,
                      TRIM(COALESCE(SEO.DESTITPRDMRT, PRD.DESPRDPAD, PRD.DESPRDRDC)) DESMER ,
                      PRD.CODBRRUNDVNDCSM                                                   ,
                      'MARTINS' NOMSRR                                                      ,
                      PRD.DESAPRMERMKTRVONVO                                                ,
                      PRD.CODPRD                                                            ,
                      PRD.CODGRPMERNCM                                                      ,
                      MRC.DESMRCCTL                                                         ,
                      PRD.CODSEC       AS CODSECAO                                                ,
                      TRIM(SEC.NOMSEC) AS SECAO                                                   ,
                      (
                           SELECT
                                LISTAGG(UF, ',') WITHIN GROUP (ORDER BY UF)
                           FROM
                                UF_REG
                      )
                      AS CODESTUNIFAT,
                      (
                           SELECT
                                LISTAGG(REGIAO, ',') WITHIN GROUP (ORDER BY REGIAO)
                           FROM
                                UF_REG
                      )
                      AS SIGLA_REGIAO_FATURAMENTO                       ,
                      TIP.TIPO_PRODUTO                                  ,
                      TIP.VALOR_TIPO_PRODUTO                            ,
                      MER.QDEUNDVNDCXAFRN AS QDE_UND_VENDA_CX_FORNECEDOR,
                      MER.QDEUNDVNDCXAKIT AS QDE_UND_VENDA_CX_KIT       ,
                      CASE
                           WHEN SEOMKT.DESURLLNK IS NOT NULL
                                THEN TRIM(SEOMKT.DESURLLNK) || '-' || TRIM(SEOMKT.CODMERDTB)
                           WHEN SEO.DESURLLNK IS NOT NULL
                                THEN TRIM(SEO.DESURLLNK) || '-martins_' || TRIM(PRD.CODMERMRT)
                                ELSE ''
                      END DESURLLNK                ,
                      MER.CODGRPMER    GRUPO       ,
                      GRP.DESGRPMER                ,
                      MER.CODFMLMER    CATEGORIA   ,
                      CAT.DESFMLMER                ,
                      MER.CODCLSMER    SUBCATEGORIA,
                      SUB.DESCLSMER                ,
                      MER.CODFRNPCPMER CODFRN      ,
                      FRN.NOMFRN                   ,
                      MRC.CODMRCMER                ,
                      TRIM(INITCAP(MRC.DESMRCMER)) DESMRCMER
                 FROM
                      MRT.CADPRDCTLSMA PRD
                      LEFT JOIN
                           MRT.CADINFMERCMCETN ETN
                           ON
                                ETN.CODMER = PRD.CODMERMRT
                      LEFT JOIN
                           MRT.RLCPRDSEOCTLSMA SEO
                           ON
                                SEO.CODPRD        = PRD.CODPRD
                                AND SEO.CODCNLNGC = 3
                      LEFT JOIN
                           MRT.RLCPRDSEOCTLMKT SEOMKT
                           ON
                                SEOMKT.CODORIMER  = 4
                                AND SEOMKT.CODPRD = PRD.CODMERMRT
                      LEFT JOIN
                           MRT.CADMRCPRDCTLSMA MRC
                           ON
                                MRC.CODMRCCTL = PRD.CODMRCCTL
                      LEFT JOIN
                           MRT.T0100086 MER
                           ON
                                MER.CODMER     = PRD.CODMERMRT
                                AND MER.CODEMP = 1
                      LEFT JOIN
                           MRT.CADSECCLTCSMSMA SEC
                           ON
                                SEC.CODCNLNGC  = 3
                                AND SEC.CODSEC = PRD.CODSEC
                      -- TIPO
                      LEFT JOIN
                           (
                                SELECT
                                     TIP.CODPRD                ,
                                     TIP.CODCNLNGC             ,
                                     ATR.DESATRPRD TIPO_PRODUTO,
                                     RAT.DESOPCATR VALOR_TIPO_PRODUTO
                                FROM
                                     MRT.RLCATRPRDCTLSMA TIP
                                     JOIN
                                          MRT.CADATRCTLSMA ATR
                                          ON
                                               ATR.CODATRPRD        = TIP.CODATRPRD
                                               AND ATR.DATDST IS NULL
                                     JOIN
                                          MRT.RLCOPCATRCTLSMA RAT
                                          ON
                                               RAT.CODATRPRD        = TIP.CODATRPRD
                                               AND RAT.CODOPCATR    = TIP.CODOPCATR
                                               AND RAT.DATDST IS NULL
                           )
                           TIP
                           ON
                                TIP.CODPRD        = PRD.CODPRD
                                AND TIP.CODCNLNGC = 1
                                AND ROWNUM        = 1
                      -- FIM TIPO
                      -- Categoria
                      LEFT JOIN
                           MRT.T0100426 FRN
                           ON
                                MER.CODFRNPCPMER = FRN.CODFRN
                      LEFT JOIN
                           MRT.T0100167 GRP
                           ON
                                MER.CODGRPMER = GRP.CODGRPMER
                      LEFT JOIN
                           MRT.T0100159 CAT
                           ON
                                MER.CODGRPMER     = CAT.CODGRPMER
                                AND MER.CODFMLMER = CAT.CODFMLMER
                      LEFT JOIN
                           MRT.T0100132 SUB
                           ON
                                MER.CODGRPMER     = SUB.CODGRPMER
                                AND MER.CODFMLMER = SUB.CODFMLMER
                                AND MER.CODCLSMER = SUB.CODCLSMER
                      LEFT JOIN
                           MRT.CADMRCMER MRC
                           ON
                                FRN.CODGRPFRN = MRC.CODGRPFRN
                      -- Fim Categoria
                 WHERE
                      PRD.CODMERMRT = :CODMER
                        ";
    }

    /// <summary>
    /// Obtem o detalhe do produto marketplace.
    /// </summary>
    /// <returns>Query.</returns>
    public static string GetProductDetailB2B3P()
    {
        return @"WITH PRODUTOLOJA AS
                      (
                           SELECT
                                LJA.CODCLI            ,
                                LJA.CODPRDLJA         ,
                                PRD.CODPRD            ,
                                PRD.DESPRDRDC         ,
                                PRD.DESPRDPAD         ,
                                LJA.DESPRDLJA         ,
                                PRD.CODBRRUNDVNDCSM   ,
                                PRD.DESAPRMERMKTRVONVO,
                                MRC.DESMRCCTL         ,
                                ROW_NUMBER() OVER (PARTITION BY LJA.CODCLI, LJA.CODPRDLJA ORDER BY
                                                   LJA.DATATUPCO DESC) AS RN
                           FROM
                                MRT.CADPRDLJACTLSMA LJA
                                JOIN
                                     MRT.CADPRDCTLSMA PRD
                                     ON
                                          PRD.CODPRD = LJA.CODPRD
                                LEFT JOIN
                                     MRT.CADMRCPRDCTLSMA MRC
                                     ON
                                          MRC.CODMRCCTL = PRD.CODMRCCTL
                           WHERE
                                LJA.CODTIPPRD            = 2
                                AND LJA.CODPRD IS NOT NULL
                      )
                 SELECT
                      COALESCE(SRR.CODESTUNIFATSRRBTB,' ') CODESTUNIFAT                                       ,
                      MER.CODGRPMERNCM                                                                        ,
                      TRIM(COALESCE(SEOSMA.DESTITPRDMRT, PRD.DESPRDRDC, PRD.DESPRDPAD, PRD.DESPRDLJA)) DESMER ,
                      PRD.CODBRRUNDVNDCSM                                                                     ,
                      SRR.DESEMPSRR             NOMSRR                                                                    ,
                      MER.CODMERSRR             CODMER                                                                    ,
                      NVL(MER.DESMERMKPBTB,' ') DESFCATCNMERCMCETN                                                        , -- DESFCATCNPRD
                      PRD.DESAPRMERMKTRVONVO                                                                              ,
                      PRD.CODPRD                                                                                          ,
                      PRD.CODCLI CODSLR                                                                                   ,
                      PRD.DESMRCCTL                                                                                       ,
                      CASE
                           WHEN SEO.DESURLLNK IS NOT NULL
                                THEN TRIM(SEO.DESURLLNK) || '-' || TRIM(SEO.CODMERDTB)
                                ELSE ''
                      END DESURLLNK
                 FROM
                      MRT.CADEMPSRR SRR
                      JOIN
                           MRT.CADPRDCTLOBTAPIMKP MER
                           ON
                                SRR.NUMIDTEMPSRRAPIMKP = MER.NUMIDTEMPSRRAPIMKP
                      -- Produto Catalogo
                      JOIN
                           PRODUTOLOJA PRD
                           ON
                                PRD.CODCLI                   = TO_NUMBER(MER.NUMIDTEMPSRRAPIMKP)
                                AND TO_NUMBER(PRD.CODPRDLJA) = TO_NUMBER(MER.NUMIDTMERAPIMKP)
                                AND PRD.RN                   = 1
                      -- Fim Produto Catalogo
                      -- Título Produto SEO
                      LEFT JOIN
                           MRT.RLCPRDSEOCTLSMA SEOSMA
                           ON
                                SEOSMA.CODPRD        = PRD.CODPRD
                                AND SEOSMA.CODCNLNGC = 3 -- Fixo B2B
                      -- Fim Título Produto SEO
                      -- URL marketplace (forma não otimizada)
                      LEFT JOIN
                           (
                                SELECT
                                     X.CODMERDTB ,
                                     X.DESURLLNK ,
                                     ROW_NUMBER() OVER (PARTITION BY X.CODMERDTB ORDER BY
                                                        X.DATALT DESC) AS RN
                                FROM
                                     MRT.RLCPRDSEOCTLMKT X
                                WHERE
                                     X.CODORIMER = 5
                           )
                           SEO
                           ON
                                SEO.CODMERDTB = MER.CODMERSRR
                                AND SEO.RN    = 1
                 WHERE
                      SRR.DESABVNOMSRR  = :DESABVNOMSRR COLLATE BINARY_CI
                      AND MER.CODMERSRR = :CODMERSRR    COLLATE BINARY_CI";
    }

    /// <summary>
    /// Obtêm o detalhe de produto e-Fácil 3P 
    /// </summary>
    /// <returns>Query.</returns>
    /// Douglas Antunes @ByteOn
    public static string GetProductDetailEfacil3P()
    {
        return @"SELECT
                NULL CODESTUNIFAT
                , CTL.CODGRPMERNCM
                , TRIM(COALESCE(SEOSMA.DESTITPRDMRT, CTL.DESPRDPAD, LJA.DESPRDLJA)) DESMER
                , CTL.CODBRRUNDVNDCSM
                , SLR.NOMDTBEXBCMCETN AS NOMSRR
                , RTRIM(LJA.CODERPLJA) || '-' || LPAD(LJA.CODCLI, 5, '0') AS CODMER
                , NVL(CTL.DESFCATCNPRD, ' ') AS DESFCATCNMERCMCETN -- DESFCATCNPRD
                , CTL.DESAPRMERMKTRVONVO
                , CTL.CODPRD
                , LJA.CODCLI AS CODSLR
                , MRC.DESMRCCTL
                , CASE
                   WHEN SEOSMA.DESURLLNK IS NOT NULL
                        THEN TRIM(SEOSMA.DESURLLNK) || '-' || REPLACE(RTRIM(LJA.CODERPLJA), '.', '') || '-' || LPAD(LJA.CODCLI, 5, '0')
                        ELSE ''
                END AS DESURLLNK
            FROM
                MRT.CADPRDLJACTLSMA LJA
            INNER JOIN MRT.CADDTBCMCETN SLR ON
                SLR.CODDTBCMCETN = LJA.CODCLI
            -- Produto Catalogo
            INNER JOIN MRT.CADPRDCTLSMA CTL ON
                CTL.CODPRD = LJA.CODPRD
            INNER JOIN MRT.CADMRCPRDCTLSMA MRC ON
                MRC.CODMRCCTL = CTL.CODMRCCTL
            INNER JOIN MRT.RLCPRDSEOCTLSMA SEOSMA ON
                SEOSMA.CODPRD = LJA.CODPRD
                AND SEOSMA.CODCNLNGC = 2
            WHERE
                LJA.CODTIPPRD = 3
                AND LJA.CODCLI  = :CODCLI
                AND LJA.CODERPLJA LIKE :CODERPLJA";
    }

    /// <summary>
    /// Obtêm o detalhe de produto e-Fácil 1P 
    /// </summary>
    /// <returns>Query.</returns>
    /// Douglas Antunes @ByteOn
    public static string GetProductDetailEfacil1P()
    {
        return @"SELECT
                NULL CODESTUNIFAT
                , CTL.CODGRPMERNCM
                , TRIM(COALESCE(SEOSMA.DESTITPRDMRT, CTL.DESPRDPAD, LJA.DESPRDLJA)) DESMER
                , CTL.CODBRRUNDVNDCSM
                , 'e-Fácil' AS NOMSRR
                , RTRIM(LJA.CODPRDLJA) AS CODMER
                , NVL(CTL.DESFCATCNPRD, ' ') AS DESFCATCNMERCMCETN -- DESFCATCNPRD
                , CTL.DESAPRMERMKTRVONVO
                , CTL.CODPRD
                , LJA.CODCLI AS CODSLR
                , MRC.DESMRCCTL
                , CASE
                   WHEN SEOSMA.DESURLLNK IS NOT NULL
                        THEN TRIM(SEOSMA.DESURLLNK) || '-' || RTRIM(LJA.CODPRDLJA)
                        ELSE ''
                END AS DESURLLNK
                , RTRIM(CTL.DESDIVCMP) AS DESDIVCMP
                , CTL.CODDRT
                , TRIM(DESDRTCMP) AS DESDRT
            FROM
                MRT.CADPRDLJACTLSMA LJA
            -- Produto Catalogo
            INNER JOIN MRT.CADPRDCTLSMA CTL ON
                CTL.CODPRD = LJA.CODPRD
            INNER JOIN MRT.CADMRCPRDCTLSMA MRC ON
                MRC.CODMRCCTL = CTL.CODMRCCTL
            LEFT JOIN MRT.T0123183 DRT ON
                DRT.CODDRTCMP = CTL.CODDRT
            INNER JOIN MRT.RLCPRDSEOCTLSMA SEOSMA ON
                SEOSMA.CODPRD = LJA.CODPRD
                AND SEOSMA.CODCNLNGC = 2
            WHERE
                LJA.CODTIPPRD = 4
                AND LJA.CODCLI  = :CODCLI
                AND LJA.CODPRDLJA = :CODPRDLJA";
    }

    /// <summary>
    /// Obtem as imagens do produto martins.
    /// </summary>
    /// <returns>Query.</returns>
    public static string GetProductImageB2B1P()
    {
        return @"SELECT
                31                                                                                                                                                                               AS CODE_SLR   , -- 3P Cod Seller
                ''                                                                                                                                                                               AS CODERP_SLR , -- cód erp produto seller
                'martins_' || CTL.CODMERMRT                                                                                                                                                      AS SKU_3P     ,
                CTL.CODMERMRT                                                                                                                                                                    AS SKU_1P     ,
                CTL.CODPRD                                                                                                                                                                       AS CODPRDCTL  ,
                'https://imgprd.martinsatacado.com.br/catalogoimg/' || CTL.CODPRD || '/' || IMGCTL.NOMIMG || '?v=' || TO_CHAR(NVL(IMGCTL.DATALT, IMGCTL.DATCAD), 'ddMMyyyyMISS') || '&ims=1000x' AS IMGLNK     ,
                IMGCTL.CODTIPIMG                                                                                                                                                                               ,
                IMGCTL.NUMSEQIMG
            FROM
                MRT.CADPRDCTLSMA CTL
            -- IMG
            INNER JOIN MRT.RLCPRDIMGCTLSMA IMGCTL ON
                IMGCTL.CODPRD           = CTL.CODPRD
                AND IMGCTL.DATDST IS NULL
            WHERE
                CTL.CODPRD = :CODPRD";
    }

    /// <summary>
    /// Obtem as imagens do produto marketplace.
    /// </summary>
    /// <returns>Query.</returns>
    public static string GetProductImageB2B3P()
    {
        return @"SELECT
                CTL.CODMERMRT                                                                                                                                                                    AS SKU_1P    ,
                CTL.CODPRD                                                                                                                                                                       AS CODPRDCTL ,
                'https://imgprd.martinsatacado.com.br/catalogoimg/' || CTL.CODPRD || '/' || IMGCTL.NOMIMG || '?v=' || TO_CHAR(NVL(IMGCTL.DATALT, IMGCTL.DATCAD), 'ddMMyyyyMISS') || '&ims=1000x' AS IMGLNK    ,
                IMGCTL.CODTIPIMG                                                                                                                                                                              ,
                IMGCTL.NUMSEQIMG
            FROM
                MRT.CADPRDCTLSMA CTL
            -- IMG
            INNER JOIN MRT.RLCPRDIMGCTLSMA IMGCTL ON
                IMGCTL.CODPRD           = CTL.CODPRD
                AND IMGCTL.DATDST IS NULL
            WHERE
                CTL.DATDST IS NULL
                AND CTL.CODPRD   = :CODPRD";
    }

    /// <summary>
    /// Obtem os atributos do produto.
    /// </summary>
    /// <returns>Query.</returns>
    public static string GetProductAttributes()
    {
        return @"SELECT DISTINCT
                TRIM(ATR.DESATRPRD) NAME
                , TRIM(OPC.DESOPCATR) VALUE
                , ATR.INDATRDEF
                , ATR.CODATRPRDEXT
                , ATR.CODATRPRD
            FROM
                MRT.CADPRDCTLSMA PRD
            JOIN MRT.RLCPRDHIRCNLCTLMRT CNL ON
                CNL.CODPRD = PRD.CODPRD
            JOIN MRT.CADSUBCTGCLTCSMSMA SUBCTG ON
                SUBCTG.CODSUBCTGPRD = CNL.CODSUBCTGPRD
            JOIN MRT.RLCATRHIRCTLSMA ATRHIR ON
                ATRHIR.CODSEC = SUBCTG.CODSEC
                AND
                (
                    ATRHIR.CODCTG    = SUBCTG.CODCTG
                    OR ATRHIR.CODCTG = 0
                )
                AND
                (
                    ATRHIR.CODSUBCTGPRD    = SUBCTG.CODSUBCTGPRD
                    OR ATRHIR.CODSUBCTGPRD = 0
                )
            JOIN MRT.CADATRCTLSMA ATR ON
                ATR.CODATRPRD = ATRHIR.CODATRPRD
            JOIN MRT.RLCOPCATRCTLSMA OPC ON
                OPC.CODATRPRD = ATR.CODATRPRD
            JOIN MRT.RLCATRPRDCTLSMA ATRPRD ON
                ATRPRD.CODATRPRD            = ATR.CODATRPRD
                AND ATRPRD.CODOPCATR        = OPC.CODOPCATR
                AND ATRPRD.CODPRD           = PRD.CODPRD
                AND ATRPRD.CODPRD IS NOT NULL
            WHERE
                ATR.DATDST  IS NULL
                AND PRD.CODPRD = :CODPRD
                AND CNL.CODCNL = :CODCNL";
    }

    /// <summary>
    /// Obtem as informações do SEO Martins.
    /// </summary>
    /// <returns>Query.</returns>
    public static string GetInfoSEOB2B1P()
    {
        return @"SELECT
                PRD.DESTITPRD,   -- Título do produto
                SEO.DESURLLNK,   -- URL
                PRD.DESMDDPRD,   -- Metadescription
                PRD.DESTERCHV,   -- Keyword Manager
                PRD.DESTITIMGPRD -- Texto Alt. em Imagens
            FROM
                MRT.CADPRDCTLSMA PRD
            JOIN MRT.RLCPRDSEOCTLSMA SEO ON
                SEO.CODPRD        = PRD.CODPRD
                AND SEO.CODCNLNGC = 3 -- Fixo B2B
            WHERE
            PRD.CODMERMRT = :CODMER";
    }

    /// <summary>
    /// Obtem as informações do SEO Seller.
    /// </summary>
    /// <returns>Query.</returns>
    public static string GetInfoSEOB2B3P()
    {
        return @"WITH PRODUTOLOJA AS
            (
                SELECT
                    LJA.CODCLI      ,
                    LJA.CODPRDLJA   ,
                    PRD.DESTITPRD   , -- Título do produto
                    SEO.DESURLLNK   , -- URL
                    PRD.DESMDDPRD   , -- Metadescription
                    PRD.DESTERCHV   , -- Keyword Manager
                    PRD.DESTITIMGPRD, -- Texto Alt. em Imagens
                    ROW_NUMBER() OVER (PARTITION BY LJA.CODCLI, LJA.CODPRDLJA ORDER BY LJA.DATATUPCO DESC) AS RN
                FROM
                    MRT.CADPRDLJACTLSMA LJA
                JOIN MRT.CADPRDCTLSMA PRD ON
                    PRD.CODPRD = LJA.CODPRD
                JOIN MRT.RLCPRDSEOCTLSMA SEO ON
                    SEO.CODPRD        = PRD.CODPRD
                    AND SEO.CODCNLNGC = 3 -- Fixo B2B
                WHERE
                    LJA.CODTIPPRD            = 2
                    AND LJA.CODPRD IS NOT NULL
            )
            SELECT
                PRD.DESTITPRD,   -- Título do produto
                PRD.DESURLLNK,   -- URL
                PRD.DESMDDPRD,   -- Metadescription
                PRD.DESTERCHV,   -- Keyword Manager
                PRD.DESTITIMGPRD -- Texto Alt. em Imagens
            FROM
                MRT.CADEMPSRR SRR
            JOIN MRT.CADPRDCTLOBTAPIMKP MER ON
                SRR.NUMIDTEMPSRRAPIMKP = MER.NUMIDTEMPSRRAPIMKP
            -- Produto Catalogo
            JOIN PRODUTOLOJA PRD ON
                PRD.CODCLI                   = TO_NUMBER(MER.NUMIDTEMPSRRAPIMKP)
                AND TO_NUMBER(PRD.CODPRDLJA) = TO_NUMBER(MER.NUMIDTMERAPIMKP)
                AND PRD.RN                   = 1
            WHERE
                SRR.DESABVNOMSRR  = :DESABVNOMSRR COLLATE BINARY_CI
                AND MER.CODMERSRR = :CODMERSRR COLLATE BINARY_CI";
    }

    /// <summary>
    /// Obtém as informações do SEO e-Fácil 3P.
    /// </summary>
    /// <returns>Query.</returns>
    public static string GetInfoSEOEfacil3P()
    {
        return @"SELECT
                PRD.DESTITPRD,   -- Título do produto
                SEO.DESURLLNK,   -- URL
                PRD.DESMDDPRD,   -- Metadescription
                PRD.DESTERCHV,   -- Keyword Manager
                PRD.DESTITIMGPRD -- Texto Alt. em Imagens
            FROM
                MRT.CADPRDLJACTLSMA LJA
            INNER JOIN MRT.CADPRDCTLSMA PRD ON
                PRD.CODPRD = LJA.CODPRD
            INNER JOIN MRT.RLCPRDSEOCTLSMA SEO ON
                SEO.CODPRD        = LJA.CODPRD
                AND SEO.CODCNLNGC = 2 -- Fixo e-Fácil
            WHERE
                LJA.CODTIPPRD = 3
                AND LJA.CODCLI  = :CODCLI
                AND LJA.CODERPLJA = :CODERPLJA";
    }

    /// <summary>
    /// Obtém as informações do SEO e-Fácil 1P.
    /// </summary>
    /// <returns>Query.</returns>
    public static string GetInfoSEOEfacil1P()
    {
        return @"SELECT
                PRD.DESTITPRD,   -- Título do produto
                SEO.DESURLLNK,   -- URL
                PRD.DESMDDPRD,   -- Metadescription
                PRD.DESTERCHV,   -- Keyword Manager
                PRD.DESTITIMGPRD -- Texto Alt. em Imagens
            FROM
                MRT.CADPRDLJACTLSMA LJA
            INNER JOIN MRT.CADPRDCTLSMA PRD ON
                PRD.CODPRD = LJA.CODPRD
            INNER JOIN MRT.RLCPRDSEOCTLSMA SEO ON
                SEO.CODPRD        = LJA.CODPRD
                AND SEO.CODCNLNGC = 2 -- Fixo e-Fácil
            WHERE
                LJA.CODTIPPRD = 4
                AND LJA.CODCLI  = :CODCLI
                AND LJA.CODPRDLJA = :CODPRDLJA";
    }

    /// <summary>
    /// Obtem hierarquia do produto Martins.
    /// </summary>
    /// <returns>Query.</returns>
    public static string GetHierarchyProductB2B1P()
    {
        return @"SELECT
                CAT.CODCTG             ,
                TRIM(CAT.DESCTG) DESCTG,
                SUBCTG.CODSUBCTGPRD    ,
                TRIM(SUBCTG.DESSUBCTGPRD) DESSUBCTGPRD
            FROM
                MRT.CADPRDCTLSMA PRD
            JOIN MRT.RLCPRDHIRCNLCTLMRT HIE ON
                HIE.CODPRD = PRD.CODPRD
            JOIN MRT.CADSUBCTGCLTCSMSMA SUBCTG ON
                SUBCTG.CODSUBCTGPRD = HIE.CODSUBCTGPRD
                AND HIE.CODCNL      = 3 -- Fixo B2B
            JOIN MRT.CADSECCLTCSMSMA SEC ON
                SEC.CODSEC = SUBCTG.CODSEC
            JOIN MRT.CADCTGCLTCSMSMA CAT ON
                CAT.CODCTG = SUBCTG.CODCTG
            WHERE
                PRD.CODMERMRT = :CODMER";
    }

    /// <summary>
    /// Obtem hierarquia do produto Seller.
    /// </summary>
    /// <returns>Query.</returns>
    public static string GetHierarchyProductB2B3P()
    {
        return @"WITH PRODUTOLOJA AS
            (
                SELECT
                    LJA.CODCLI                            ,
                    LJA.CODPRDLJA                         ,
                    CAT.CODCTG                            ,
                    TRIM(CAT.DESCTG) DESCTG               ,
                    SUBCTG.CODSUBCTGPRD                   ,
                    TRIM(SUBCTG.DESSUBCTGPRD) DESSUBCTGPRD,
                    ROW_NUMBER() OVER (PARTITION BY LJA.CODCLI, LJA.CODPRDLJA ORDER BY LJA.DATATUPCO DESC) AS RN
                FROM
                    MRT.CADPRDLJACTLSMA LJA
                JOIN MRT.CADPRDCTLSMA PRD ON
                    PRD.CODPRD = LJA.CODPRD
                JOIN MRT.RLCPRDHIRCNLCTLMRT HIE ON
                    HIE.CODPRD     = PRD.CODPRD
                    AND HIE.CODCNL = 3 -- Fixo B2B
                JOIN MRT.CADSUBCTGCLTCSMSMA SUBCTG ON
                    SUBCTG.CODSUBCTGPRD = HIE.CODSUBCTGPRD
                JOIN MRT.CADSECCLTCSMSMA SEC ON
                    SEC.CODSEC = SUBCTG.CODSEC
                JOIN MRT.CADCTGCLTCSMSMA CAT ON
                    CAT.CODCTG = SUBCTG.CODCTG
                WHERE
                    LJA.CODTIPPRD            = 2
                    AND LJA.CODPRD IS NOT NULL
            )
            SELECT
                PRD.CODCTG,
                PRD.DESCTG,
                PRD.CODSUBCTGPRD,
                PRD.DESSUBCTGPRD
            FROM
                MRT.CADEMPSRR SRR
            JOIN MRT.CADPRDCTLOBTAPIMKP MER ON
                SRR.NUMIDTEMPSRRAPIMKP = MER.NUMIDTEMPSRRAPIMKP
            -- Produto Catalogo
            JOIN PRODUTOLOJA PRD ON
                PRD.CODCLI                   = TO_NUMBER(MER.NUMIDTEMPSRRAPIMKP)
                AND TO_NUMBER(PRD.CODPRDLJA) = TO_NUMBER(MER.NUMIDTMERAPIMKP)
                AND PRD.RN                   = 1
            WHERE
                SRR.DESABVNOMSRR  = :DESABVNOMSRR COLLATE BINARY_CI
                AND MER.CODMERSRR = :CODMERSRR COLLATE BINARY_CI";
    }

    /// <summary>
    /// Obtém hierarquia do produto e-Fácil 3P.
    /// </summary>
    /// <returns>Query.</returns>
    public static string GetHierarchyProductEfacil3P()
    {
        return @"SELECT
                CAT.CODCTG             ,
                TRIM(CAT.DESCTG) DESCTG,
                SUBCTG.CODSUBCTGPRD    ,
                TRIM(SUBCTG.DESSUBCTGPRD) DESSUBCTGPRD
            FROM
                MRT.CADPRDLJACTLSMA LJA
            INNER JOIN MRT.RLCPRDHIRCNLCTLMRT HIE ON
                HIE.CODPRD = LJA.CODPRD
                AND HIE.CODCNL = 2 -- Fixo e-Fácil
            INNER JOIN MRT.CADSUBCTGCLTCSMSMA SUBCTG ON
                SUBCTG.CODSUBCTGPRD = HIE.CODSUBCTGPRD
                AND SUBCTG.CODCNLNGC = HIE.CODCNL
            INNER JOIN MRT.CADSECCLTCSMSMA SEC ON
                SEC.CODSEC = SUBCTG.CODSEC
                AND SEC.CODCNLNGC = SUBCTG.CODCNLNGC
            INNER JOIN MRT.CADCTGCLTCSMSMA CAT ON
                CAT.CODCTG = SUBCTG.CODCTG
                AND CAT.CODCNLNGC = SUBCTG.CODCNLNGC
            WHERE
                LJA.CODTIPPRD = 3
                AND LJA.CODCLI  = :CODCLI
                AND LJA.CODERPLJA = :CODERPLJA";
    }

    /// <summary>
    /// Obtém hierarquia do produto e-Fácil 1P.
    /// </summary>
    /// <returns>Query.</returns>
    public static string GetHierarchyProductEfacil1P()
    {
        return @"SELECT
                CAT.CODCTG             ,
                TRIM(CAT.DESCTG) DESCTG,
                SUBCTG.CODSUBCTGPRD    ,
                TRIM(SUBCTG.DESSUBCTGPRD) DESSUBCTGPRD
            FROM
                MRT.CADPRDLJACTLSMA LJA
            INNER JOIN MRT.RLCPRDHIRCNLCTLMRT HIE ON
                HIE.CODPRD = LJA.CODPRD
                AND HIE.CODCNL = 2 -- Fixo e-Fácil
            INNER JOIN MRT.CADSUBCTGCLTCSMSMA SUBCTG ON
                SUBCTG.CODSUBCTGPRD = HIE.CODSUBCTGPRD
                AND SUBCTG.CODCNLNGC = HIE.CODCNL
            INNER JOIN MRT.CADSECCLTCSMSMA SEC ON
                SEC.CODSEC = SUBCTG.CODSEC
                AND SEC.CODCNLNGC = SUBCTG.CODCNLNGC
            INNER JOIN MRT.CADCTGCLTCSMSMA CAT ON
                CAT.CODCTG = SUBCTG.CODCTG
                AND CAT.CODCNLNGC = SUBCTG.CODCNLNGC
            WHERE
                LJA.CODTIPPRD = 4
                AND LJA.CODCLI  = :CODCLI
                AND LJA.CODPRDLJA = :CODPRDLJA";
    }

    /// <summary>
    /// Obtem a imagem única do produto martins.
    /// </summary>
    /// <returns>Query.</returns>
    public static string GetProductImageB2B1PUnique()
    {
        return @"SELECT
                'martins_' || PRD.CODMERMRT AS SKU_3P ,
                'https://imgprd.martinsatacado.com.br/catalogoimg/' || CTL.CODPRD || '/' || IMGCTL.NOMIMG || '?v=' || TO_CHAR(NVL(IMGCTL.DATALT, IMGCTL.DATCAD), 'ddMMyyyyMISS') || '&ims=1000x' AS IMGLNK ,
                IMGCTL.CODTIPIMG
            FROM
                MRT.CADPRDCTLSMA CTL
            -- IMG
            JOIN MRT.RLCPRDIMGCTLSMA IMGCTL ON
                IMGCTL.CODPRD = CTL.CODPRD
                AND     IMGCTL.DATDST IS NULL
            JOIN MRT.CADPRDCTLSMA PRD ON
                PRD.CODPRD = CTL.CODPRD
            WHERE
                PRD.CODMERMRT    = :CODMER
                AND     IMGCTL.CODTIPIMG = 1
            ORDER BY
                IMGCTL.CODTIPIMG
            FETCH   NEXT 1 ROWS ONLY";
    }

    /// <summary>
    /// Obtem a imagem única do produto marketplace.
    /// </summary>
    /// <returns>Query.</returns>
    public static string GetProductImageB2B3PUnique()
    {
        return @"SELECT
                MER.CODMERSRR AS SKU_3P ,
                'https://imgprd.martinsatacado.com.br/catalogoimg/' || CTL.CODPRD || '/' || IMGCTL.NOMIMG || '?v=' || TO_CHAR(NVL(IMGCTL.DATALT, IMGCTL.DATCAD), 'ddMMyyyyMISS') || '&ims=1000x' AS IMGLNK ,
                IMGCTL.CODTIPIMG
            FROM
                MRT.CADPRDCTLSMA CTL
            -- Imagem
            JOIN MRT.RLCPRDIMGCTLSMA IMGCTL ON
                IMGCTL.CODPRD = CTL.CODPRD
                AND     IMGCTL.DATDST IS NULL
            JOIN MRT.CADPRDLJACTLSMA LJA ON
                LJA.CODPRD    = IMGCTL.CODPRD
                AND     LJA.CODTIPPRD = 2
                AND     LJA.CODPRD IS NOT NULL
            JOIN MRT.CADPRDCTLOBTAPIMKP MER ON
                TO_NUMBER(MER.NUMIDTEMPSRRAPIMKP) = LJA.CODCLI
                AND     TO_NUMBER(MER.NUMIDTMERAPIMKP)    = TO_NUMBER(LJA.CODPRDLJA)
            JOIN MRT.CADEMPSRR SRR ON
                SRR.NUMIDTEMPSRRAPIMKP = MER.NUMIDTEMPSRRAPIMKP
            WHERE
                CTL.DATDST IS NULL
                AND     SRR.DESABVNOMSRR = :DESABVNOMSRR COLLATE BINARY_CI
                AND     MER.CODMERSRR    = :CODMERSRR    COLLATE BINARY_CI
                AND     IMGCTL.CODTIPIMG = 1
            ORDER BY
                IMGCTL.CODTIPIMG
            FETCH   NEXT 1 ROWS ONLY";
    }

    /// <summary>
    /// Obtem a imagem única do produto e-Fácil 3P.
    /// </summary>
    /// <returns>Query.</returns>
    public static string GetProductImageEfacil3PUnique()
    {
        return @"SELECT
                REPLACE(RTRIM(LJA.CODERPLJA), '.', '') || '-' || LPAD(LJA.CODCLI, 5, '0') AS SKU_3P ,
                'https://imgprd.martinsatacado.com.br/catalogoimg/' || LJA.CODPRD || '/' || IMGCTL.NOMIMG || '?v=' || TO_CHAR(NVL(IMGCTL.DATALT, IMGCTL.DATCAD), 'ddMMyyyyMISS') || '&ims=1000x' AS IMGLNK ,
                IMGCTL.CODTIPIMG
            FROM
                MRT.CADPRDLJACTLSMA LJA
            INNER JOIN MRT.CADPRDCTLSMA CTL ON
                CTL.CODPRD = LJA.CODPRD
                AND CTL.DATDST IS NULL
            -- Imagem
            INNER JOIN MRT.RLCPRDIMGCTLSMA IMGCTL ON
                IMGCTL.CODPRD = LJA.CODPRD
                AND IMGCTL.DATDST IS NULL
                AND IMGCTL.CODTIPIMG = 1
            WHERE
                LJA.CODTIPPRD = 3
                AND LJA.CODCLI  = :CODCLI
                AND LJA.CODERPLJA = :CODERPLJA
            FETCH NEXT 1 ROWS ONLY";
    }

    /// <summary>
    /// Obtem a imagem única do produto e-Fácil 1P.
    /// </summary>
    /// <returns>Query.</returns>
    public static string GetProductImageEfacil1PUnique()
    {
        return @"SELECT
                RTRIM(LJA.CODPRDLJA) AS SKU_3P ,
                'https://imgprd.martinsatacado.com.br/catalogoimg/' || LJA.CODPRD || '/' || IMGCTL.NOMIMG || '?v=' || TO_CHAR(NVL(IMGCTL.DATALT, IMGCTL.DATCAD), 'ddMMyyyyMISS') || '&ims=1000x' AS IMGLNK ,
                IMGCTL.CODTIPIMG
            FROM
                MRT.CADPRDLJACTLSMA LJA
            INNER JOIN MRT.CADPRDCTLSMA CTL ON
                CTL.CODPRD = LJA.CODPRD
                AND CTL.DATDST IS NULL
            -- Imagem
            INNER JOIN MRT.RLCPRDIMGCTLSMA IMGCTL ON
                IMGCTL.CODPRD = LJA.CODPRD
                AND IMGCTL.DATDST IS NULL
                AND IMGCTL.CODTIPIMG = 1
            WHERE
                LJA.CODTIPPRD = 4
                AND LJA.CODCLI  = :CODCLI
                AND LJA.CODPRDLJA = :CODPRDLJA
            FETCH NEXT 1 ROWS ONLY";
    }

    /// <summary>
    /// Obtêm dados de mercadorias 1P e 3P DIVINO
    /// </summary>
    /// <returns></returns>
    public static string GetMercadoria1P_3P()
    {
        return @"WITH PRSPRD_CLEAN AS (
                SELECT
                    CODMER_LIMPO,
                    DATPCS
                FROM (
                    SELECT
                        TO_NUMBER(REGEXP_REPLACE(CODMERSRR, '[^0-9]', '')) AS CODMER_LIMPO,
                        DATPCS,
                        ROW_NUMBER() OVER (
                            PARTITION BY TO_NUMBER(REGEXP_REPLACE(CODMERSRR, '[^0-9]', ''))
                            ORDER BY CODPRD DESC
                        ) AS RN
                    FROM MRT.RLCPRSPRD
                )
                WHERE RN = 1
            )
            SELECT *
            FROM (
                SELECT TABELA.*, ROWNUM AS LINHA
                FROM (
                    SELECT
                        '1P' AS SELLER,
                        'martins_' || CTL.CODMERMRT AS SKU,
                        CTL.CODMERMRT AS SKUMRT,
                        CTL.DATCAD,
                        CTL.DATALT,
                        P.DATPCS
                    FROM MRT.CADPRDCTLSMA CTL

                    JOIN MRT.T0100086 MER
                        ON MER.CODMER = CTL.CODMERMRT
                    AND MER.CODBRRUNDVNDMRT = CTL.CODBRRUNDVNDCSM

                    JOIN MRT.RLCPRDHIRCNLCTLMRT HIE
                        ON HIE.CODPRD = CTL.CODPRD
                    AND HIE.CODCNL = 3

                    JOIN MRT.CADSUBCTGCLTCSMSMA SUB
                        ON SUB.CODSUBCTGPRD = HIE.CODSUBCTGPRD

                    JOIN MRT.CADSECCLTCSMSMA SEC
                        ON SEC.CODSEC = SUB.CODSEC

                    JOIN MRT.CADCTGCLTCSMSMA CAT
                        ON CAT.CODCTG = SUB.CODCTG

                    JOIN MRT.CADMRCPRDCTLSMA MRC
                        ON MRC.CODMRCCTL = CTL.CODMRCCTL

                    LEFT JOIN MRT.RLCPRDSEOCTLSMA SEO
                        ON SEO.CODPRD = CTL.CODPRD
                    AND SEO.CODCNLNGC = 3

                    LEFT JOIN PRSPRD_CLEAN P
                        ON P.CODMER_LIMPO = CTL.CODMERMRT

                    WHERE EXISTS (
                        SELECT 1
                        FROM MRT.T0201350 X
                        JOIN MRT.T0111770 Y
                        ON Y.CODMER = X.CODMER
                        AND X.CODFILEMP = Y.CODFILEMP
                        WHERE X.CODMER = MER.CODMER
                        AND Y.DATDSTMER IS NULL
                    )

                    AND (
                            CTL.DATCAD  >= P.DATPCS
                        OR CTL.DATALT >= P.DATPCS
                        OR P.DATPCS IS NULL
                    )

                    ORDER BY CTL.CODMERMRT DESC
                ) TABELA
                WHERE ROWNUM <= :NUMFIMITE
            )
            WHERE LINHA > :NUMINIITE
";

    }

    /// <summary>
    /// Obtêm dados de mercadorias E FILIAIS 1P e 3P DIVINO
    /// </summary>
    /// <returns></returns>
    public static string GetMercadoria_Filiais_3P()
    {
        return @"SELECT 
                           TO_NUMBER(PRD.NUMIDTMERAPIMKP)               AS CODIGOMERCADORIA
                         , LISTAGG(DISTINCT WAREHOUSES.CODESTUNI, '|')  AS CODIGOFILAISMERCADORIA
                    FROM MRT.MOVENVPCLPRDPSQ MOV
                        INNER JOIN MRT.CADENVPRDPSQ ENV ON ENV.CODPLT = MOV.CODPLT AND ENV.CODTIPPRD = 2
                        INNER JOIN MRT.CADPRDCTLOBTAPIMKP PRD ON PRD.NUMIDTEMPSRRAPIMKP = ENV.NUMIDTEMPSRRAPIMKP AND PRD.NUMIDTMERAPIMKP = TRIM(ENV.CODPRDLJA) AND PRD.DATDST IS NULL
                        INNER JOIN MRT.CADETQPRDCTLAPIMKP ETQ ON PRD.NUMIDTEMPSRRAPIMKP = ETQ.NUMIDTEMPSRRAPIMKP AND PRD.NUMIDTMERAPIMKP = ETQ.NUMIDTMERAPIMKP AND ETQ.QDEETQMER > 0 -- há estoque AND (ETQ.DATFIMETQBTB < SYSDATE - 3 OR ETQ.DATFIMETQBTB IS NULL) -- Passou 72 horas ou null
                        INNER JOIN MRT.RLCSRRATDCEP WAREHOUSES ON WAREHOUSES.NUMIDTEMPSRRAPIMKP = ENV.NUMIDTEMPSRRAPIMKP
                    WHERE 1=1
                        --AND upper(prd.codmersrr) like upper('%BRISADOCE_2744%')
                        AND upper(prd.codmersrr) like upper(:CODMERSRR)
                    GROUP BY PRD.NUMIDTMERAPIMKP

                ";
    }
    /// <summary>
    /// Obtêm dados de mercadorias E FILIAIS 1P e 3P DIVINO
    /// </summary>
    /// <returns></returns>
    public static string GetMercadoria_Filiais_1P()
    {
        return @"SELECT 
                         mer.CODMER                             AS CODIGOMERCADORIA
                        ,LISTAGG(DISTINCT etq.CODFILEMP, '|')   AS CODIGOFILAISMERCADORIA
                    FROM MRT.T0100086 mer
                    INNER JOIN MRT.t0201350 etq ON etq.CODEMP=1 AND etq.CODCNLDTB=0 AND etq.CODMER=mer.CODMER
                    INNER JOIN MRT.t0111770 det ON det.CODEMP=1 AND det.CODFILEMP=etq.CODFILEMP AND det.CODMER=mer.CODMER
                    INNER JOIN MRT.t0112963 fil ON fil.CODEMP=1 AND fil.CODFILEMP=etq.CODFILEMP AND fil.FLGFILEMPEPD='*'
                    WHERE 1 = 1 
                    --AND mer.CODMER=100490
                        AND mer.CODMER=:CODMER 
                    GROUP BY mer.CODMER
                ";
    }

    /// <summary>
    /// Salva o protocolo do produto.
    /// </summary>
    /// <returns>Query.</returns>
    public static string InsertProtocolo()
    {
        return @"INSERT INTO MRT.RLCPRSPRD
                      (
                           CODPRD         , -- PK
                           CODSKUHSH      , -- HASH
                           CODMERSRR      , -- SKU
                           CODBRRUNDVNDCSM, -- EAN
                           DATPCS         , -- DATA DO PROCESSO
                           DATCAD           -- DATA CADASTRO
                      )
                      VALUES
                      (
                           (SELECT NVL(MAX(CODPRD), 0) + 1 FROM MRT.RLCPRSPRD),
                           :CODSKUHSH      ,
                           :CODMERSRR      ,
                           :CODBRRUNDVNDCSM,
                           :DATPCS         ,
                           SYSDATE
                      )";
    }
}
