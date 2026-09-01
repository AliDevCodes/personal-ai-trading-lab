using System.Threading;
using System.Threading.Tasks;
using TradingLab.Application.Abstractions.MarketData;
using TradingLab.Domain.Market;

namespace TradingLab.Application.Modules.Market
{
    public interface IMarketDataService
    {
        Task<MarketDataResult> GetLatestAsync(TradingLab.Domain.Market.Market market, TradingLab.Domain.Market.Timeframe timeframe, CancellationToken cancellationToken = default);
        Task<MarketHistoryResult> GetHistoryAsync(TradingLab.Domain.Market.Market market, TradingLab.Domain.Market.Timeframe timeframe, int limit, System.DateTimeOffset? to = null, CancellationToken cancellationToken = default);
    }

    public class MarketDataService : IMarketDataService
    {
        private readonly IMarketDataProvider _provider;

        public MarketDataService(IMarketDataProvider provider)
        {
            _provider = provider;
        }

        public Task<MarketDataResult> GetLatestAsync(TradingLab.Domain.Market.Market market, TradingLab.Domain.Market.Timeframe timeframe, CancellationToken cancellationToken = default)
        {
            return _provider.GetLatestAsync(market, timeframe, cancellationToken);
        }

        public Task<MarketHistoryResult> GetHistoryAsync(TradingLab.Domain.Market.Market market, TradingLab.Domain.Market.Timeframe timeframe, int limit, System.DateTimeOffset? to = null, CancellationToken cancellationToken = default)
        {
            // Application-level validation for limit
            if (limit <= 0 || limit > 1000)
            {
                return System.Threading.Tasks.Task.FromResult(MarketHistoryResult.FromError(MarketDataError.InvalidResponse));
            }

            return _provider.GetHistoryAsync(market, timeframe, limit, to, cancellationToken);
        }
    }
}
