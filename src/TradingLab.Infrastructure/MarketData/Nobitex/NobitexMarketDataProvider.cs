using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TradingLab.Application.Abstractions.MarketData;
using TradingLab.Domain.Market;

namespace TradingLab.Infrastructure.MarketData.Nobitex
{
    public class NobitexMarketDataProvider : IMarketDataProvider
    {
        private readonly HttpClient _httpClient;
        private readonly Uri _baseUri;

        public NobitexMarketDataProvider(HttpClient httpClient, string? baseUrl = null)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _baseUri = new Uri(baseUrl ?? "https://apiv2.nobitex.ir/");
        }

        public async Task<MarketDataResult> GetLatestAsync(Market market, Timeframe timeframe, CancellationToken cancellationToken = default)
        {
            // Map market to provider symbol
            var symbol = MapMarketToSymbol(market);
            if (symbol is null) return MarketDataResult.FromError(MarketDataError.NotFound);

            try
            {
                // Stats endpoint for current price
                var statsUrl = new Uri(_baseUri, $"market/stats?srcCurrency={market.Base.Code.ToLowerInvariant()}&dstCurrency={market.Quote.Code.ToLowerInvariant()}");
                var statsResp = await _httpClient.GetAsync(statsUrl, cancellationToken).ConfigureAwait(false);
                if (!statsResp.IsSuccessStatusCode) return MarketDataResult.FromError(MarketDataError.ProviderUnavailable);
                var statsContent = await statsResp.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                if (!NobitexResponseMapper.TryParseStats(statsContent, market, market.Quote, out var price))
                {
                    return MarketDataResult.FromError(MarketDataError.InvalidResponse);
                }

                // Validate price using domain validation
                try
                {
                    price.Validate();
                }
                catch
                {
                    return MarketDataResult.FromError(MarketDataError.InvalidResponse);
                }

                // UDF history for candles
                var resolution = MapTimeframeToResolution(timeframe);
                if (resolution is null) return MarketDataResult.FromError(MarketDataError.InvalidResponse);

                var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                var from = DateTimeOffset.UtcNow.AddHours(-2).ToUnixTimeSeconds();
                var udfUrl = new Uri(_baseUri, $"market/udf/history?symbol={symbol}&resolution={resolution}&from={from}&to={now}");
                var udfResp = await _httpClient.GetAsync(udfUrl, cancellationToken).ConfigureAwait(false);
                if (!udfResp.IsSuccessStatusCode) return MarketDataResult.FromError(MarketDataError.ProviderUnavailable);
                var udfContent = await udfResp.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                if (!NobitexResponseMapper.TryParseUdfHistory(udfContent, market, timeframe, market.Quote, out var candle))
                {
                    return MarketDataResult.FromError(MarketDataError.InvalidResponse);
                }

                // Validate candle invariants
                try
                {
                    candle.Validate();
                }
                catch
                {
                    return MarketDataResult.FromError(MarketDataError.InvalidResponse);
                }

                return MarketDataResult.FromSuccess(price, candle);
            }
            catch (HttpRequestException)
            {
                return MarketDataResult.FromError(MarketDataError.Network);
            }
            catch (OperationCanceledException)
            {
                // preserve cancellation semantics
                throw;
            }
            catch
            {
                return MarketDataResult.FromError(MarketDataError.Unknown);
            }
        }

        public async Task<MarketHistoryResult> GetHistoryAsync(Market market, Timeframe timeframe, int limit, DateTimeOffset? to = null, CancellationToken cancellationToken = default)
        {
            var symbol = MapMarketToSymbol(market);
            if (symbol is null) return MarketHistoryResult.FromError(MarketDataError.NotFound);

            var resolution = MapTimeframeToResolution(timeframe);
            if (resolution is null) return MarketHistoryResult.FromError(MarketDataError.InvalidResponse);

            try
            {
                var toUnix = (to ?? DateTimeOffset.UtcNow).ToUnixTimeSeconds();
                var udfUrl = new Uri(_baseUri, $"market/udf/history?symbol={symbol}&resolution={resolution}&countback={limit}&to={toUnix}");
                var udfResp = await _httpClient.GetAsync(udfUrl, cancellationToken).ConfigureAwait(false);
                if (!udfResp.IsSuccessStatusCode) return MarketHistoryResult.FromError(MarketDataError.ProviderUnavailable);
                var udfContent = await udfResp.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                if (!NobitexResponseMapper.TryParseUdfHistoryToCandles(udfContent, market, timeframe, market.Quote, out var candles))
                {
                    return MarketHistoryResult.FromError(MarketDataError.InvalidResponse);
                }

                return MarketHistoryResult.FromSuccess(candles);
            }
            catch (HttpRequestException)
            {
                return MarketHistoryResult.FromError(MarketDataError.Network);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch
            {
                return MarketHistoryResult.FromError(MarketDataError.Unknown);
            }
        }

        internal static string? MapMarketToSymbol(Market market)
        {
            if (market.Base.Code == "BTC" && market.Quote.Code == "USDT") return "BTCUSDT";
            return null;
        }

        internal static string? MapTimeframeToResolution(Timeframe timeframe)
        {
            if (timeframe == Timeframe.OneHour) return "60";
            return null;
        }
    }
}
