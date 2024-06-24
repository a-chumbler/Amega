using Amega.Services.Implementations.ApiModels;
using Amega.Services.Models;
using System.Collections.ObjectModel;

namespace Amega.Services.Implementations
{

    // we can use cache for the data from API so that we can call API less frequently and respond
    // faster but cache settings depend on API restrictions and business requirements
    public class AlphavantageDemoPriceDataProvider : IPriceDataProvider
    {
        // this is hardcoded because only these pairs allowed for demo
        private readonly static ReadOnlyCollection<string> _allowedInstruments = new ReadOnlyCollection<string>(["USDJPY","BTCEUR"]);

        // again, we hardcode urls for simplicity, in a realworld scenario we will use configuration and build query properly
        private static readonly Dictionary<string, string> _urls = new Dictionary<string, string>() 
        { 
            { "USDJPY", "https://www.alphavantage.co/query?function=CURRENCY_EXCHANGE_RATE&from_currency=USD&to_currency=JPY&apikey=demo" },
            { "BTCEUR", "https://www.alphavantage.co/query?function=CURRENCY_EXCHANGE_RATE&from_currency=BTC&to_currency=EUR&apikey=demo" },
        };

        private readonly IHttpClientFactory _clientFactory;

        public AlphavantageDemoPriceDataProvider(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public ValueTask<ReadOnlyCollection<string>> GetInstruments(CancellationToken cancellationToken)
        {
            return ValueTask.FromResult(_allowedInstruments);
        }

        public async ValueTask<CurrencyExchangeRate> GetPrice(string instrument, CancellationToken cancellationToken)
        {
            if (!_urls.TryGetValue(instrument, out var url))
            {
                throw new ArgumentException("Instrument is not found", nameof(instrument));
            }

            var client = _clientFactory.CreateClient(nameof(AlphavantageDemoPriceDataProvider));
            var apiModel = await client.GetFromJsonAsync<ExchangeCurrencyApiModel>(url, cancellationToken);

            // we deliberately dont check for null here, because we expect a strong contract with API,
            // if we face an issue we need to define requirements and handle it accordingly

            var exchangeRate = new CurrencyExchangeRate(
                apiModel!.RealtimeCurrencyExchangeRate.FromCurrencyCode,
                apiModel.RealtimeCurrencyExchangeRate.ToCurrencyCode,
                decimal.Parse(apiModel.RealtimeCurrencyExchangeRate.ExchangeRate));

            return exchangeRate;
        }
    }
}
