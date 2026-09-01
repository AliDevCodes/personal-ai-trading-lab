using System.Threading.Tasks;
using TradingLab.Application.Abstractions.MarketData;
using TradingLab.Application.Modules.Market;
using TradingLab.Domain.Market;
using Xunit;

namespace TradingLab.UnitTests.Application
{
    public class MarketHistoryServiceTests
    {
        [Fact]
        public async Task GetHistory_ValidLimit_CallsProvider()
        {
            var market = TradingLab.Domain.Market.Market.BtcUsdt;
            var tf = TradingLab.Domain.Market.Timeframe.OneHour;
            var candle = new Candle(market, tf, System.DateTimeOffset.UtcNow, new Price(1m, Asset.USDT), new Price(1m, Asset.USDT), new Price(1m, Asset.USDT), new Price(1m, Asset.USDT), 1m);
            var provider = new FakeProvider(MarketHistoryResult.FromSuccess(new[] { candle }));
            var svc = new MarketDataService(provider);

            var res = await svc.GetHistoryAsync(market, tf, 10);

            Assert.True(res.Success);
            Assert.NotNull(res.Candles);
        }

        [Fact]
        public async Task GetHistory_LimitZero_IsRejected()
        {
            var provider = new ThrowIfCalledProvider();
            var svc = new MarketDataService(provider);
            var res = await svc.GetHistoryAsync(TradingLab.Domain.Market.Market.BtcUsdt, TradingLab.Domain.Market.Timeframe.OneHour, 0);
            Assert.False(res.Success);
            Assert.Equal(MarketDataError.InvalidResponse, res.Error);
        }

        [Fact]
        public async Task GetHistory_LimitTooLarge_IsRejected()
        {
            var provider = new ThrowIfCalledProvider();
            var svc = new MarketDataService(provider);
            var res = await svc.GetHistoryAsync(TradingLab.Domain.Market.Market.BtcUsdt, TradingLab.Domain.Market.Timeframe.OneHour, 1001);
            Assert.False(res.Success);
            Assert.Equal(MarketDataError.InvalidResponse, res.Error);
        }

        private class FakeProvider : IMarketDataProvider
        {
            private readonly MarketHistoryResult _res;
            public FakeProvider(MarketHistoryResult res) { _res = res; }
            public Task<MarketDataResult> GetLatestAsync(TradingLab.Domain.Market.Market market, TradingLab.Domain.Market.Timeframe timeframe, System.Threading.CancellationToken cancellationToken = default) => Task.FromResult(MarketDataResult.FromError(MarketDataError.Unknown));
            public Task<MarketHistoryResult> GetHistoryAsync(TradingLab.Domain.Market.Market market, TradingLab.Domain.Market.Timeframe timeframe, int limit, System.DateTimeOffset? to = null, System.Threading.CancellationToken cancellationToken = default) => Task.FromResult(_res);
        }

        private class ThrowIfCalledProvider : IMarketDataProvider
        {
            public Task<MarketDataResult> GetLatestAsync(TradingLab.Domain.Market.Market market, TradingLab.Domain.Market.Timeframe timeframe, System.Threading.CancellationToken cancellationToken = default) => Task.FromResult(MarketDataResult.FromError(MarketDataError.Unknown));
            public Task<MarketHistoryResult> GetHistoryAsync(TradingLab.Domain.Market.Market market, TradingLab.Domain.Market.Timeframe timeframe, int limit, System.DateTimeOffset? to = null, System.Threading.CancellationToken cancellationToken = default) => throw new System.Exception("Should not be called");
        }
    }
}
