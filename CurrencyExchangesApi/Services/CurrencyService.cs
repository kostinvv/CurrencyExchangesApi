using CurrencyExchangesApi.DTOs;
using CurrencyExchangesApi.Enums;
using CurrencyExchangesApi.Models;
using CurrencyExchangesApi.Models.Responses;

namespace CurrencyExchangesApi.Services
{
    public class CurrencyService
    {
        private readonly ICurrenciesRepository _repository;

        public CurrencyService(ICurrenciesRepository repository)
        {
            _repository = repository;
        }

        public async Task<Currency> GetCurrency(string code)
        {
            var currency = await _repository.Get(code);

            return currency;
        }

        public async Task<IEnumerable<Currency>> GetCurrencies()
        {
            var currencies = await _repository.Get();

            return currencies;
        }

        public async Task<Response<Currency>> CreateCurrency(CreateCurrency currencyDto)
        {
            var currency = await _repository.Get(currencyDto.Code);

            if (currency != null)
            {
                return new Response<Currency>() 
                { 
                    Status = ServiceStatus.Conflict,
                };
            }

            var newCurrency = new Currency
            {
                Code = currencyDto.Code,
                FullName = currencyDto.FullName,
                Sign = currencyDto.Sign,
            };

            await _repository.Insert(newCurrency);

            var createdСurrency = await _repository.Get(newCurrency.Code);

            return new Response<Currency>()
            {
                Data = createdСurrency,
                Status = ServiceStatus.Success,
            };
        }
    }
}
