# Personal AI Trading Lab — Project Master Context v1.6

## Purpose

This is the durable cross-session context for AI assistants working on Personal AI Trading Lab.

The repository and its canonical documents are the sources of truth. Chat history, screenshots, and previous AI summaries are context only.

When information conflicts:
1. Actual repository files and Git state
2. Canonical project/trading documents
3. This file
4. Previous chat summaries

Do not silently reconcile conflicts. Investigate first.

---

## 1. Project identity

Project: Personal AI Trading Lab

Repository:
https://github.com/AliDevCodes/personal-ai-trading-lab

Local path:
E:\Projects\personal-ai-trading-lab

Primary branch:
main

Expected origin:
https://github.com/AliDevCodes/personal-ai-trading-lab

---

## 2. Product purpose

Personal AI-assisted trading/research laboratory for disciplined, explainable evaluation of selected market opportunities before real-money activity is considered.

V1 is paper trading only.

The product is intended to:
- monitor selected markets
- evaluate an explicit strategy hypothesis
- generate explainable signals
- use AI for reasoning, interpretation, explanation, and contextual analysis
- support long/short paper trading
- maintain journal and performance views
- provide basic in-app alerts

UI/UX quality criterion:
- UI/UX must be professional, modern, current, attractive, usable, and coherent at a level that can withstand review by a professional UI/UX specialist without obvious serious deficiencies. This complements the existing principle of calm, focused decision support and evidence-oriented workflows.

V1 explicitly excludes:
- real-money execution
- auto-trading
- external notifications
- mobile application
- microservices
- Kubernetes
- advanced agents/large-scale scanning
- advanced ML/fine-tuning

Canonical scope:
docs/product/vision-and-scope.md

Canonical roadmap:
docs/product/roadmap.md

---

## 3. Core safety and trading principles

- Capital preservation precedes profitability and growth.
- Deterministic financial logic is authoritative.
- AI is advisory only.
- AI must not mutate or override financial state.
- Strategy V1 remains a hypothesis until supported by evidence.
- V1 must not introduce live execution, leverage, margin, liquidation, or auto-trading paths.

---

## 4. Architecture

Architecture:
Modular Monolith + Clean Architecture

Backend projects:
- TradingLab.Domain
- TradingLab.Application
- TradingLab.Infrastructure
- TradingLab.Api

Frontend:
- web/trading-lab-web
- React + TypeScript + Vite

Tests:
- TradingLab.UnitTests
- TradingLab.IntegrationTests
- TradingLab.ApiTests

Dependency direction:
API -> Application -> Domain
API -> Infrastructure -> Application + Domain

Domain:
- deterministic concepts/invariants only
- no HTTP, ASP.NET, React, PostgreSQL, provider APIs, provider SDKs, or configuration infrastructure

Application:
- use cases
- orchestration
- application-facing ports/abstractions

Infrastructure:
- provider adapters
- provider-specific DTOs/mapping
- persistence when approved

API:
- HTTP contracts
- validation/mapping
- DI composition
- no business/financial logic

React:
- presentation
- interaction
- API consumption
- no risk/accounting/strategy authority

Canonical architecture:
docs/architecture/solution-blueprint.md

---

## 5. Engineering / AI-agent workflow

Before modification:
- git rev-parse --show-toplevel
- git remote get-url origin
- git branch --show-current
- git status --short

Expected origin must match the repository above.

Rules:
- respect docs/project/CURRENT.md
- stay within approved scope
- prefer small vertical slices
- avoid unrelated changes
- every behavior change gets tests
- do not introduce dependencies without justification
- do not redesign architecture during unrelated feature work
- never trust an AI agent's READY claim without independent verification
- never commit/push unless explicitly requested by the user

Preferred collaboration style:
- senior engineer / mentor mindset
- explain why, tradeoffs, edge cases, maintainability
- keep implementation practical and incremental
- use AI agents for reasoning and implementation
- use CLI for simple mechanical work to conserve agent quota
- inspect AI-generated diffs before commit

Canonical rules:
AGENTS.md

---

## 6. Trader profile

The canonical trader profile is:
docs/trading/trader-profile.md

Current confirmed profile:
- Capital: $500–$1,000
- Priority: capital preservation -> profitability -> capital growth
- Risk tolerance: low to moderate
- No leverage initially
- Preference: selective, high-quality setups
- Style: swing-biased hybrid
- Direction: long and short in paper trading
- Overnight positions: acceptable when risk is predefined

Behavioral hypotheses to validate, not assume:
- early-exit risk
- perfectionism / over-analysis
- drawdown sensitivity
- excessive strategy tweaking

Boundary:
TraderProfile is user context. It does not override deterministic risk rules.

