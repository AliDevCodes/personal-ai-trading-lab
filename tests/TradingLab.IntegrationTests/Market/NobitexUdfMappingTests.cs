using TradingLab.Infrastructure.MarketData.Nobitex;
using TradingLab.Domain.Market;
using Xunit;

namespace TradingLab.IntegrationTests.Market
{
    public class NobitexUdfMappingTests
    {
        [Fact]
        public void ParseMultiCandleUdf_ReturnsCandlesInOrder()
        {
            var json = "{ \"s\": \"ok\", \"t\": [1690000000,1690003600], \"o\": [\"28000\",\"28100\"], \"h\": [\"29000\",\"29100\"], \"l\": [\"27900\",\"28050\"], \"c\": [\"28900\",\"29050\"], \"v\": [\"12.5\",\"10.1\"] }";
            var market = TradingLab.Domain.Market.Market.BtcUsdt;
            Assert.True(NobitexResponseMapper.TryParseUdfHistoryToCandles(json, market, Timeframe.OneHour, Asset.USDT, out var candles));
            Assert.Equal(2, candles.Length);
            Assert.True(candles[0].IntervalStart < candles[1].IntervalStart);
            Assert.Equal(28900m, candles[0].Close.Amount);
            Assert.Equal(29050m, candles[1].Close.Amount);
        }

        [Fact]
        public void ParseUdf_MismatchedArrays_Rejected()
        {
            var json = "{ \"s\": \"ok\", \"t\": [1690000000], \"o\": [\"28000\",\"28100\"], \"h\": [\"29000\"], \"l\": [\"27900\"], \"c\": [\"28900\"], \"v\": [\"12.5\"] }";
            var market = TradingLab.Domain.Market.Market.BtcUsdt;
            Assert.False(NobitexResponseMapper.TryParseUdfHistoryToCandles(json, market, Timeframe.OneHour, Asset.USDT, out var _));
        }

        [Fact]
        public void ParseUdf_InvalidStatus_Rejected()
        {
            var json = "{ \"s\": \"no\", \"t\": [1690000000], \"o\": [\"28000\"], \"h\": [\"29000\"], \"l\": [\"27900\"], \"c\": [\"28900\"], \"v\": [\"12.5\"] }";
            var market = TradingLab.Domain.Market.Market.BtcUsdt;
            Assert.False(NobitexResponseMapper.TryParseUdfHistoryToCandles(json, market, Timeframe.OneHour, Asset.USDT, out var _));
        }

        [Fact]
        public void ParseUdf_InvalidNumeric_Rejected()
        {
            var json = "{ \"s\": \"ok\", \"t\": [1690000000], \"o\": [\"notnum\"], \"h\": [\"29000\"], \"l\": [\"27900\"], \"c\": [\"28900\"], \"v\": [\"12.5\"] }";
            var market = TradingLab.Domain.Market.Market.BtcUsdt;
            Assert.False(NobitexResponseMapper.TryParseUdfHistoryToCandles(json, market, Timeframe.OneHour, Asset.USDT, out var _));
        }
    }
}
