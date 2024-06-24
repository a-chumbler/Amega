namespace Amega.Services.Models
{
    public record class CurrencyExchangeRate(string FromCode, string ToCode, decimal ExchangeRate) { }
}
