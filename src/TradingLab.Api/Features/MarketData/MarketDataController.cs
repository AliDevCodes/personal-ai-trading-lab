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
    }
}
