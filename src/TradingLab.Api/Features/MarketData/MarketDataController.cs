using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using TradingLab.Application.Modules.Market;
using TradingLab.Api.Features.MarketData.Contracts;
using TradingLab.Domain.Market;

namespace TradingLab.Api.Features.MarketData
{
    [ApiController]
    [Route("api/market-data")]
    public class MarketDataController : ControllerBase
    {
        private readonly IMarketDataService _service;

        public MarketDataController(IMarketDataService service)
        {
            _service = service;
        }

        [HttpGet("{symbol}")]
        public async Task<IActionResult> Get(string symbol, [FromQuery] string timeframe)
        {
            if (string.IsNullOrWhiteSpace(symbol)) return BadRequest();

            // Only support BTCUSDT for this slice
            Market market;
            if (string.Equals(symbol, "BTCUSDT", StringComparison.OrdinalIgnoreCase))
            {
                market = TradingLab.Domain.Market.Market.BtcUsdt;
            }
            else
            {
                return NotFound();
            }

            Timeframe tf;
            if (string.Equals(timeframe, "1h", StringComparison.OrdinalIgnoreCase))
            {
                tf = TradingLab.Domain.Market.Timeframe.OneHour;
            }
            else
            {
                return BadRequest();
            }

            try
            {
                var result = await _service.GetLatestAsync(market, tf);
                if (!result.Success)
                {
                    return result.Error switch
                    {
                        TradingLab.Application.Abstractions.MarketData.MarketDataError.Network => StatusCode(503),
                        TradingLab.Application.Abstractions.MarketData.MarketDataError.ProviderUnavailable => StatusCode(503),
                        TradingLab.Application.Abstractions.MarketData.MarketDataError.InvalidResponse => StatusCode(502),
                        TradingLab.Application.Abstractions.MarketData.MarketDataError.NotFound => NotFound(),
                        _ => StatusCode(500),
                    };
                }

                var dto = new MarketDataDto(
                    market.ToString(),
                    tf.Id,
                    result.CurrentPrice!.Amount,
                    result.CurrentPrice!.Quote.Code,
                    result.LatestCandle!.IntervalStart,
                    result.LatestCandle!.Open.Amount,
                    result.LatestCandle!.High.Amount,
                    result.LatestCandle!.Low.Amount,
                    result.LatestCandle!.Close.Amount,
                    result.LatestCandle!.Volume
                );

                return Ok(dto);
            }
            catch (OperationCanceledException)
            {
                return StatusCode(503);
            }
            catch (Exception ex)
            {
                // Do not expose exception details to the client. Return generic 500.
                _ = ex; // explicit use to avoid compiler warnings; keep details internal.
                return StatusCode(500);
            }
        }

        [HttpGet("{symbol}/history")]
        public async Task<IActionResult> History(string symbol, [FromQuery] string timeframe, [FromQuery] int? limit, [FromQuery] string? to)
        {
            if (string.IsNullOrWhiteSpace(symbol)) return BadRequest();

            // Only support BTCUSDT for this slice
            Market market;
            if (string.Equals(symbol, "BTCUSDT", StringComparison.OrdinalIgnoreCase))
            {
                market = TradingLab.Domain.Market.Market.BtcUsdt;
            }
            else
            {
                return NotFound();
            }

            Timeframe tf;
            if (string.Equals(timeframe, "1h", StringComparison.OrdinalIgnoreCase))
            {
                tf = TradingLab.Domain.Market.Timeframe.OneHour;
            }
            else
            {
                return BadRequest();
            }

            var lim = limit ?? 100;
            if (lim <= 0 || lim > 1000) return BadRequest();

            DateTimeOffset? toDt = null;
            if (!string.IsNullOrWhiteSpace(to))
            {
                if (!long.TryParse(to, out var unix)) return BadRequest();
                try
                {
                    toDt = DateTimeOffset.FromUnixTimeSeconds(unix);
                }
                catch
                {
                    return BadRequest();
                }
            }

            try
            {
                var result = await _service.GetHistoryAsync(market, tf, lim, toDt);
                if (!result.Success)
                {
                    return result.Error switch
                    {
                        TradingLab.Application.Abstractions.MarketData.MarketDataError.Network => StatusCode(503),
                        TradingLab.Application.Abstractions.MarketData.MarketDataError.ProviderUnavailable => StatusCode(503),
                        TradingLab.Application.Abstractions.MarketData.MarketDataError.InvalidResponse => StatusCode(502),
                        TradingLab.Application.Abstractions.MarketData.MarketDataError.NotFound => NotFound(),
                        _ => StatusCode(500),
                    };
                }

                var candles = result.Candles ?? Array.Empty<TradingLab.Domain.Market.Candle>();

                var dto = new Contracts.MarketHistoryDto(
                    market.ToString(),
                    tf.Id,
                    Array.ConvertAll(candles, c => new Contracts.CandleDto(
                        c.IntervalStart,
                        c.Open.Amount,
                        c.High.Amount,
                        c.Low.Amount,
                        c.Close.Amount,
                        c.Volume
                    ))
                );

                return Ok(dto);
            }
            catch (OperationCanceledException)
            {
                return StatusCode(503);
            }
            catch (Exception ex)
            {
                _ = ex;
                return StatusCode(500);
            }
        }
    }
}
