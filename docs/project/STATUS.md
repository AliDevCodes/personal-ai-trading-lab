Current phase: MD-001 Historical Candle Slice

Current milestone: Phase 2 CLOSED (feature milestone); post-Phase-2 engineering hardening (approved small set) completed

Completed:
- Domain primitives (Market, Timeframe, Price, Candle)
- Application service abstractions (IMarketDataService) and MarketDataService
- Infrastructure Nobitex provider and mapping
- API latest endpoint GET /api/market-data/{symbol}
- API history endpoint GET /api/market-data/{symbol}/history
- Unit and integration tests covering provider mapping and API behaviors
- Hardening #1 — cleanup: three template UnitTest1.cs files removed; stale README implementation-state wording corrected (commit 326ea43, pushed)
- Hardening #2 — host-level API verification: Microsoft.AspNetCore.Mvc.Testing 10.0.11, minimal public partial Program for WebApplicationFactory, one deterministic host test; no buffering workaround; no external network (commit d7fa2e4, pushed)
- Hardening #3A — neutral UDF parser extraction: shared UDF array validation and Unix timestamp conversion only; divergent parser-path semantics preserved (commit 88ea064, committed locally, NOT yet pushed)

Current task:
- Final Git synchronization: publish local commit 88ea064 to origin/main (main is ahead of origin/main by 1). No commit/push happens automatically.

Next milestone:
- Phase 3 — Historical Data UI + Chart (initial: BTC/USDT 1H)

Verification:
- Historical market-data capability: COMPLETED
- Historical API endpoint: COMPLETED and runtime-verified (Phase 2)
- Nobitex parser tests after #3A: PASS (12/12)
- Full solution tests after hardening: PASS (44/44)

Phase status:
- Phase 2: CLOSED as a feature milestone; feature scope unchanged by hardening. Implementation commit 0c31a42; hardening 326ea43 and d7fa2e4 pushed; hardening 88ea064 local-only, pending publish.

Deferred engineering backlog (not current work):
- Semantic unification of the two UDF parser paths (TryParseUdfHistory / TryParseUdfHistoryToCandles) — deliberately preserved; requires explicit approval to start.

Notes:
- Hardening #3A changed only Infrastructure (NobitexModels.cs) as an internal, behavior-preserving refactor.
- Domain/Application production logic was not modified for this slice or its hardening.
