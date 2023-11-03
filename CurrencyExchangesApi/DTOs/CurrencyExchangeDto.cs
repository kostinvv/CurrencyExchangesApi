using System.ComponentModel.DataAnnotations;

namespace CurrencyExchangesApi.DTOs
{
    public class CurrencyExchangeDto
    {
        [Required]
        [StringLength(3, MinimumLength = 3)]
        public string From { get; set; } = string.Empty;

        [Required]
        [StringLength(3, MinimumLength = 3)]
        public string To { get; set; } = string.Empty;

        [Required]
        public decimal Amount { get; set; }
    }
}
