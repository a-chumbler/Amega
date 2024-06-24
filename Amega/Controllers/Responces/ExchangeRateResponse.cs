namespace Amega.Controllers.Responces
{
    public record class ExchangeRateResponse(bool IsSuccess, ExcangeRate? ExcangeRate) { }
    public record class ExcangeRate(string FromCode, string ToCode, decimal ExchangeRate) { }
}
