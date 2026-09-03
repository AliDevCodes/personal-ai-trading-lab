using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TradingLab.Application.Modules.Market;
using TradingLab.Application.Abstractions.MarketData;
using TradingLab.Domain.Market;
using Xunit;

namespace TradingLab.ApiTests.Integration
{
    public class MarketDataHostTests : IDisposable
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly FakeMarketDataService _fakeService;

        public MarketDataHostTests()
        {
            _fakeService = new FakeMarketDataService();
            _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Ensure deterministic test double: replace the real service with the
                    // exact fake instance the test asserts against.
                    services.RemoveAll<IMarketDataService>();
                    services.AddSingleton<IMarketDataService>(_fakeService);
                });

                // no buffering middleware: keep host minimal and rely on TestHost behavior
            });
        }

        [Fact]
        public async Task History_Returns200_With10Candles()
        {
            using var client = _factory.CreateClient();
            var res = await client.GetAsync("/api/market-data/BTCUSDT/history?timeframe=1h&limit=10");
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);

            var json = await res.Content.ReadAsStringAsync(CancellationToken.None);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            Assert.Equal("BTC/USDT", root.GetProperty("market").GetString());
            Assert.Equal("1h", root.GetProperty("timeframe").GetString());

            var candles = root.GetProperty("candles");
            Assert.Equal(10, candles.GetArrayLength());

            var first = candles[0];

            // Validate the whole sequence is ordered oldest -> newest with the exact
            // deterministic one-hour timestamps produced by the fake.
            for (var i = 0; i < candles.GetArrayLength(); i++)
            {
                var ts = DateTimeOffset.Parse(candles[i].GetProperty("intervalStart").GetString()!);
                Assert.Equal(FakeMarketDataService.Start.AddHours(i), ts);

                if (i > 0)
                {
                    var prev = DateTimeOffset.Parse(candles[i - 1].GetProperty("intervalStart").GetString()!);
                    Assert.True(prev < ts, $"Candle {i} must be ordered after candle {i - 1} (oldest -> newest)");
                }
            }

            // Validate a numeric mapping (open/close)
            var firstOpen = first.GetProperty("open").GetDecimal();
            var firstClose = first.GetProperty("close").GetDecimal();
            Assert.Equal(FakeMarketDataService.CandleOpenAmount, firstOpen);
            Assert.Equal(FakeMarketDataService.CandleCloseAmount, firstClose);

            // Ensure the fake instance registered in DI was invoked by the host
            Assert.True(_fakeService.HistoryCalled, "Fake service should have been invoked by the host");
        }

        public void Dispose()
        {
            _factory.Dispose();
        }

        private sealed class FakeMarketDataService : IMarketDataService
        {
            public static DateTimeOffset Start { get; } = new DateTimeOffset(2026, 9, 2, 13, 30, 0, TimeSpan.Zero);
            public const decimal CandleOpenAmount = 76000.00m;
            public const decimal CandleCloseAmount = 76200.00m;

            public bool HistoryCalled { get; private set; }

            public Task<MarketDataResult> GetLatestAsync(Domain.Market.Market market, Timeframe timeframe, CancellationToken cancellationToken = default)
            {
                // Not used in this test
                return Task.FromResult(MarketDataResult.FromError(MarketDataError.NotFound));
            }

            public Task<MarketHistoryResult> GetHistoryAsync(Domain.Market.Market market, Timeframe timeframe, int limit, DateTimeOffset? to = null, CancellationToken cancellationToken = default)
            {
                HistoryCalled = true;

                // Build exactly `limit` candles (or 10) deterministically
                var count = limit;
                var candles = Enumerable.Range(0, count).Select(i =>
                {
                    var ts = Start.AddHours(i);
                    var priceOpen = new Price(CandleOpenAmount, Asset.USDT);
                    var priceHigh = new Price(CandleOpenAmount + 200, Asset.USDT);
                    var priceLow = new Price(CandleOpenAmount - 100, Asset.USDT);
                    var priceClose = new Price(CandleCloseAmount, Asset.USDT);
                    var volume = 0.5m + i * 0.1m;
                    var c = new Candle(TradingLab.Domain.Market.Market.BtcUsdt, Timeframe.OneHour, ts, priceOpen, priceHigh, priceLow, priceClose, volume);
                    c.Validate();
                    return c;
                }).ToArray();

                return Task.FromResult(MarketHistoryResult.FromSuccess(candles));
            }
        }
    }
}
