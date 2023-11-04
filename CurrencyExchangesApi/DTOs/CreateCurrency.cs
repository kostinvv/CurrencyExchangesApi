using System.ComponentModel.DataAnnotations;

namespace CurrencyExchangesApi.DTOs
{
    public class CreateCurrency
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Sign { get; set; } = string.Empty;
    }
}
