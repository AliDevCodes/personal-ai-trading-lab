# Architecture Principles

## Confirmed direction

- Modular monolith with Clean Architecture.
- Strong module boundaries; simple, explicit, testable design; maintainability is first-class.
- Avoid premature microservices.
- Depend on abstractions at external boundaries.

## Boundary rules

- Strategy code depends on `IMarketDataProvider`, not Nobitex details.
- AI capabilities depend on `IAIProvider`, not OpenAI-specific details.
- Deterministic domain/application logic owns financial calculations, risk, accounting, and trade state.
- AI remains advisory: reasoning, interpretation, explanation, and contextual analysis.

## Provider direction

- Crypto: Nobitex public market-data APIs initially.
- AI: OpenAI API initially.
- Gold and Silver market-data provider: TBD before integration work begins.
