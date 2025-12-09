using System.IO;
using System.Xml;

namespace Catalogo.Service.Api;

/// <summary>
/// Constantes padrão do sistema.
/// </summary>
/// <remarks>Leon Denis [ByteOn]</remarks>
public static class Constants
{
    #region Conexão

    /// <summary>
    /// Conexão com o banco de dados.
    /// </summary>
    public static string Connection { get; private set; }

    /// <summary>
    /// Seta o caminho do arquivo de conexão.
    /// </summary>
    public static string ConnectionFilePath
    {
        set
        {
            try
            {
                // Verificando se é um arquivo.
                if (File.Exists(value))
                {
                    XmlDocument xml = new();
                    xml.Load(value);
                    XmlNode node = xml.DocumentElement.SelectSingleNode("connectionStrings/add[@name='MRT001']");
                    Connection = node.Attributes["value"].Value;
                }
                else
                    Connection = value;
            }
            catch
            {
                Connection = string.Empty;
            }
        }
    }

    #endregion Conexão

    #region Cache

    /// <summary>
    /// Cache da aplicação habilitado?
    /// </summary>
    public static bool IsApplicationCacheEnabled { get; set; }

    /// <summary>
    /// Tempo de expiração do cache ultimo click.
    /// </summary>
    public static int ClickExpirationTime { get; set; }

    /// <summary>
    /// Tempo de expiração total.
    /// </summary>
    public static int AbsoluteExpirationTime { get; set; }

    #endregion Cache
}
