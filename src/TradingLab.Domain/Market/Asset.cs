namespace TradingLab.Domain.Market
{
    public sealed record Asset(string Code)
    {
        public static Asset USD => new("USD");
        public static Asset USDT => new("USDT");
        public static Asset BTC => new("BTC");
    }
}
