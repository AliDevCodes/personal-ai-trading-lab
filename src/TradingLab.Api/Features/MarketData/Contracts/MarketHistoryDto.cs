using System;

namespace TradingLab.Api.Features.MarketData.Contracts
{
    public sealed record CandleDto(
        DateTimeOffset IntervalStart,
        decimal Open,
        decimal High,
        decimal Low,
        decimal Close,
        decimal Volume);

    public sealed record MarketHistoryDto(
        string Market,
        string Timeframe,
        CandleDto[] Candles);
}
