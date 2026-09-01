using TradingLab.Domain.Market;
using Xunit;

namespace TradingLab.UnitTests.Market
{
    public class MarketMappingTests
    {
        [Fact]
        public void BtcUsdt_ToString_IsCorrect()
        {
            var m = TradingLab.Domain.Market.Market.BtcUsdt;
            Assert.Equal("BTC/USDT", m.ToString());
        }
    }
}
