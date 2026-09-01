using System;
using System.Globalization;
using System.Text.Json;
using TradingLab.Domain.Market;

namespace TradingLab.Infrastructure.MarketData.Nobitex
{
    internal static class NobitexResponseMapper
    {
        // Parse /market/stats response defensively and extract latest price
        public static bool TryParseStats(string json, Market market, Asset quote, out Price price)
        {
            price = null!;
            try
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                // Must be an object with status and stats
                if (root.ValueKind != JsonValueKind.Object) return false;

                // optional: check overall status
                if (root.TryGetProperty("status", out var statusEl) && statusEl.ValueKind == JsonValueKind.String)
                {
                    var s = statusEl.GetString();
                    if (!string.Equals(s, "ok", StringComparison.OrdinalIgnoreCase)) return false;
                }

                if (!root.TryGetProperty("stats", out var statsEl) || statsEl.ValueKind != JsonValueKind.Object) return false;

                // derive provider key for market, e.g., btc-usdt
                var key = $"{market.Base.Code.ToLowerInvariant()}-{market.Quote.Code.ToLowerInvariant()}";
                if (!statsEl.TryGetProperty(key, out var marketStats) || marketStats.ValueKind != JsonValueKind.Object) return false;

                if (!marketStats.TryGetProperty("latest", out var latestEl)) return false;
                if (!TryGetDecimalValue(latestEl, out var latestVal)) return false;

                price = new Price(latestVal, quote);
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Parse UDF history response with arrays t[], o[], h[], l[], c[], v[]
        public static bool TryParseUdfHistory(string json, Market market, Timeframe timeframe, Asset quote, out Candle candle)
        {
            candle = null!;
            try
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                if (root.ValueKind != JsonValueKind.Object) return false;

                // Some UDF responses include a status field like s: "ok" - reject non-ok
                if (root.TryGetProperty("s", out var sprop) && sprop.ValueKind == JsonValueKind.String)
                {
                    var s = sprop.GetString();
                    if (!string.Equals(s, "ok", StringComparison.OrdinalIgnoreCase)) return false;
                }
                if (root.TryGetProperty("status", out var statusEl) && statusEl.ValueKind == JsonValueKind.String)
                {
                    var st = statusEl.GetString();
                    if (!string.Equals(st, "ok", StringComparison.OrdinalIgnoreCase)) return false;
                }

                if (!root.TryGetProperty("t", out var tArr) || tArr.ValueKind != JsonValueKind.Array) return false;
                if (!root.TryGetProperty("o", out var oArr) || oArr.ValueKind != JsonValueKind.Array) return false;
                if (!root.TryGetProperty("h", out var hArr) || hArr.ValueKind != JsonValueKind.Array) return false;
                if (!root.TryGetProperty("l", out var lArr) || lArr.ValueKind != JsonValueKind.Array) return false;
                if (!root.TryGetProperty("c", out var cArr) || cArr.ValueKind != JsonValueKind.Array) return false;
                if (!root.TryGetProperty("v", out var vArr) || vArr.ValueKind != JsonValueKind.Array) return false;

                var len = tArr.GetArrayLength();
                if (len == 0) return false;
                if (oArr.GetArrayLength() != len || hArr.GetArrayLength() != len || lArr.GetArrayLength() != len || cArr.GetArrayLength() != len || vArr.GetArrayLength() != len)
                    return false;

                // take last aligned index
                var idx = len - 1;

                if (!TryGetDecimalValue(oArr[idx], out var o)) return false;
                if (!TryGetDecimalValue(hArr[idx], out var h)) return false;
                if (!TryGetDecimalValue(lArr[idx], out var l)) return false;
                if (!TryGetDecimalValue(cArr[idx], out var cval)) return false;
                if (!TryGetDecimalValue(vArr[idx], out var v)) v = 0m;

                // timestamp
                DateTimeOffset ts = DateTimeOffset.UtcNow;
                var tprop = tArr[idx];
                if (tprop.ValueKind == JsonValueKind.Number && tprop.TryGetInt64(out var unix))
                {
                    if (unix > 9999999999) ts = DateTimeOffset.FromUnixTimeMilliseconds(unix);
                    else ts = DateTimeOffset.FromUnixTimeSeconds(unix);
                }
                else if (tprop.ValueKind == JsonValueKind.String && long.TryParse(tprop.GetString(), out var unix2))
                {
                    if (unix2 > 9999999999) ts = DateTimeOffset.FromUnixTimeMilliseconds(unix2);
                    else ts = DateTimeOffset.FromUnixTimeSeconds(unix2);
                }

                var open = new Price(o, quote);
                var high = new Price(h, quote);
                var low = new Price(l, quote);
                var close = new Price(cval, quote);

                candle = new Candle(market, timeframe, ts, open, high, low, close, v);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static bool TryGetDecimalValue(JsonElement el, out decimal value)
        {
            value = 0m;
            if (el.ValueKind == JsonValueKind.Number && el.TryGetDecimal(out value)) return true;
            if (el.ValueKind == JsonValueKind.String)
            {
                var s = el.GetString();
                if (decimal.TryParse(s, NumberStyles.Number | NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out value)) return true;
            }
            return false;
        }
    }
}
