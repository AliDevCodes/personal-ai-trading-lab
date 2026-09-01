using System;
using TradingLab.Domain.Market;

namespace TradingLab.Application.Abstractions.MarketData
{
    public sealed record MarketDataResult(bool Success, MarketDataError? Error, Price? CurrentPrice, Candle? LatestCandle)
    {
        public static MarketDataResult FromSuccess(Price currentPrice, Candle latest) => new(true, null, currentPrice, latest);
        public static MarketDataResult FromError(MarketDataError error) => new(false, error, null, null);
    }

    public enum MarketDataError
    {
        Unknown,
        Network,
        InvalidResponse,
        NotFound,
        ProviderUnavailable
    }
}