Versioned operational assumptions belong to TradingConfiguration.

AI-friendly trader summary:
docs/trading/TRADER_PROFILE_CONTEXT.md

---

## 7. Current technology

Backend:
.NET 10 / ASP.NET Core

Frontend:
React 19 / TypeScript / Vite + Lightweight Charts 5.2.1 (candlestick charting)

Planned database:
PostgreSQL

Current crypto market-data provider:
Nobitex

Verified working production REST host:
https://apiv2.nobitex.ir/

Verified public endpoints:
- /market/stats
- /market/udf/history

Current verified market:
BTC/USDT

Current verified mapping:
BTC/USDT -> BTCUSDT

Current verified timeframe:
1h -> Nobitex resolution 60

Historical UDF requests use countback.
Historical candles are normalized to oldest -> newest.

---

## 8. Completed technical milestones

Completed and independently/runtime verified earlier:
- project foundation
- architecture/documentation foundation
- backend scaffold
- testing foundation
- React/Vite scaffold
- latest market-data capability
- latest market-data API endpoint
- React market-data UI
- real Nobitex integration
- Vite development proxy
- historical market-data Application/Infrastructure capability
- host-level API verification tests (WebApplicationFactory, .NET 10 aligned)
- neutral UDF parser consolidation (semantics preserved)
- historical data UI + chart (Phase 3.1 — BTC/USDT 1H, Lightweight Charts 5.2.1)

Real end-to-end path verified earlier:
Browser -> Vite proxy -> ASP.NET API -> Application -> Nobitex Infrastructure -> Nobitex

The Browser successfully displayed real BTC/USDT data.

---

## 9. Historical API status
Latest GitHub checkpoint (origin/main): 663605b

