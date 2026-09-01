using System;

namespace TradingLab.Domain.Market
{
    public sealed record Market(Asset Base, Asset Quote)
    {
        public static Market BtcUsdt => new(Asset.BTC, Asset.USDT);

        public override string ToString() => $"{Base.Code}/{Quote.Code}";
    }
}
