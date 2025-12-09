#nullable enable
using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Catalogo.Service.Api;

public enum EfacilProductOrigin { Seller3P, Kit1P, Fracionado1P }

public class EfacilSkuParser
{
    public const ushort CodigoClienteKitEfacil1P = 1;
    public const ushort CodigoClienteFracionadoEfacil1P = 2;
    private static readonly Regex Sku3PRegex = new(@"[Pp]?(.+)-(\d)");
    private static readonly Regex Sku1PRegex = new(@"[Pp]?(\d+)");

    public long CODCLI { get; private set; }
    public string? ErpCode { get; private set; }
    public decimal? CodMer { get; private set; }
    public EfacilProductOrigin Origem { get; private set; }
    public string ValidSku { get; private set; } = null!;

    private EfacilSkuParser() { }

    public static EfacilSkuParser? Parse(string sku)
    {
        if (string.IsNullOrEmpty(sku)) throw new ArgumentNullException(nameof(sku));

        Match match1P = Sku1PRegex.Match(sku);
        Match match3P = Sku3PRegex.Match(sku);

        if (!match1P.Success && !match3P.Success) return null;

        EfacilSkuParser parsed = new();

        if (match3P.Success)
        {
            parsed.Origem = EfacilProductOrigin.Seller3P;

            long codCli = long.Parse(sku.Split('-').Last());
            string erpCode = match3P.Groups[1].Value;

            parsed.CODCLI = codCli;
            parsed.ErpCode = erpCode;
            parsed.ValidSku = $"{erpCode}-{codCli:D5}";
        }
        else if (match1P.Success)
        {
            decimal codMer = decimal.Parse(match1P.Groups[1].Value);
            parsed.CodMer = codMer;
            parsed.ValidSku = codMer.ToString(CultureInfo.InvariantCulture);

            parsed.Origem = codMer > 2_000_000_000 ? EfacilProductOrigin.Kit1P : EfacilProductOrigin.Fracionado1P;
            parsed.CODCLI = parsed.Origem switch
            {
                EfacilProductOrigin.Fracionado1P => CodigoClienteFracionadoEfacil1P,
                EfacilProductOrigin.Kit1P => CodigoClienteKitEfacil1P,
                _ => throw new NotImplementedException("Tradução Origem x Código Cliente não implementada")
            };
        }

        return parsed;
    }
}
