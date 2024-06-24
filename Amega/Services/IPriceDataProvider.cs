using Amega.Services.Models;
using System.Collections.ObjectModel;

namespace Amega.Services
{
    public interface IPriceDataProvider
    {
        public ValueTask<ReadOnlyCollection<string>> GetInstruments(CancellationToken cancellationToken);

        public ValueTask<CurrencyExchangeRate> GetPrice(string instrument, CancellationToken cancellationToken);
    }
}
