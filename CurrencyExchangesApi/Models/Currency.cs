using Newtonsoft.Json;

namespace CurrencyExchangesApi.Models
{
    public class Currency
    {
        public int CurrencyId { get; set; }
        public string Code { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "name")]
        public string FullName { get; set; } = string.Empty;
        public string Sign { get; set; } = string.Empty;
    }
}
