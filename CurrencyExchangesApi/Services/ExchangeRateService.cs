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

        public async Task<Response<IEnumerable<ExchangeRate>>> GetExchangeRates()
        {
            try
            {
                var exchangeRates = await _exchangeRepository.Get();

                return new Response<IEnumerable<ExchangeRate>>()
                {
                    Data = exchangeRates,
                    Status = ServiceStatus.Success,
                };
            }
            catch (Exception ex)
            {
                return new Response<IEnumerable<ExchangeRate>>()
                {
                    Status = ServiceStatus.ServerError,
                    Message = ex.Message,
                };
            }
        }

        public async Task<Response<ExchangeRate>> GetExchangeRate(string codePair)
        {
            var currentCode = codePair[..^3];
            var targetCode = codePair[3..];

            try
            {
                var exchangeRate = await _exchangeRepository.Get(currentCode, targetCode);

                return new Response<ExchangeRate>()
                {
                    Data = exchangeRate,
                    Status = ServiceStatus.Success,
                };
            }
            catch (Exception ex)
            {
                return new Response<ExchangeRate>()
                {
                    Status = ServiceStatus.ServerError,
                    Message = ex.Message,
                };
            }
        }

        public async Task<Response<ExchangeRate>> CreateExchangeRate(CreateExchangeRate createDto)
        {
            try
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
            catch (Exception ex)
            {
                return new Response<ExchangeRate>()
                {
                    Status = ServiceStatus.ServerError,
                    Message = ex.Message,
                };
            }
        }

        public async Task<Response<ExchangeRate>> UpdateExchangeRate(string codePair, EditExchageRate editDto)
        {
            try
            {
                var currentCode = codePair[..^3];
                var targetCode = codePair[3..];

                var exchangeRate = await _exchangeRepository.Get(currentCode, targetCode);

                if (exchangeRate == null)
                {
                    return new Response<ExchangeRate>()
                    {
                        Status = ServiceStatus.NotFound,
                    };
                }

                await _exchangeRepository.Update(currentCode, targetCode, editDto.Rate);

                return new Response<ExchangeRate>()
                {
                    Data = await _exchangeRepository.Get(currentCode, targetCode),
                };
            }
            catch (Exception ex)
            {
                return new Response<ExchangeRate>()
                {
                    Status = ServiceStatus.ServerError,
                    Message = ex.Message,
                };
            }
        }
    }
}
