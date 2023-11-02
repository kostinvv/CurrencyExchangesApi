using CurrencyExchangesApi.DTOs;
using CurrencyExchangesApi.Enums;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyExchangesApi.Controllers
{
    [Route("api")]
    [ApiController]
    public class ExchangeRateController : ControllerBase
    {
        private readonly ExchangeRateService _service;

        public ExchangeRateController(ExchangeRateService service)
        {
            _service = service;
        }

        [HttpGet("exchangeRates")]
        public async Task<IActionResult> Get()
        {
            var response = await _service.GetExchangeRates();

            if (response.Status == ServiceStatus.ServerError)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = response.Message });
            }

            return Ok(response.Data);
        }

        [HttpGet("exchangeRates/codePair")]
        public async Task<IActionResult> Get(string codePair)
        {
            if (codePair.Length != 6)
            {
                return BadRequest( new { message = "Пара кодов передана неверно." } );
            }

            var response = await _service.GetExchangeRate(codePair);

            if (response.Status == ServiceStatus.ServerError)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = response.Message });
            }

            if (response.Data == null)
            {
                return NotFound( new { message = "Курс не найден." } );
            }

            return Ok(response.Data);
        }

        [HttpPost("exchangeRates")]
        public async Task<IActionResult> Create([FromBody] CreateExchangeRate createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest( new { message = "Значения не переданы." } );
            }

            var response = await _service.CreateExchangeRate(createDto);

            if (response.Status == ServiceStatus.ServerError)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = response.Message });
            }

            if (response.Status == ServiceStatus.Conflict)
            {
                return Conflict(new { message = response.Message });
            }

            if (response.Status == ServiceStatus.NotFound)
            {
                return NotFound( new { response.Message } );
            }

            return Ok(response.Data);
        }

        [HttpPatch("exchangeRate/codePair")]
        public async Task<IActionResult> Update(string codePair, [FromBody] EditExchageRate editDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest( new { message = "Значения не переданы." } );
            }

            if (codePair.Length != 6)
            {
                return BadRequest( new { message = "Пара кодов передана неверно." } );
            }

            var response = await _service.UpdateExchangeRate(codePair, editDto);

            if (response.Status == ServiceStatus.ServerError)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = response.Message });
            }

            if (response.Status == ServiceStatus.NotFound)
            {
                return NotFound(new { message = response.Message });
            }

            return Ok(response.Data);
        }

        [HttpGet("exchange")]
        public async Task<IActionResult> Get(string baseCurrencyCode, string targetCurrencyCode, decimal amount)
        {
            if (baseCurrencyCode.Length != 3 || targetCurrencyCode.Length != 3)
            {
                return BadRequest(new { message = "Неверные коды валют." });
            }

            var response = await _service.GetCurrencyExchange(baseCurrencyCode, targetCurrencyCode, amount);

            if (response.Status == ServiceStatus.ServerError)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = response.Message });
            }

            if (response.Status == ServiceStatus.NotFound)
            {
                return NotFound(new { message = response.Message });
            }

            return Ok(response.Data);
        }
    }
}
