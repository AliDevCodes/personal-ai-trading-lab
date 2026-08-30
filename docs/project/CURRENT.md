# Current Operational Context

## Repository identity requirement

Before any file modification, run the mandatory preflight in `AGENTS.md`. The origin must be `https://github.com/AliDevCodes/personal-ai-trading-lab`. Never continue if repository verification fails.

## Current phase and task

The project is in foundation and implementation design. The current task is to preserve documented boundaries and resolve implementation-critical decisions before application work begins.

## Allowed scope

- Documentation, planning, and decision clarification within approved V1 scope.
- Future implementation only after its design gate is satisfied and the task explicitly authorizes it.

## Explicitly prohibited actions

- Expanding V1, introducing real-money execution or auto-trading, or adding external notifications.
- Treating AI output as financial authority or bypassing deterministic risk, accounting, sizing, or trade-state rules.
- Adding unapproved architecture, dependencies, frameworks, infrastructure, or provider-specific domain logic.
- Implementing gold or silver data integration before a provider decision.

## Canonical documents to read

- `docs/product/vision-and-scope.md` for V1 scope and exclusions.
- `docs/project/STATUS.md` for project state.
- `docs/architecture/principles.md` and `docs/architecture/module-map.md` for boundaries.
- `docs/domain/domain-model-v1.md` for the domain baseline and open decisions.
- `docs/trading/strategy-hypothesis.md`, `risk-management.md`, and `trader-profile.md` for trading context.

## Architecture and domain-model status

Clean Architecture and modular-monolith direction are documented; no application projects or source code exist. The V1 domain-model baseline is documented, including explicit invariants and open decisions; it is not a persistence, API, or implementation design.

## Next gate

Resolve or explicitly defer the implementation-critical open decisions required by the proposed slice, produce a focused plan for large changes, and obtain task authorization before creating application code.
