using Newtonsoft.Json;

namespace Catalogo.Service.Api.Models
{
    /// <summary>
    /// Obtêm dados de mercadorias E FILIAIS 1P e 3P DIVINO
    /// </summary>
    public class MercadoriaSeller
    {

        [JsonProperty("SELLER")]
        public string SELLER { get; set; }  

        [JsonProperty("SKU")]
        public string SKU { get; set; }  

        [JsonProperty("SKUMRT")]
        public string SKUMRT { get; set; }  

        public int LINHA { get; set; }
    }
}
