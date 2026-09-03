var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Register application services
builder.Services.AddScoped<TradingLab.Application.Modules.Market.IMarketDataService, TradingLab.Application.Modules.Market.MarketDataService>();

// Register Nobitex provider using typed HttpClient
builder.Services.AddHttpClient<TradingLab.Application.Abstractions.MarketData.IMarketDataProvider, TradingLab.Infrastructure.MarketData.Nobitex.NobitexMarketDataProvider>(client =>
{
    client.BaseAddress = new System.Uri("https://apiv2.nobitex.ir/");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.MapControllers();

app.Run();

// Expose Program for WebApplicationFactory in tests
public partial class Program { }
