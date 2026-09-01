using System.Threading.Tasks;
using TradingLab.Application.Abstractions.MarketData;
using TradingLab.Application.Modules.Market;
using TradingLab.Domain.Market;
using Xunit;

namespace TradingLab.UnitTests.Application
{
    public class MarketDataServiceTests
    {
        [Fact]
        public async Task GetLatestAsync_ReturnsProviderResult()
        {
            var market = TradingLab.Domain.Market.Market.BtcUsdt;
            var tf = TradingLab.Domain.Market.Timeframe.OneHour;

            var price = new Price(100m, Asset.USDT);
            var candle = new Candle(market, tf, System.DateTimeOffset.UtcNow, price, price, price, price, 1m);
            var provider = new FakeProvider(MarketDataResult.FromSuccess(price, candle));
            var svc = new MarketDataService(provider);

            var res = await svc.GetLatestAsync(market, tf);

            Assert.True(res.Success);
            Assert.Equal(100m, res.CurrentPrice!.Amount);
        }

        private class FakeProvider : IMarketDataProvider
        {
            private readonly MarketDataResult _result;
            public FakeProvider(MarketDataResult result) { _result = result; }
            public Task<MarketDataResult> GetLatestAsync(TradingLab.Domain.Market.Market market, TradingLab.Domain.Market.Timeframe timeframe, System.Threading.CancellationToken cancellationToken = default) => Task.FromResult(_result);
            public Task<MarketHistoryResult> GetHistoryAsync(TradingLab.Domain.Market.Market market, TradingLab.Domain.Market.Timeframe timeframe, int limit, System.DateTimeOffset? to = null, System.Threading.CancellationToken cancellationToken = default)
                => Task.FromResult(MarketHistoryResult.FromError(MarketDataError.Unknown));
        }
    }
}
