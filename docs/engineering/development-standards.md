# Development Standards

- Use the approved direction: React + TypeScript + Vite; ASP.NET Core on .NET 10; PostgreSQL; Docker; later Python for research/backtesting.
- Keep modules explicit, cohesive, and independently testable within the modular monolith.
- Keep provider-specific code behind `IMarketDataProvider` and `IAIProvider`.
- Prefer deterministic, readable domain logic over cleverness.
- Keep secrets and environment-specific values out of source control and client-side code.
- Document material decisions and TBD resolutions in the relevant document; create an ADR only after a decision is made.
- Do not expand V1 scope without an explicit product decision.
