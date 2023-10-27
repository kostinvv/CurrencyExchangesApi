namespace CurrencyExchangesApi.Models
{
    public class ExchangeRate
    {
        public int ExchangeRateId { get; set; }
        public Currency BaseCurrency { get; set; } = null!;
        public Currency TargetCurrency { get; set; } = null!;
        public decimal Rate { get; set; }

        public static implicit operator ExchangeRate(Task<ExchangeRate> v)
        {
            throw new NotImplementedException();
        }
    }
}
