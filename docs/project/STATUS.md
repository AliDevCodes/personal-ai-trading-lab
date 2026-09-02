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

Next milestone:
- Historical Data UI + Chart (initial: BTC/USDT 1H)

Verification:
- Historical market-data capability: COMPLETED
- Historical API endpoint: COMPLETED and runtime-verified
- Tests: 46/46 passing

Phase status:
- Phase 2 closed pending Git closeout (implementation present: commit 0c31a42)

Details:
- Historical market data capability: COMPLETE
- Historical API endpoint: COMPLETE and runtime-verified
- Tests: 46/46 passing
- Phase 2: complete, pending Git closeout only

Notes:
- Domain/Application/Infrastructure production logic was not modified for this slice.
