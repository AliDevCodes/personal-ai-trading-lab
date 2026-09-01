Current phase: MD-001 Historical Candle Slice

Current milestone: Phase 2 complete — ASP.NET API endpoint for historical candles

Completed:
- Domain primitives (Market, Timeframe, Price, Candle)
- Application service abstractions (IMarketDataService) and MarketDataService
- Infrastructure Nobitex provider and mapping
- API latest endpoint GET /api/market-data/{symbol}
- API history endpoint GET /api/market-data/{symbol}/history
- Unit and integration tests covering provider mapping and API behaviors

Current task:
- Open PR for review and integration testing (this repository contains the committed changes)

Next milestone:
- Integrate API history endpoint with frontend and run end-to-end verification

Notes:
- Domain/Application/Infrastructure production logic was not modified for this slice.
