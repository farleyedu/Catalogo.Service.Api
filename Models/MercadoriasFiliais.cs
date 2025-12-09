using Newtonsoft.Json;

namespace Catalogo.Service.Api.Models
{
    /// <summary>
    /// Obtêm dados de mercadorias E FILIAIS 1P e 3P DIVINO
    /// </summary>
    public class MercadoriasFiliais
    {
        [JsonProperty("CODIGOMERCADORIA")]
        public long CODIGOMERCADORIA { get; set; }

        [JsonProperty("CODIGOFILAISMERCADORIA")]
        public string CODIGOFILAISMERCADORIA { get; set; }
    }
}