Checkpoint trail:
79a61e2 (historical market data) -> 0c31a42 (Phase 2 implementation) -> 326ea43 (Hardening #1) -> d7fa2e4 (Hardening #2) -> 88ea064 (Hardening #3A) -> 663605b (documentation sync) -> 67dba66 (Phase 3.1 historical data UI, local only)

Phase 2 implementation (MD-001 Phase 2):
- Local implementation commit: 0c31a42
- Classification: Reviewed and KEEP
- Milestone: CLOSED as a feature milestone

Post-Phase-2 hardening (feature scope unchanged):
- Hardening #1 — cleanup (326ea43, pushed): three template UnitTest1.cs files removed; stale README implementation-state wording corrected.
- Hardening #2 — host-level API verification (d7fa2e4, pushed): Microsoft.AspNetCore.Mvc.Testing 10.0.11; minimal public partial Program for WebApplicationFactory; one deterministic WebApplicationFactory host test; no buffering workaround; no external network.
- Hardening #3A — neutral UDF parser extraction (88ea064, pushed; included in checkpoint 663605b): semantics-neutral shared UDF array validation and Unix timestamp conversion extracted in NobitexModels.cs; divergent parser-path semantics preserved.

Implemented endpoint:
- GET /api/market-data/{symbol}/history?timeframe=1h&limit=100&to=<optional-unix-seconds>

Verified behavior:
- BTCUSDT -> Market.BtcUsdt (unsupported -> 404)
- 1h -> Timeframe.OneHour (unsupported -> 400)
- limit default 100, allowed range 1..1000 (invalid -> 400)
- optional to as Unix seconds; invalid -> 400; omitted -> null passed to application
- provider errors mapped to 503/502 as appropriate
- API DTOs prevent provider-type leakage

Verification summary:
- Fresh build: PASS
- Tests: 44/44 passing after post-Phase-2 hardening (46/46 at Phase 2 close; template tests removed in #1, one host test added in #2)
- Runtime verification: GET /api/market-data/BTCUSDT/history?timeframe=1h&limit=10 returned exactly 10 candles in chronological order (oldest → newest).

Phase status:
- Phase 2: CLOSED. Post-Phase-2 hardening completed for the approved small set and fully pushed; main and origin/main synchronized at 663605b.
- Phase 3.1 (Historical Data UI + Chart, BTC/USDT 1H): implemented, reviewed, committed locally at 67dba66; NOT yet pushed (origin/main still 663605b). No backend changes.
- Next milestone: Phase 3.2 / next approved historical-data UI work — only after the current checkpoint is published

Deferred engineering backlog (not current work; requires explicit approval):
- Semantic unification of the two UDF parser paths (TryParseUdfHistory / TryParseUdfHistoryToCandles) — divergence intentionally preserved in Hardening #3A.

---

## 10. Current recovery task

Immediate task:
- Phase 3.1 (Historical Data UI + Chart, BTC/USDT 1H) is implemented, reviewed, and committed locally at 67dba66. Pending action: standard Git push/synchronization (origin/main still at 663605b) — push happens only on explicit user request. Next milestone: Phase 3.2 / next approved historical-data UI work, only after the current checkpoint is published.

Notes:
- No destructive Git actions required. Do not perform destructive resets during handoff.
- Deferred engineering backlog (do not start without explicit approval): semantic unification of the two Nobitex UDF parser paths.

---

## 11. Development roadmap

High-level:
Foundation
-> latest market data
-> historical market data
-> historical UI / chart
-> market analysis
-> strategy hypothesis
-> signals
-> risk evaluation
-> paper trading
-> journal/performance
-> AI advisory
-> evidence/backtesting
-> later V2 research/notifications as separately approved

The first chart slice should stay narrow:
- BTC/USDT
- 1H
- historical candles
- minimal loading/empty/error states

Do not jump prematurely to a large dashboard, many markets, broad indicators, AI trading authority, or live execution.

---

## 12. Durable project-memory files

Engineering rules:
AGENTS.md

Project-wide stable context:
docs/project/MASTER_CONTEXT.md

Current exact checkpoint:
docs/project/CURRENT.md

Operational status:
docs/project/STATUS.md

Decision log:
docs/project/DECISIONS.md

Temporary session handoff:
docs/project/SESSION_HANDOFF.md

Canonical trading profile:
docs/trading/trader-profile.md

AI-friendly trader handoff:
docs/trading/TRADER_PROFILE_CONTEXT.md

Strategy hypothesis:
docs/trading/strategy-hypothesis.md

Risk rules:
docs/trading/risk-management.md

---

## 13. Update policy

Do NOT update every file after every small change.

Update MASTER_CONTEXT when:
- project direction changes
- architecture changes
- long-term workflow changes
- technology stack decisions change
- the stable project snapshot materially changes

Update CURRENT.md when:
- the exact active task/checkpoint changes
- a milestone starts or ends
- the next gate changes

Update STATUS.md when:
- the overall implementation state changes materially
- completed capabilities change
- the project status needs synchronization with reality

Update SESSION_HANDOFF.md:
- at the end of a meaningful development session
- whenever a chat is likely to be abandoned/replaced
- whenever there is an unresolved local-vs-GitHub state

Update TRADER_PROFILE_CONTEXT.md:
- only when confirmed trader-profile facts/preferences change
- or when the AI-facing interpretation/summary materially changes
- never silently change confirmed profile facts

Update DECISIONS.md:
- only for consequential architecture/product/trading decisions
- record the decision, rationale, alternatives, and consequence

When a meaningful update occurs:
1. modify the appropriate durable files
2. verify internal consistency
3. run relevant tests/checks
4. include the documentation change in the same logical commit when appropriate

Do not rewrite stable history unnecessarily.

---

## 14. Multi-agent model

Multiple specialized AI agents are allowed, but they must share:
- one repository
- Git history
- canonical documentation
- project context

Suggested roles:
- Architecture
- Backend/.NET
- Frontend/React/UIUX
- Data/PostgreSQL
- Trading Research/Strategy
- QA/Verification

Agents are maintainers/reviewers, not independent sources of truth.

Synchronize through:
- Git
- CURRENT.md
- STATUS.md
- MASTER_CONTEXT.md
- DECISIONS.md
- SESSION_HANDOFF.md

No agent should assume another agent's chat context.

---

## 15. Git discipline

Preferred lifecycle:
working tree
-> git add
-> staged diff review
-> git commit
-> git push

Before commit:
- git diff --cached --check
- git diff --cached --stat
- inspect important staged diffs

Push only publishes committed history.

Never commit/push automatically.

---

## 16. Documentation governance

The canonical rules for maintaining project memory are defined in:

docs/project/DOCUMENTATION_GOVERNANCE.md

The active AI assistant is responsible for:
- detecting when durable documentation needs an update
- updating the smallest appropriate set of documents
- keeping CURRENT/STATUS/MASTER/SESSION_HANDOFF and trading context synchronized
- documenting consequential decisions
- avoiding duplicate/version-suffixed canonical files
- treating Git history as the version history

Do not wait for the user to specify which project-memory file needs updating when the trigger is obvious.

## 17. Session continuity

A new AI session should first read:
1. AGENTS.md
2. docs/project/CURRENT.md
3. docs/project/STATUS.md
4. docs/project/MASTER_CONTEXT.md
5. docs/trading/TRADER_PROFILE_CONTEXT.md
6. relevant strategy/risk/module docs
7. current Git status/history

Then independently verify the repository state.

A previous chat summary is never enough to authorize a change.

---

## End
