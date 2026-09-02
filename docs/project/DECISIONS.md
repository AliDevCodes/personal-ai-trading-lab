# Decisions Log

## Purpose

Compact record of consequential project/product/architecture decisions.

Do not use this file as a dumping ground for ordinary implementation details.

---

## D-001 — Modular monolith with Clean Architecture

Status: Approved

Decision:
Use one modular monolith with four backend projects:
- API
- Application
- Domain
- Infrastructure

Rationale:
Keep explicit dependency direction and module boundaries without premature service decomposition.

Canonical:
docs/architecture/solution-blueprint.md

---

## D-002 — AI is advisory, not authoritative

Status: Approved

Decision:
AI may provide reasoning, interpretation, explanation, and contextual analysis only.

Deterministic logic remains authoritative over:
- risk
- sizing
- accounting
- P&L
- paper-trading state

Rationale:
Maintain inspectability and prevent probabilistic model output from becoming financial-state authority.

Canonical:
AGENTS.md
docs/product/vision-and-scope.md

---

## D-003 — Paper trading only in V1

Status: Approved

Decision:
V1 must not introduce real-money execution or auto-trading.

Rationale:
The product must validate strategy and workflow through evidence before any broader decision.

Canonical:
docs/product/vision-and-scope.md
AGENTS.md

---

## D-004 — Nobitex behind IMarketDataProvider

Status: Approved

Decision:
Application owns the market-data port; Nobitex-specific integration stays in Infrastructure.

Rationale:
Prevent provider coupling from leaking into Domain/Application/Strategy.

---

## D-005 — Historical candles use provider countback

Status: Approved

Decision:
Historical candle requests use Nobitex UDF countback rather than relying primarily on manually calculated from ranges.

Rationale:
The request semantics map naturally to "last N candles" and keep provider-specific behavior inside Infrastructure.

---

## D-006 — TraderProfile separated from TradingConfiguration

Status: Approved

Decision:
TraderProfile contains user context/preferences/behavioral hypotheses.
TradingConfiguration contains versioned operational assumptions.

Rationale:
User context must not become an implicit override for deterministic financial rules.

Canonical:
docs/trading/trader-profile.md

---

## D-007 — Durable project memory lives in repository artifacts

Status: Approved

Decision:
Use version-controlled Markdown as durable AI project memory.

Core files:
- AGENTS.md
- docs/project/MASTER_CONTEXT.md
- docs/project/CURRENT.md
- docs/project/STATUS.md
- docs/project/DECISIONS.md
- docs/project/SESSION_HANDOFF.md
- docs/trading/trader-profile.md
- docs/trading/TRADER_PROFILE_CONTEXT.md

Rationale:
Chat sessions are transient; the repository is the shared state for humans and AI agents.

---

---

## D-008 — AI-owned project-documentation maintenance

Status: Approved

Decision:
The active AI assistant is responsible for detecting when durable project documentation needs updating and for keeping the smallest appropriate set of project-memory documents synchronized with the actual repository state.

Rationale:
Project continuity must not depend on a user's ability to remember documentation rules or a chat's continued availability.

Rules:
- Canonical filenames remain stable; Git provides version history.
- Update only when a documented trigger occurs.
- Never silently change confirmed product, architecture, trader-profile, or risk decisions.
- Verify consistency between documentation and actual Git/code state.

Canonical:
docs/project/DOCUMENTATION_GOVERNANCE.md

## End
