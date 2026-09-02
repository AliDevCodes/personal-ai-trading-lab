MD-001 Phase 2 — Historical Candle Slice (ASP.NET API)

Status: MD-001 Phase 2 completed and verified.

What was added:
- Endpoint: GET /api/market-data/{symbol}/history?timeframe=1h&limit=100&to=<optional-unix-seconds>
- API DTOs: MarketHistoryDto and CandleDto

Behavior summary:
- Supported market: BTCUSDT -> Market.BtcUsdt (unsupported -> 404)
- Supported timeframe: 1h -> Timeframe.OneHour (unsupported -> 400)
- limit: optional, default 100, must be 1..1000 (invalid -> 400)
- to: optional Unix epoch seconds; invalid -> 400; omitted -> null passed to application
- Error mapping: Network/ProviderUnavailable -> 503, InvalidResponse -> 502, NotFound -> 404, unexpected -> 500

Verification summary:
- Local implementation commit: 0c31a42 (reviewed and classified KEEP)
- Fresh build: PASS
- Fresh tests: PASS (46/46)
- Runtime verification: GET /api/market-data/BTCUSDT/history?timeframe=1h&limit=10 returned exactly 10 candles in chronological order (oldest → newest).

Next milestone: Phase 3 — Historical Data UI + Chart (initial: BTC/USDT 1H)
