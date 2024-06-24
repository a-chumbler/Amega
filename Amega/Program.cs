using Amega.Services;
using Amega.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Here we can configure the necessary URLs, properties and logging for the http client.
// and we can also implement resiliency policies in the form of retry, circuitbreaker, fallback etc.
builder.Services.AddHttpClient(nameof(AlphavantageDemoPriceDataProvider));
builder.Services.AddSingleton<IPriceDataProvider, AlphavantageDemoPriceDataProvider>();

var app = builder.Build();

app.UseWebSockets();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();

app.Run();