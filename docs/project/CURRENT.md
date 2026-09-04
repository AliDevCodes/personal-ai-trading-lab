MD-001 Phase 3 — Historical Data UI + Chart (BTC/USDT 1H)

Status: Phase 3 implemented, reviewed, and verified. Phase 3.1 (chart) is committed and published (67dba66, checkpoint close cd2891c; origin/main at cd2891c). Phase 3.2 (candle inspection) is committed locally at 094e489 — the current local feature checkpoint — and is NOT yet pushed; pending standard Git push.

Implemented:
- BTC/USDT 1H historical candlestick chart + volume
- Inspected-candle interaction (desktop crosshair hover, mobile touch)
- Refresh inspection reset (inspected panel + chart crosshair cleared)
- Loading, success, empty, and error states
- Refresh with safe abort handling
- Responsive light/dark UI
- Accessible chart labeling; inspected-candle details as real DOM text
- Required TradingView attribution
- Real Nobitex-backed historical data
- No backend changes

Verification:
- Frontend build: PASS
- Frontend lint: PASS
- Browser verification: PASS
- Visual/product review: APPROVED

Phase 2: CLOSED.
Post-Phase-2 hardening: CLOSED.

Deferred to later milestones (requires explicit approval):
- Realtime/live market data
- PostgreSQL/persistence
- Additional markets and timeframes
- Indicators, analysis, strategy, signals, risk, paper trading, AI

Deferred engineering backlog:
- Semantic unification of the two UDF parser paths; requires explicit approval.

Next milestone:
Next approved post-Phase-3 roadmap item (e.g., market analysis) — only after 094e489 is published.