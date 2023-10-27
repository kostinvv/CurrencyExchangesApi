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
            try
            {
                var exchangeRates = await _service.GetExchangeRates();

                return Ok(exchangeRates);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("exchangeRates/codePair")]
        public async Task<IActionResult> Get(string codePair)
        {
            try
            {
                if (codePair.Length != 6)
                {
                    return BadRequest();
                }

                var exchangeRates = await _service.GetExchangeRate(codePair);

                if (exchangeRates == null)
                {
                    return NotFound();
                }

                return Ok(exchangeRates);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("exchangeRates")]
        public async Task<IActionResult> Create([FromBody] CreateExchangeRate createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                var response = await _service.CreateExchangeRate(createDto);

                if (response.Status == ServiceStatus.Conflict)
                {
                    return Conflict();
                }

                if (response.Status == ServiceStatus.NotFound)
                {
                    return NotFound( new { response.Message, response.Status } );
                }

                return Ok(response.Data);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPatch("exchangeRate/codePair")]
        public async Task<IActionResult> Update(string codePair, [FromBody] EditExchageRate editDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                var response = await _service.UpdateExchangeRate(codePair, editDto);

                if (response.Status == ServiceStatus.NotFound)
                {
                    return NotFound();
                }

                return Ok(response.Data);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
