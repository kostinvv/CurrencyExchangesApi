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
            var response = await _service.GetCurrencies();

            if (response.Status == ServiceStatus.ServerError)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = response.Message });
            }

            return Ok(response.Data);
        }

        [HttpGet("currency/code")]
        public async Task<IActionResult> Get(string code)
        {
            if (code.Length != 3)
            {
                return BadRequest(new { message = "Неверный код валюты." });
            }

            var response = await _service.GetCurrency(code);

            if (response.Status == ServiceStatus.ServerError)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = response.Message });
            }

            if (response.Data == null)
            {
                return NotFound(new { message = "Валюта не найдена." });
            }

            return Ok(response.Data);
        }

        [HttpPost("currencies")]
        public async Task<IActionResult> Create([FromForm] CreateCurrency currencyDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var response = await _service.CreateCurrency(currencyDto);

            if (response.Status == ServiceStatus.ServerError)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = response.Message });
            }

            if (response.Status == ServiceStatus.Conflict)
            {
                return Conflict(new { message = response.Message });
            }

            return Ok(response.Data);
        }
    }
}
