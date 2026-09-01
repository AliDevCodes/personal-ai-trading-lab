using System;

namespace TradingLab.Domain.Market
{
    public sealed record Price(decimal Amount, Asset Quote)
    {
        public Price Validate()
        {
            if (Amount <= 0) throw new ArgumentOutOfRangeException(nameof(Amount), "Price amount must be positive");
            if (Quote is null) throw new ArgumentNullException(nameof(Quote));
            return this;
        }
    }
}
