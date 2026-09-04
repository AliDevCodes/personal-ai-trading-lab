Current phase: MD-001 Historical Candle Slice

Current milestone:
Phase 3 CLOSED — Historical Data UI + Chart implemented, reviewed, and verified. Phase 3.1 committed and published (67dba66, cd2891c). Phase 3.2 candle inspection committed locally at 094e489; pending standard Git push.

Completed:
- Historical API
- Post-Phase-2 hardening #1/#2/#3A
- Phase 3.1 Historical Data UI + Chart
- Phase 3.2 candle inspection (desktop hover, mobile touch)
- Refresh inspection reset
- Lightweight Charts 5.2.1
- Candlestick + volume
- Typed API client
- Loading/success/empty/error
- Refresh
- Responsive + light/dark
- Accessibility
- TradingView attribution
- Real Nobitex-backed data
- No backend changes

Verification:
- Build: PASS
- Lint: PASS
- Browser verification: PASS
- Visual/product review: APPROVED

Current task:
- Standard Git push of local commit 094e489 (on explicit request). No other implementation work pending for Phase 3.

Next milestone:
Next approved post-Phase-3 roadmap item (e.g., market analysis) — after 094e489 is published. No Phase 3 expansion (realtime, PostgreSQL, additional markets/timeframes) without explicit approval.

Deferred:
- Realtime/live market data, PostgreSQL/persistence, additional markets, additional timeframes, indicators, analysis, strategy, signals, risk, paper trading, AI.
- Semantic UDF parser unification; requires explicit approval.