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

        public async Task<Response<Currency>> GetCurrency(string code)
        {
            try
            {
                var currency = await _repository.Get(code);

                return new Response<Currency>()
                {
                    Data = currency,
                    Status = ServiceStatus.Success,
                };
            }
            catch (Exception ex)
            {
                return new Response<Currency>()
                {
                    Status = ServiceStatus.ServerError,
                    Message = ex.Message,
                };
            }

        }

        public async Task<Response<IEnumerable<Currency>>> GetCurrencies()
        {
            try
            {
                var currencies = await _repository.Get();

                return new Response<IEnumerable<Currency>>()
                {
                    Data = currencies,
                    Status = ServiceStatus.Success,
                };
            }
            catch (Exception ex)
            {
                return new Response<IEnumerable<Currency>>()
                {
                    Status = ServiceStatus.ServerError,
                    Message = ex.Message,
                };
            }
        }

        public async Task<Response<Currency>> CreateCurrency(CreateCurrency currencyDto)
        {
            try
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
            catch (Exception ex)
            {
                return new Response<Currency>()
                {
                    Status = ServiceStatus.ServerError,
                    Message = ex.Message,
                };
            }
        }
    }
}
