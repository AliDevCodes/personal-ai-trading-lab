using System;
using TradingLab.Domain.Market;
using Xunit;

namespace TradingLab.UnitTests.Market
{
    public class CandleValidationTests
    {
        [Fact]
        public void Validate_Candle_WithBadHighLow_Throws()
        {
            var market = TradingLab.Domain.Market.Market.BtcUsdt;
            var tf = Timeframe.OneHour;
            var open = new Price(100m, Asset.USDT);
            var high = new Price(90m, Asset.USDT);
            var low = new Price(95m, Asset.USDT);
            var close = new Price(92m, Asset.USDT);
            var candle = new Candle(market, tf, DateTimeOffset.UtcNow, open, high, low, close, 1m);
            Assert.Throws<ArgumentException>(() => candle.Validate());
        }
    }
}
