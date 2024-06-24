using Amega.Controllers.Responces;
using Amega.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace Amega.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MarketDataController : ControllerBase
    {
        private readonly ILogger<MarketDataController> _logger;
        private readonly IPriceDataProvider _priceDataProvider;

        public MarketDataController(
            ILogger<MarketDataController> logger,
            IPriceDataProvider priceDataProvider)
        {
            _logger = logger;
            _priceDataProvider = priceDataProvider;
        }

        [HttpGet]
        public async Task<IEnumerable<string>> GetAllInstruments(CancellationToken ct)
        {
            var instruments = await _priceDataProvider.GetInstruments(ct);
            return instruments;
        }

        [Route("ws")]
        public async Task GetInstrumentPrice(CancellationToken ct)
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await FetchPrice(webSocket, ct);
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }

        private async Task FetchPrice(WebSocket webSocket, CancellationToken cancellationToken)
        {
            var buffer = new byte[1024];
            var segment = new ArraySegment<byte>(buffer);
            var result = await webSocket.ReceiveAsync(segment, cancellationToken);
            
            while (!result.CloseStatus.HasValue)
            {
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                _logger.LogInformation($"Received message: {message}");

                using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                var sendTask = Task.Run(() => SendPriceData(webSocket, message, cts.Token));

                result = await webSocket.ReceiveAsync(
                    segment,
                    cancellationToken);

                cts.Cancel();
            }

            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
            _logger.LogInformation("WebSocket connection closed");
        }

        private async Task SendPriceData(WebSocket webSocket, string instrument, CancellationToken ct)
        {
            // in this method we can also handle and react somehow on errors from data provider
            // but due to no requirements we will just ignore errors.
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    ExchangeRateResponse response = null;
                    // lets chech instrument is correct (we can also check it in UI)
                    var awailableInstruments = await _priceDataProvider.GetInstruments(ct);
                    if (!awailableInstruments.Contains(instrument))
                    {
                        response = new ExchangeRateResponse(false, null);
                    }
                    else
                    {
                        var exchangeRate = await _priceDataProvider.GetPrice(instrument, ct);
                        response = new ExchangeRateResponse(
                            true,
                            new ExcangeRate(
                                exchangeRate.FromCode,
                                exchangeRate.ToCode,
                                exchangeRate.ExchangeRate));
                    }

                    var message = JsonSerializer.Serialize(response);
                    var responseBytes = Encoding.UTF8.GetBytes(message);
                    var segment = new ArraySegment<byte>(responseBytes);
                    var _ = webSocket.SendAsync(segment, WebSocketMessageType.Text, true, ct);

                    // we need to set up this delay properly according to business requirements
                    // for now we can just hardcode this value
                    await Task.Delay(5000, ct);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }
        }
    }
}
