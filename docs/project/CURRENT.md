MD-001 Phase 3.1 — Historical Data UI + Chart (BTC/USDT 1H)

Status: Phase 3.1 implemented, reviewed, and committed locally at 67dba66. Pending standard Git push/synchronization. origin/main is 663605b.

Implemented:
- Lightweight Charts 5.2.1
- Typed frontend historical market-data API client
- BTC/USDT 1H candlestick chart + volume
- Loading, success, empty, and error states
- Refresh with safe abort handling
- Responsive light/dark UI
- Accessible chart labeling
- Required TradingView attribution
- Removed unused Vite/React starter assets
- No backend changes

Verification:
- Frontend build: PASS
- Frontend lint: PASS
- Browser verification: PASS
- Visual review: APPROVED for this slice

Phase 2: CLOSED.
Post-Phase-2 hardening: CLOSED.
Deferred: semantic unification of the two UDF parser paths; requires explicit approval.

Next milestone:
Phase 3.2 — only after 67dba66 is published.