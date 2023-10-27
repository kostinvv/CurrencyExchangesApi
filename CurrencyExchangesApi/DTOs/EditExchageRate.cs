using System.ComponentModel.DataAnnotations;

namespace CurrencyExchangesApi.DTOs
{
    public class EditExchageRate
    {
        [Required]
        public decimal Rate { get; set; }
    }
}
