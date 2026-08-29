# AI Coding Agent Rules

## Scope and architecture

- Build a personal AI-assisted trading laboratory as a modular monolith using Clean Architecture. Preserve explicit module boundaries and dependency direction.
- The intended stack is React + TypeScript + Vite, ASP.NET Core on .NET 10, PostgreSQL, Docker, and later Python research/backtesting. Do not introduce alternatives without an approved decision.
- The canonical V1 scope and exclusions are in `docs/product/vision-and-scope.md`; do not expand them without an explicit product decision.
- Do not introduce microservices, Kubernetes, mobile clients, live trading, or auto-trading into V1.

## Trading and safety invariants

- V1 is paper trading only. Never create real-money execution paths; external notifications are not part of V1.
- A virtual paper portfolio is in scope: cash, positions, quantity, realized/unrealized P&L, and portfolio state. Portfolio management is not in scope.
- Deterministic application logic exclusively owns financial calculations, position sizing, risk limits, accounting, and trade state management.
- AI may provide reasoning, interpretation, explanation, and contextual analysis only. It must not make or execute authoritative financial-state changes.
- Treat Strategy V1 as a hypothesis until supported by backtesting and paper-trading evidence.

## Integration boundaries

- Access crypto market data through `IMarketDataProvider`; strategy logic must not depend on Nobitex-specific types or APIs. Nobitex public market-data APIs are the initial crypto source.
- Access AI through `IAIProvider`; OpenAI API is the initial provider direction. Keep secrets server-side and out of source control.
- Gold and silver data-provider selection is TBD. Do not implement their integration until it is resolved.

## Engineering expectations

- Keep changes small, explicit, testable, and documented where decisions change.
- Add or update tests with deterministic domain behavior. Do not rely on AI output for correctness assertions of financial calculations.
- Do not commit credentials, local IDE artifacts, generated build output, or environment-specific configuration.
- Record consequential architecture decisions as ADRs only when a decision has actually been made.
