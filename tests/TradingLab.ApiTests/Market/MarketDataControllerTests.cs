using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TradingLab.Api.Features.MarketData;
using TradingLab.Application.Abstractions.MarketData;
using TradingLab.Application.Modules.Market;
using TradingLab.Domain.Market;
using Xunit;

namespace TradingLab.ApiTests.Market
{
    public class MarketDataControllerTests
    {
        [Fact]
        public async Task Get_Returns200_OnSuccess()
        {
            var price = new Price(100m, Asset.USDT);
            var candle = new Candle(TradingLab.Domain.Market.Market.BtcUsdt, TradingLab.Domain.Market.Timeframe.OneHour, System.DateTimeOffset.UtcNow, price, price, price, price, 1m);
            var svc = new FakeService(MarketDataResult.FromSuccess(price, candle));
            var controller = new MarketDataController(svc);

            var result = await controller.Get("BTCUSDT", "1h");

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(ok.Value);
        }

        [Fact]
        public async Task Get_Returns404_OnUnsupportedMarket()
        {
            var svc = new FakeService(MarketDataResult.FromError(MarketDataError.NotFound));
            var controller = new MarketDataController(svc);

            var result = await controller.Get("ETHUSDT", "1h");

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Get_Returns400_OnUnsupportedTimeframe()
        {
            var svc = new FakeService(MarketDataResult.FromError(MarketDataError.NotFound));
            var controller = new MarketDataController(svc);

            var result = await controller.Get("BTCUSDT", "5m");

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Get_Returns503_OnNetwork()
        {
            var svc = new FakeService(MarketDataResult.FromError(MarketDataError.Network));
            var controller = new MarketDataController(svc);

            var result = await controller.Get("BTCUSDT", "1h");

            var status = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(503, status.StatusCode);
        }

        [Fact]
        public async Task Get_Returns502_OnInvalidResponse()
        {
            var svc = new FakeService(MarketDataResult.FromError(MarketDataError.InvalidResponse));
            var controller = new MarketDataController(svc);

            var result = await controller.Get("BTCUSDT", "1h");

            var status = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(502, status.StatusCode);
        }

        [Fact]
        public async Task Get_Returns503_OnProviderUnavailable()
        {
            var svc = new FakeService(MarketDataResult.FromError(MarketDataError.ProviderUnavailable));
            var controller = new MarketDataController(svc);

            var result = await controller.Get("BTCUSDT", "1h");

            var status = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(503, status.StatusCode);
        }

        [Fact]
        public async Task Get_Returns500_OnUnexpectedException()
        {
            var svc = new ThrowingService();
            var controller = new MarketDataController(svc);

            var result = await controller.Get("BTCUSDT", "1h");

            var status = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(500, status.StatusCode);
        }

        private class FakeService : IMarketDataService
        {
            private readonly MarketDataResult _res;
            public FakeService(MarketDataResult res) { _res = res; }
            public Task<MarketDataResult> GetLatestAsync(TradingLab.Domain.Market.Market market, TradingLab.Domain.Market.Timeframe timeframe, System.Threading.CancellationToken cancellationToken = default) => Task.FromResult(_res);
            public Task<MarketHistoryResult> GetHistoryAsync(TradingLab.Domain.Market.Market market, TradingLab.Domain.Market.Timeframe timeframe, int limit, System.DateTimeOffset? to = null, System.Threading.CancellationToken cancellationToken = default)
                => Task.FromResult(MarketHistoryResult.FromError(MarketDataError.Unknown));
        }

        [Fact]
        public async Task History_Returns200_OnSuccess()
        {
            var price = new Price(77700m, Asset.USDT);
            var candle = new Candle(TradingLab.Domain.Market.Market.BtcUsdt, TradingLab.Domain.Market.Timeframe.OneHour, new System.DateTimeOffset(2026,9,1,12,0,0, System.TimeSpan.Zero), price, price, price, price, 0.123m);
            var history = MarketHistoryResult.FromSuccess(new[] { candle });
            var svc = new FakeHistoryService(history);
            var controller = new MarketDataController(svc);

            var result = await controller.History("BTCUSDT", "1h", 100, null);

            var ok = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<TradingLab.Api.Features.MarketData.Contracts.MarketHistoryDto>(ok.Value);
            Assert.Equal("BTC/USDT", dto.Market);
            Assert.Equal("1h", dto.Timeframe);
            Assert.Single(dto.Candles);
            var c = dto.Candles[0];
            Assert.Equal(new System.DateTimeOffset(2026,9,1,12,0,0, System.TimeSpan.Zero), c.IntervalStart);
            Assert.Equal(77700m, c.Open);
            Assert.Equal(0.123m, c.Volume);
        }

        [Fact]
        public async Task History_Returns404_OnUnsupportedMarket()
        {
            var svc = new FakeHistoryService(MarketHistoryResult.FromError(MarketDataError.NotFound));
            var controller = new MarketDataController(svc);

            var result = await controller.History("ETHUSDT", "1h", null, null);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task History_Returns400_OnUnsupportedTimeframe()
        {
            var svc = new FakeHistoryService(MarketHistoryResult.FromError(MarketDataError.NotFound));
            var controller = new MarketDataController(svc);

            var result = await controller.History("BTCUSDT", "5m", null, null);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task History_Returns400_OnLimitZero()
        {
            var svc = new FakeHistoryService(MarketHistoryResult.FromError(MarketDataError.NotFound));
            var controller = new MarketDataController(svc);

            var result = await controller.History("BTCUSDT", "1h", 0, null);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task History_Returns400_OnLimitAboveMaximum()
        {
            var svc = new FakeHistoryService(MarketHistoryResult.FromError(MarketDataError.NotFound));
            var controller = new MarketDataController(svc);

            var result = await controller.History("BTCUSDT", "1h", 1001, null);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task History_Returns400_OnInvalidTo()
        {
            var svc = new FakeHistoryService(MarketHistoryResult.FromSuccess(new TradingLab.Domain.Market.Candle[0]));
            var controller = new MarketDataController(svc);

            var result = await controller.History("BTCUSDT", "1h", null, "not-a-number");

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task History_Returns503_OnNetwork()
        {
            var svc = new FakeHistoryService(MarketHistoryResult.FromError(MarketDataError.Network));
            var controller = new MarketDataController(svc);

            var result = await controller.History("BTCUSDT", "1h", null, null);

            var status = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(503, status.StatusCode);
        }

        [Fact]
        public async Task History_Returns502_OnInvalidResponse()
        {
            var svc = new FakeHistoryService(MarketHistoryResult.FromError(MarketDataError.InvalidResponse));
            var controller = new MarketDataController(svc);

            var result = await controller.History("BTCUSDT", "1h", null, null);

            var status = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(502, status.StatusCode);
        }

        [Fact]
        public async Task History_Returns503_OnProviderUnavailable()
        {
            var svc = new FakeHistoryService(MarketHistoryResult.FromError(MarketDataError.ProviderUnavailable));
            var controller = new MarketDataController(svc);

            var result = await controller.History("BTCUSDT", "1h", null, null);

            var status = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(503, status.StatusCode);
        }

        [Fact]
        public async Task History_Returns500_OnUnexpectedException()
        {
            var svc = new ThrowingService();
            var controller = new MarketDataController(svc);

            var result = await controller.History("BTCUSDT", "1h", null, null);

            var status = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(500, status.StatusCode);
        }

        private class FakeHistoryService : IMarketDataService
        {
            private readonly MarketHistoryResult _res;
            public FakeHistoryService(MarketHistoryResult res) { _res = res; }
            public Task<MarketDataResult> GetLatestAsync(TradingLab.Domain.Market.Market market, TradingLab.Domain.Market.Timeframe timeframe, System.Threading.CancellationToken cancellationToken = default) => Task.FromResult(MarketDataResult.FromError(MarketDataError.Unknown));
            public Task<MarketHistoryResult> GetHistoryAsync(TradingLab.Domain.Market.Market market, TradingLab.Domain.Market.Timeframe timeframe, int limit, System.DateTimeOffset? to = null, System.Threading.CancellationToken cancellationToken = default)
                => Task.FromResult(_res);
        }

        private class CapturingHistoryService : IMarketDataService
        {
            private readonly MarketHistoryResult _res;
            public bool Called { get; private set; }
            public TradingLab.Domain.Market.Market? LastMarket { get; private set; }
            public TradingLab.Domain.Market.Timeframe? LastTimeframe { get; private set; }
            public int? LastLimit { get; private set; }
            public System.DateTimeOffset? LastTo { get; private set; }

            public CapturingHistoryService(MarketHistoryResult res)
            {
                _res = res;
            }

            public Task<MarketDataResult> GetLatestAsync(TradingLab.Domain.Market.Market market, TradingLab.Domain.Market.Timeframe timeframe, System.Threading.CancellationToken cancellationToken = default)
                => Task.FromResult(MarketDataResult.FromError(MarketDataError.Unknown));

            public Task<MarketHistoryResult> GetHistoryAsync(TradingLab.Domain.Market.Market market, TradingLab.Domain.Market.Timeframe timeframe, int limit, System.DateTimeOffset? to = null, System.Threading.CancellationToken cancellationToken = default)
            {
                Called = true;
                LastMarket = market;
                LastTimeframe = timeframe;
                LastLimit = limit;
                LastTo = to;
                return Task.FromResult(_res);
            }
        }

        [Fact]
        public async Task History_ForwardsDefaultLimit_WhenLimitOmitted()
        {
            var svc = new CapturingHistoryService(MarketHistoryResult.FromSuccess(new TradingLab.Domain.Market.Candle[0]));
            var controller = new MarketDataController(svc);

            var result = await controller.History("BTCUSDT", "1h", null, null);

            Assert.True(svc.Called);
            Assert.Equal(100, svc.LastLimit);
            Assert.Null(svc.LastTo);
            Assert.Equal(TradingLab.Domain.Market.Market.BtcUsdt, svc.LastMarket);
            Assert.Equal(TradingLab.Domain.Market.Timeframe.OneHour, svc.LastTimeframe);
        }

        [Fact]
        public async Task History_ForwardsCustomLimit()
        {
            var svc = new CapturingHistoryService(MarketHistoryResult.FromSuccess(new TradingLab.Domain.Market.Candle[0]));
            var controller = new MarketDataController(svc);

            var result = await controller.History("BTCUSDT", "1h", 37, null);

            Assert.True(svc.Called);
            Assert.Equal(37, svc.LastLimit);
        }

        [Fact]
        public async Task History_ForwardsTo_AsDateTimeOffset()
        {
            var unix = 1690000000L;
            var svc = new CapturingHistoryService(MarketHistoryResult.FromSuccess(new TradingLab.Domain.Market.Candle[0]));
            var controller = new MarketDataController(svc);

            var result = await controller.History("BTCUSDT", "1h", 10, unix.ToString());

            Assert.True(svc.Called);
            Assert.Equal(System.DateTimeOffset.FromUnixTimeSeconds(unix), svc.LastTo);
        }

        [Fact]
        public async Task History_InvalidLimit_DoesNotCallService()
        {
            var svc = new CapturingHistoryService(MarketHistoryResult.FromSuccess(new TradingLab.Domain.Market.Candle[0]));
            var controller = new MarketDataController(svc);

            // invalid limit = 0
            var res1 = await controller.History("BTCUSDT", "1h", 0, null);
            Assert.IsType<BadRequestResult>(res1);
            Assert.False(svc.Called);

            // invalid limit = 1001
            var res2 = await controller.History("BTCUSDT", "1h", 1001, null);
            Assert.IsType<BadRequestResult>(res2);
            Assert.False(svc.Called);
        }

        [Fact]
        public async Task History_InvalidTo_DoesNotCallService()
        {
            var svc = new CapturingHistoryService(MarketHistoryResult.FromSuccess(new TradingLab.Domain.Market.Candle[0]));
            var controller = new MarketDataController(svc);

            var res = await controller.History("BTCUSDT", "1h", null, "not-a-number");
            Assert.IsType<BadRequestResult>(res);
            Assert.False(svc.Called);
        }

        [Fact]
        public async Task History_ReturnsMappedCandles_InSameOrder()
        {
            var price = new Price(100m, Asset.USDT);
            var c1 = new Candle(TradingLab.Domain.Market.Market.BtcUsdt, TradingLab.Domain.Market.Timeframe.OneHour, new System.DateTimeOffset(2026,1,1,0,0,0, System.TimeSpan.Zero), price, price, price, price, 1.1m);
            var c2 = new Candle(TradingLab.Domain.Market.Market.BtcUsdt, TradingLab.Domain.Market.Timeframe.OneHour, new System.DateTimeOffset(2026,1,1,1,0,0, System.TimeSpan.Zero), price, price, price, price, 2.2m);
            var svc = new FakeHistoryService(MarketHistoryResult.FromSuccess(new[] { c1, c2 }));
            var controller = new MarketDataController(svc);

            var result = await controller.History("BTCUSDT", "1h", null, null);

            var ok = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<TradingLab.Api.Features.MarketData.Contracts.MarketHistoryDto>(ok.Value);
            Assert.Equal(2, dto.Candles.Length);
            Assert.Equal(c1.IntervalStart, dto.Candles[0].IntervalStart);
            Assert.Equal(c2.IntervalStart, dto.Candles[1].IntervalStart);
            Assert.Equal(c1.Volume, dto.Candles[0].Volume);
            Assert.Equal(c2.Volume, dto.Candles[1].Volume);
        }

        private class ThrowingService : IMarketDataService
        {
            public Task<MarketDataResult> GetLatestAsync(TradingLab.Domain.Market.Market market, TradingLab.Domain.Market.Timeframe timeframe, System.Threading.CancellationToken cancellationToken = default)
            {
                throw new System.InvalidOperationException("boom");
            }
            public Task<MarketHistoryResult> GetHistoryAsync(TradingLab.Domain.Market.Market market, TradingLab.Domain.Market.Timeframe timeframe, int limit, System.DateTimeOffset? to = null, System.Threading.CancellationToken cancellationToken = default)
            {
                throw new System.InvalidOperationException("boom");
            }
        }
    }
}
