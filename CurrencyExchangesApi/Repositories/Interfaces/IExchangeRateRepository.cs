using CurrencyExchangesApi.Models;

namespace CurrencyExchangesApi.Repositories.Interfaces
{
    public interface IExchangeRateRepository
    {
        public Task<IEnumerable<ExchangeRate>> Get();
        public Task<ExchangeRate> Get(string currentCode, string targetCode);
        public Task Insert(string currentCode, string targetCode, decimal rate);
        public Task Update(string currentCode, string targetCode, decimal rate);
    }
}
