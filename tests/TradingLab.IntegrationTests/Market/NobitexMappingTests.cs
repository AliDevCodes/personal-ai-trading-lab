using TradingLab.Infrastructure.MarketData.Nobitex;
using TradingLab.Domain.Market;
using Xunit;

namespace TradingLab.IntegrationTests.Market
{
    public class NobitexMappingTests
    {
        [Fact]
        public void ParseStatsResponse_ReturnsPrice()
        {
            var json = "{ \"status\": \"ok\", \"stats\": { \"btc-usdt\": { \"latest\": \"28934.12\" } } }";
            var market = TradingLab.Domain.Market.Market.BtcUsdt;
            Assert.True(NobitexResponseMapper.TryParseStats(json, market, Asset.USDT, out var price));
            Assert.Equal(28934.12m, price.Amount);
            Assert.Equal("USDT", price.Quote.Code);
        }

        [Fact]
        public void ParseUdfHistory_ReturnsCandle()
        {
            var json = "{ \"t\": [1690000000], \"o\": [\"28000\"], \"h\": [\"29000\"], \"l\": [\"27900\"], \"c\": [\"28900\"], \"v\": [\"12.5\"] }";
            var market = TradingLab.Domain.Market.Market.BtcUsdt;
            Assert.True(NobitexResponseMapper.TryParseUdfHistory(json, market, Timeframe.OneHour, Asset.USDT, out var candle));
            Assert.Equal(28900m, candle.Close.Amount);
            Assert.Equal(12.5m, candle.Volume);
        }

        [Fact]
        public void ParseUdfHistory_MismatchedArrays_ReturnsFalse()
        {
            var json = "{ \"t\": [1690000000,1690000300], \"o\": [\"28000\"], \"h\": [\"29000\"], \"l\": [\"27900\"], \"c\": [\"28900\"], \"v\": [\"12.5\"] }";
            var market = TradingLab.Domain.Market.Market.BtcUsdt;
            Assert.False(NobitexResponseMapper.TryParseUdfHistory(json, market, Timeframe.OneHour, Asset.USDT, out var _));
        }

        [Fact]
        public void MapMarketSymbol_IsBtcUsdt()
        {
            var m = TradingLab.Domain.Market.Market.BtcUsdt;
            Assert.Equal("BTCUSDT", NobitexMarketDataProvider.MapMarketToSymbol(m));
        }

        [Fact]
        public void MapTimeframe_OneHour_ResolvesTo60()
        {
            Assert.Equal("60", NobitexMarketDataProvider.MapTimeframeToResolution(Timeframe.OneHour));
        }
    }
}
