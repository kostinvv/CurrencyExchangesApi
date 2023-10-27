using CurrencyExchangesApi.DTOs;
using CurrencyExchangesApi.Enums;
using CurrencyExchangesApi.Models;
using CurrencyExchangesApi.Models.Responses;

namespace CurrencyExchangesApi.Services
{
    public class ExchangeRateService
    {
        private readonly IExchangeRateRepository _exchangeRepository;
        private readonly ICurrenciesRepository _currencyRepository;

        public ExchangeRateService(IExchangeRateRepository exchangeRepository, ICurrenciesRepository currenciesRepository)
        {
            _exchangeRepository = exchangeRepository;
            _currencyRepository = currenciesRepository;
        }

        public async Task<IEnumerable<ExchangeRate>> GetExchangeRates()
        {
            var exchangeRates = await _exchangeRepository.Get();

            return exchangeRates;
        }

        public async Task<ExchangeRate> GetExchangeRate(string codePair)
        {
            var currentCode = codePair[..^3];
            var targetCode = codePair[3..];

            var exchangeRate = await _exchangeRepository.Get(currentCode, targetCode);

            return exchangeRate;
        }

        public async Task<Response<ExchangeRate>> CreateExchangeRate(CreateExchangeRate createDto)
        {
            var exchangeRate = await _exchangeRepository.Get(createDto.BaseCurrencyCode, createDto.TargetCurrencyCode);

            if (exchangeRate != null)
            {
                return new Response<ExchangeRate>()
                {
                    Status = ServiceStatus.Conflict,
                };
            }

            var baseCurrency = await _currencyRepository.Get(createDto.BaseCurrencyCode);

            var targetCurrency = await _currencyRepository.Get(createDto.TargetCurrencyCode);

            if (baseCurrency == null || targetCurrency == null)
            {
                return new Response<ExchangeRate>()
                {
                    Message = $"Currency by code {createDto.BaseCurrencyCode} or {createDto.TargetCurrencyCode} not found.",
                    Status = ServiceStatus.NotFound,
                };
            }

            await _exchangeRepository.Insert(createDto.BaseCurrencyCode, createDto.TargetCurrencyCode, createDto.Rate);

            var createdExchangeRate = await _exchangeRepository.Get(createDto.BaseCurrencyCode, createDto.TargetCurrencyCode);

            return new Response<ExchangeRate>()
            {
                Data = createdExchangeRate,
                Status = ServiceStatus.Success,
            };
        }

        public async Task<ExchangeRate> UpdateExchangeRate(string codePair, EditExchageRate editDto)
        {
            var currentCode = codePair[..^3];
            var targetCode = codePair[3..];

            await _exchangeRepository.Update(currentCode, targetCode, editDto.Rate);

            return await _exchangeRepository.Get(currentCode, targetCode);
        }
    }
}
