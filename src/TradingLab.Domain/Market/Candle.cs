using System;

namespace TradingLab.Domain.Market
{
    public sealed record Candle
    (
        Market Market,
        Timeframe Timeframe,
        DateTimeOffset IntervalStart,
        Price Open,
        Price High,
        Price Low,
        Price Close,
        decimal Volume
    )
    {
        public Candle Validate()
        {
            if (Market is null) throw new ArgumentNullException(nameof(Market));
            if (Timeframe is null) throw new ArgumentNullException(nameof(Timeframe));
            if (IntervalStart == default) throw new ArgumentException("IntervalStart must be set", nameof(IntervalStart));
            if (Open is null || High is null || Low is null || Close is null) throw new ArgumentException("Price values must be set");
            // Ensure prices are for the same quote asset
            var quote = Open.Quote;
            if (High.Quote.Code != quote.Code || Low.Quote.Code != quote.Code || Close.Quote.Code != quote.Code)
                throw new ArgumentException("Candle prices must share the same quote asset");
            // Numeric invariants
            if (High.Amount < Low.Amount) throw new ArgumentException("High cannot be less than Low");
            if (High.Amount < Open.Amount) throw new ArgumentException("High cannot be less than Open");
            if (High.Amount < Close.Amount) throw new ArgumentException("High cannot be less than Close");
            if (Low.Amount > Open.Amount) throw new ArgumentException("Low cannot be greater than Open");
            if (Low.Amount > Close.Amount) throw new ArgumentException("Low cannot be greater than Close");
            if (Open.Amount <= 0 || High.Amount <= 0 || Low.Amount <= 0 || Close.Amount <= 0) throw new ArgumentException("Prices must be positive");
            if (Volume < 0) throw new ArgumentException("Volume cannot be negative");
            return this;
        }
    }
}
