using System;
using TradingLab.Domain.Market;
using Xunit;

namespace TradingLab.UnitTests.Market
{
    public class PriceValidationTests
    {
        [Fact]
        public void Validate_Price_Negative_Throws()
        {
            var p = new Price(-1m, Asset.USDT);
            Assert.Throws<ArgumentOutOfRangeException>(() => p.Validate());
        }

        [Fact]
        public void Validate_Price_Zero_Throws()
        {
            var p = new Price(0m, Asset.USDT);
            Assert.Throws<ArgumentOutOfRangeException>(() => p.Validate());
        }
    }
}
