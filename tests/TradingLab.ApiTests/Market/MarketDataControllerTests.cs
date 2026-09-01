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
        }

        private class ThrowingService : IMarketDataService
        {
            public Task<MarketDataResult> GetLatestAsync(TradingLab.Domain.Market.Market market, TradingLab.Domain.Market.Timeframe timeframe, System.Threading.CancellationToken cancellationToken = default)
            {
                throw new System.InvalidOperationException("boom");
            }
        }
    }
}
