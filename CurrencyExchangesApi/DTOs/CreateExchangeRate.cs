using System.ComponentModel.DataAnnotations;

namespace CurrencyExchangesApi.DTOs
{
    public class CreateExchangeRate
    {
        [Required]
        [StringLength(3, MinimumLength = 3)]
        public string BaseCurrencyCode { get; set; } = string.Empty;

        [Required]
        [StringLength(3, MinimumLength = 3)]
        public string TargetCurrencyCode { get; set; } = string.Empty;

        [Required]
        public decimal Rate { get; set; }
    }
}
