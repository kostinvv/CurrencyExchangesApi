using CurrencyExchangesApi.Models;

namespace CurrencyExchangesApi.Repositories.Interfaces
{
    public interface ICurrenciesRepository
    {
        public Task<Currency> Get(string code);
        public Task<IEnumerable<Currency>> Get();
        public Task Insert(Currency currency);
    }
}
