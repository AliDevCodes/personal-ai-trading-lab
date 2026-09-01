MD-001 Phase 2 — Historical Candle Slice (ASP.NET API)

Status: Completed and committed locally in this branch.

What was added:
- Endpoint: GET /api/market-data/{symbol}/history?timeframe=1h&limit=100&to=<optional-unix-seconds>
- API DTOs: MarketHistoryDto and CandleDto

Behavior summary:
- Supported market: BTCUSDT -> Market.BtcUsdt (unsupported -> 404)
- Supported timeframe: 1h -> Timeframe.OneHour (unsupported -> 400)
- limit: optional, default 100, must be 1..1000 (invalid -> 400)
- to: optional Unix epoch seconds; invalid -> 400; omitted -> null passed to application
- Error mapping: Network/ProviderUnavailable -> 503, InvalidResponse -> 502, NotFound -> 404, unexpected -> 500

Verification: dotnet build and dotnet test run successfully; all API tests pass locally.
