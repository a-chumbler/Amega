using System.Text.Json.Serialization;

namespace Amega.Services.Implementations.ApiModels
{
    public class ExchangeCurrencyApiModel
    {
        [JsonPropertyName("Realtime Currency Exchange Rate")]
        public CurrencyExchangeRateApiModel RealtimeCurrencyExchangeRate { get; set; }
    }

    // we don't need all fields but lets retrive all fields for this test
    public class CurrencyExchangeRateApiModel
    {
        [JsonPropertyName("1. From_Currency Code")]
        public string FromCurrencyCode { get; set; }

        [JsonPropertyName("2. From_Currency Name")]
        public string FromCurrencyName { get; set; }

        [JsonPropertyName("3. To_Currency Code")]
        public string ToCurrencyCode { get; set; }

        [JsonPropertyName("4. To_Currency Name")]
        public string ToCurrencyName { get; set; }

        [JsonPropertyName("5. Exchange Rate")]
        public string ExchangeRate { get; set; }

        [JsonPropertyName("6. Last Refreshed")]
        public string LastRefreshed { get; set; }

        [JsonPropertyName("7. Time Zone")]
        public string TimeZone { get; set; }

        [JsonPropertyName("8. Bid Price")]
        public string BidPrice { get; set; }

        [JsonPropertyName("9. Ask Price")]
        public string AskPrice { get; set; }
    }
}
