using CurrencyExchangesApi.Enums;

namespace CurrencyExchangesApi.Models.Responses
{
    public class Response<TData>
    {
        public string? Message { get; set; } = string.Empty;
        public ServiceStatus Status { get; set; }
        public TData? Data { get; set; }
    }
}
