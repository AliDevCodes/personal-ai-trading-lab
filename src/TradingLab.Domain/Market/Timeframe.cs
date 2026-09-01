namespace TradingLab.Domain.Market
{
    public sealed record Timeframe(string Id, long Minutes)
    {
        public static Timeframe OneHour => new("1h", 60);
    }
}
