using TradingLab.Domain.Market;

namespace TradingLab.Application.Abstractions.MarketData
{
    public sealed record MarketHistoryResult(bool Success, MarketDataError? Error, Candle[]? Candles)
    {
        public static MarketHistoryResult FromSuccess(Candle[] candles) => new(true, null, candles);
        public static MarketHistoryResult FromError(MarketDataError error) => new(false, error, null);
    }
}
