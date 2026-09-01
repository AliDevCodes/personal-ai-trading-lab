using System;

namespace TradingLab.Api.Features.MarketData.Contracts
{
    public sealed record MarketDataDto(
        string Market,
        string Timeframe,
        decimal CurrentPriceAmount,
        string CurrentPriceQuote,
        DateTimeOffset LatestCandleIntervalStart,
        decimal Open,
        decimal High,
        decimal Low,
        decimal Close,
        decimal Volume);
}
