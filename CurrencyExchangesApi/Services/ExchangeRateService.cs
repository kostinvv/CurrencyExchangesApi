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
                    Message = $"[GetExchangeRates]: {ex.Message}",
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
                    Message = $"[GetExchangeRate]: {ex.Message}",
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
                        Message = "Валютный курс уже существует.",
                    };
                }

                var baseCurrency = await _currencyRepository.Get(createDto.BaseCurrencyCode);

                var targetCurrency = await _currencyRepository.Get(createDto.TargetCurrencyCode);

                if (baseCurrency == null || targetCurrency == null)
                {
                    return new Response<ExchangeRate>()
                    {
                        Status = ServiceStatus.NotFound,
                        Message = $"Валюта с кодом {createDto.BaseCurrencyCode} или {createDto.TargetCurrencyCode} не найдена.",
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
                    Message = $"[CreateExchangeRate]: {ex.Message}",
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
                        Message = $"Валютный курс не найден.",
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
                    Message = $"[UpdateExchangeRate]: {ex.Message}",
                };
            }
        }

        public async Task<Response<GetCurrencyExchange>> ConvertCurrency(CurrencyExchangeDto currencyExchangeDto)
        {
            try
            {
                var baseCurrencyCode = currencyExchangeDto.From;
                var targetCurrencyCode = currencyExchangeDto.To;

                var exchangeRate = await GetDirectExchangeRate(baseCurrencyCode, targetCurrencyCode);

                if (exchangeRate == null)
                {
                    exchangeRate = await GetReverseExchangeRate(baseCurrencyCode, targetCurrencyCode);
                }

                if (exchangeRate == null)
                {
                    exchangeRate = await GetCrossExchangeRate(baseCurrencyCode, targetCurrencyCode);
                }

                if (exchangeRate == null)
                {
                    return new Response<GetCurrencyExchange>()
                    {
                        Status = ServiceStatus.NotFound,
                        Message = $"Валютные курсы не найдены.",
                    };
                }

                var convertedAmount = currencyExchangeDto.Amount * exchangeRate.Rate;

                return new Response<GetCurrencyExchange>()
                {
                    Status = ServiceStatus.Success,
                    Data = new GetCurrencyExchange
                    {
                        BaseCurrency = exchangeRate.BaseCurrency,
                        TargetCurrency = exchangeRate.TargetCurrency,
                        Rate = decimal.Round(exchangeRate.Rate, 6),
                        Amount = currencyExchangeDto.Amount,
                        ConvertedAmount = decimal.Round(convertedAmount, 2),
                    },
                };
            }
            catch (Exception ex)
            {
                return new Response<GetCurrencyExchange>()
                {
                    Status = ServiceStatus.ServerError,
                    Message = $"[ConvertCurrency]: {ex.Message}",
                };
            }
        }

        private async Task<ExchangeRate> GetDirectExchangeRate(string baseCurrencyCode, string targetCurrencyCode) 
            => await _exchangeRepository.Get(baseCurrencyCode, targetCurrencyCode);

        private async Task<ExchangeRate> GetReverseExchangeRate(string baseCurrencyCode, string targetCurrencyCode)
        {
            var exchangeRate = await _exchangeRepository.Get(targetCurrencyCode, baseCurrencyCode);

            if (exchangeRate == null) return null!;

            var reverseRate = 1 / exchangeRate.Rate;

            return new ExchangeRate
            {
               BaseCurrency = exchangeRate.BaseCurrency,
               TargetCurrency = exchangeRate.TargetCurrency,
               Rate = reverseRate,
            };
        }

        private async Task<ExchangeRate> GetCrossExchangeRate(string baseCurrencyCode, string targetCurrencyCode)
        {
            var usdToBase = await _exchangeRepository.Get("USD", baseCurrencyCode);
            var usdToTarget = await _exchangeRepository.Get("USD", targetCurrencyCode);

            if (usdToBase == null || usdToTarget == null) return null!;

            var crossRate = usdToBase.Rate / usdToTarget.Rate;

            var baseCurrency = usdToBase.TargetCurrency;
            var targetCurrency = usdToTarget.TargetCurrency;

            return new ExchangeRate
            {
                BaseCurrency = baseCurrency,
                TargetCurrency = targetCurrency,
                Rate = crossRate,
            };
        }
    }
}
