using System.ComponentModel.DataAnnotations;

namespace CurrencyExchangesApi.DTOs
{
    public class CreateCurrency
    {
        [Required]
        [StringLength(3, MinimumLength = 3)]
        public string Code { get; set; } = string.Empty;

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public string Sign { get; set; } = string.Empty;
    }
}
