MD-001 Phase 2 — Historical Candle Slice (ASP.NET API)

Status: Phase 2 feature milestone CLOSED. Post-Phase-2 engineering hardening (approved small set) is complete. Final Git synchronization remains — local commit 88ea064 is not yet on GitHub.

Phase 2 (implementation commit 0c31a42 — completed and runtime verified):
- Endpoint: GET /api/market-data/{symbol}/history?timeframe=1h&limit=100&to=<optional-unix-seconds>
- API DTOs: MarketHistoryDto and CandleDto
- Supported market: BTCUSDT -> Market.BtcUsdt (unsupported -> 404)
- Supported timeframe: 1h -> Timeframe.OneHour (unsupported -> 400)
- limit: optional, default 100, must be 1..1000 (invalid -> 400)
- to: optional Unix epoch seconds; invalid -> 400; omitted -> null passed to application
- Error mapping: Network/ProviderUnavailable -> 503, InvalidResponse -> 502, NotFound -> 404, unexpected -> 500

Post-Phase-2 hardening (feature scope unchanged):
- Hardening #1 — cleanup (commit 326ea43, pushed): removed the three template UnitTest1.cs files; corrected stale README implementation-state wording.
- Hardening #2 — host-level API verification (commit d7fa2e4, pushed): added Microsoft.AspNetCore.Mvc.Testing 10.0.11; exposed Program minimally for WebApplicationFactory; added one deterministic WebApplicationFactory host test; no buffering workaround; no external network; API test suite verified green.
- Hardening #3A — neutral UDF parser extraction (commit 88ea064, committed locally, NOT yet pushed): extracted only semantics-neutral shared UDF array validation and Unix timestamp conversion; preserved the divergent semantics of the two existing parser paths.

Current state:
- Post-Phase-2 engineering hardening completed for the approved small set; final synchronization/push remains for commit 88ea064.
- Full solution tests: PASS (44/44).

Deferred engineering backlog (not current work):
- Semantic unification of the two UDF parser paths (TryParseUdfHistory / TryParseUdfHistoryToCandles) — divergence deliberately preserved; requires explicit approval before starting.

Next milestone: Phase 3 — Historical Data UI + Chart (initial scope: BTC/USDT 1H)
