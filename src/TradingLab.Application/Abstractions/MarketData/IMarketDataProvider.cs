using System.Threading;
using System.Threading.Tasks;
using TradingLab.Domain.Market;

namespace TradingLab.Application.Abstractions.MarketData
{
    public interface IMarketDataProvider
    {
        Task<MarketDataResult> GetLatestAsync(Market market, Timeframe timeframe, CancellationToken cancellationToken = default);
    }
}
