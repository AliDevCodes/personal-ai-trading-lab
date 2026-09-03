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

                if (!TryGetUdfArrays(root, out var tArr, out var oArr, out var hArr, out var lArr, out var cArr, out var vArr, out var len)) return false;

                // take last aligned index
                var idx = len - 1;

                if (!TryGetDecimalValue(oArr[idx], out var o)) return false;
                if (!TryGetDecimalValue(hArr[idx], out var h)) return false;
                if (!TryGetDecimalValue(lArr[idx], out var l)) return false;
                if (!TryGetDecimalValue(cArr[idx], out var cval)) return false;
                if (!TryGetDecimalValue(vArr[idx], out var v)) v = 0m;

                // timestamp: lenient policy preserved -- an unparsable timestamp
                // falls back to UtcNow rather than rejecting the row
                DateTimeOffset ts = DateTimeOffset.UtcNow;
                if (TryConvertUnixTimestamp(tArr[idx], out var parsedTs))
                {
                    ts = parsedTs;
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

        // Parse UDF history into multiple candles (oldest -> newest)
        public static bool TryParseUdfHistoryToCandles(string json, Market market, Timeframe timeframe, Asset quote, out Candle[] candles)
        {
            candles = null!;
            try
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                // status must be ok
                if (root.TryGetProperty("s", out var sprop) && sprop.ValueKind == JsonValueKind.String)
                {
                    if (!string.Equals(sprop.GetString(), "ok", StringComparison.OrdinalIgnoreCase)) return false;
                }
                else if (root.TryGetProperty("status", out var statusEl) && statusEl.ValueKind == JsonValueKind.String)
                {
                    if (!string.Equals(statusEl.GetString(), "ok", StringComparison.OrdinalIgnoreCase)) return false;
                }

                if (!TryGetUdfArrays(root, out var tArr, out var oArr, out var hArr, out var lArr, out var cArr, out var vArr, out var len)) return false;

                var list = new System.Collections.Generic.List<Candle>(len);

                for (var i = 0; i < len; i++)
                {
                    if (!TryGetDecimalValue(oArr[i], out var o)) return false;
                    if (!TryGetDecimalValue(hArr[i], out var h)) return false;
                    if (!TryGetDecimalValue(lArr[i], out var l)) return false;
                    if (!TryGetDecimalValue(cArr[i], out var cval)) return false;
                    if (!TryGetDecimalValue(vArr[i], out var v)) v = 0m;

                    // timestamp: strict policy preserved -- an unparsable timestamp
                    // rejects the whole payload
                    if (!TryConvertUnixTimestamp(tArr[i], out var ts))
                    {
                        return false;
                    }

                    var open = new Price(o, quote);
                    var high = new Price(h, quote);
                    var low = new Price(l, quote);
                    var close = new Price(cval, quote);

                    // construct and validate candle
                    Candle c;
                    try
                    {
                        c = new Candle(market, timeframe, ts, open, high, low, close, v).Validate();
                    }
                    catch
                    {
                        return false;
                    }

                    list.Add(c);
                }

                // Ensure chronological order oldest -> newest; if not monotonic, sort
                var ordered = list;
                bool monotonic = true;
                for (int i = 1; i < ordered.Count; i++)
                {
                    if (ordered[i].IntervalStart <= ordered[i - 1].IntervalStart)
                    {
                        monotonic = false;
                        break;
                    }
                }
                if (!monotonic)
                {
                    ordered = new System.Collections.Generic.List<Candle>(list);
                    ordered.Sort((a, b) => a.IntervalStart.CompareTo(b.IntervalStart));
                }

                candles = ordered.ToArray();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Extract the six UDF arrays (t, o, h, l, c, v) and validate that they are
        // arrays, non-empty, and all of equal length. Pure structural validation with
        // no semantic policy of its own.
        private static bool TryGetUdfArrays(JsonElement root,
            out JsonElement tArr, out JsonElement oArr, out JsonElement hArr,
            out JsonElement lArr, out JsonElement cArr, out JsonElement vArr,
            out int length)
        {
            tArr = default;
            oArr = default;
            hArr = default;
            lArr = default;
            cArr = default;
            vArr = default;
            length = 0;

            if (root.ValueKind != JsonValueKind.Object) return false;

            if (!root.TryGetProperty("t", out tArr) || tArr.ValueKind != JsonValueKind.Array) return false;
            if (!root.TryGetProperty("o", out oArr) || oArr.ValueKind != JsonValueKind.Array) return false;
            if (!root.TryGetProperty("h", out hArr) || hArr.ValueKind != JsonValueKind.Array) return false;
            if (!root.TryGetProperty("l", out lArr) || lArr.ValueKind != JsonValueKind.Array) return false;
            if (!root.TryGetProperty("c", out cArr) || cArr.ValueKind != JsonValueKind.Array) return false;
            if (!root.TryGetProperty("v", out vArr) || vArr.ValueKind != JsonValueKind.Array) return false;

            var len = tArr.GetArrayLength();
            if (len == 0) return false;
            if (oArr.GetArrayLength() != len || hArr.GetArrayLength() != len || lArr.GetArrayLength() != len || cArr.GetArrayLength() != len || vArr.GetArrayLength() != len)
                return false;

            length = len;
            return true;
        }

        // Convert a UDF timestamp element (Unix seconds, or milliseconds when the
        // value exceeds the 9999999999 threshold) to DateTimeOffset. Only reports
        // whether the element is an interpretable integral Unix timestamp; what to
        // do on failure (fallback vs reject) is the caller's policy.
        private static bool TryConvertUnixTimestamp(JsonElement element, out DateTimeOffset timestamp)
        {
            timestamp = default;
            if (element.ValueKind == JsonValueKind.Number && element.TryGetInt64(out var unix))
            {
                timestamp = unix > 9999999999 ? DateTimeOffset.FromUnixTimeMilliseconds(unix) : DateTimeOffset.FromUnixTimeSeconds(unix);
                return true;
            }
            if (element.ValueKind == JsonValueKind.String && long.TryParse(element.GetString(), out var unixAsText))
            {
                timestamp = unixAsText > 9999999999 ? DateTimeOffset.FromUnixTimeMilliseconds(unixAsText) : DateTimeOffset.FromUnixTimeSeconds(unixAsText);
                return true;
            }
            return false;
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
