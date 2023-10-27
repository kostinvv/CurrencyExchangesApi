using CurrencyExchangesApi.DTOs;
using CurrencyExchangesApi.Enums;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyExchangesApi.Controllers
{
    [Route("api")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private readonly CurrencyService _service;

        public CurrencyController(CurrencyService service)
        {
            _service = service;
        }

        [HttpGet("currencies")]
        public async Task<IActionResult> Get() 
        {
            try
            {
                var currencies = await _service.GetCurrencies();

                return Ok(currencies);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("currency/code")]
        public async Task<IActionResult> Get(string code)
        {
            try
            {
                if (code.Length != 3)
                {
                    return BadRequest();
                }

                var currency = await _service.GetCurrency(code);

                if (currency == null)
                {
                    return NotFound();
                }

                return Ok(currency);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("currencies")]
        public async Task<IActionResult> Create([FromBody] CreateCurrency currencyDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                var response = await _service.CreateCurrency(currencyDto);

                if (response.Status == ServiceStatus.Conflict)
                {
                    return Conflict(response.Message);
                }

                return Ok(response.Data);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
