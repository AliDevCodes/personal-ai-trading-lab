using System;
using System.Net.Http;
using System.Threading.Tasks;
using TradingLab.Infrastructure.MarketData.Nobitex;
using TradingLab.Domain.Market;
using Xunit;

namespace TradingLab.IntegrationTests.Market
{
    public class NobitexIntegrationTests
    {
        [Fact]
        public async Task GetLatestAsync_LiveProvider_Behaves()
        {
            var run = Environment.GetEnvironmentVariable("RUN_INTEGRATION_TESTS");
            if (!string.Equals(run, "true", StringComparison.OrdinalIgnoreCase))
            {
                // Skip integration test by not performing network activity by default.
                return;
            }

            using var client = new HttpClient();
            var provider = new NobitexMarketDataProvider(client);
            var result = await provider.GetLatestAsync(TradingLab.Domain.Market.Market.BtcUsdt, Timeframe.OneHour);
            Assert.True(result.Success, $"Provider call failed: {result.Error}");
            Assert.NotNull(result.CurrentPrice);
            Assert.NotNull(result.LatestCandle);
        }
    }
}
