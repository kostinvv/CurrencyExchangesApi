using CurrencyExchangesApi.Models;

namespace CurrencyExchangesApi.DTOs
{
    public class GetCurrencyExchange
    {
        public Currency BaseCurrency { get; set; } = null!;
        public Currency TargetCurrency { get; set; } = null!;
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
        public decimal ConvertedAmount { get; set; }
    }
}
