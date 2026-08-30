# Architecture Principles

## Confirmed direction

- The application is a modular monolith using Clean Architecture with four primary layers: API, Application, Domain, and Infrastructure.
- This direction is recorded in [ADR-001](../adr/ADR-001-architecture-style.md).
- Keep the structure explicit, readable, testable, and maintainable. Avoid premature complexity.
- Depend on abstractions at external boundaries.
- Repository Pattern, CQRS, Event Sourcing, microservices, and similar patterns are not adopted by default; reconsider them only when a concrete requirement later justifies them.

## Layer dependency rules

### Domain

- Depends on nothing external.
- Contains domain concepts and deterministic business rules.
- Must not reference ASP.NET, EF Core, PostgreSQL, OpenAI SDKs, exchange SDKs, React, or other framework/provider details.

### Application

- May depend on Domain.
- Owns application use cases and the abstractions/interfaces required by the application.
- Must not depend directly on external provider SDKs.

### Infrastructure

- Implements persistence and external integrations.
- May depend on Application and Domain.
- Contains provider-specific details, including market-data and AI-provider implementations.

### API

- Handles HTTP concerns, request/response mapping, authentication/authorization boundaries, and dependency-injection composition.
- References Application and Infrastructure. Its Infrastructure reference is only for composition-root/dependency-injection wiring.
- Must not contain business logic.

## Project reference matrix

| Project | Permitted references |
| --- | --- |
| Domain | None. |
| Application | Domain. |
| Infrastructure | Application and Domain. |
| API | Application and Infrastructure; Infrastructure is used only for composition-root/dependency-injection wiring. |

## Physical solution layout

The backend uses four .NET projects:

```text
src/
  TradingLab.Api
  TradingLab.Application
  TradingLab.Domain
  TradingLab.Infrastructure
```

The React client is a separate UI/client boundary, not a fifth backend layer. It communicates with the API:

```text
web/
  trading-lab-web
```

Tests are separate projects:

```text
tests/
  TradingLab.UnitTests
  TradingLab.IntegrationTests
  TradingLab.ApiTests
```

## Module boundary rules

- Modules are boundaries within one deployable monolith, not independently deployable services. See the [module map](module-map.md).
- Modules are organized primarily within the four backend layers; they do not each require a separate project.
- Do not add projects beyond the four backend and three test projects without a concrete requirement.
- A module must not access another module's database tables directly.
- UI and API request mapping must not own business rules.
- Strategy must use market-data contracts and must not directly call provider-specific market APIs.
- AI is advisory only and must not own deterministic risk, accounting, or trade-state logic.
- Paper trading must remain separate from Strategy.
- Deterministic domain/application logic owns financial calculations, risk, accounting, and trade state.

## Provider direction

- Crypto: Nobitex public market-data APIs initially, behind application-owned market-data abstractions.
- AI: OpenAI API initially, behind the application-owned `IAIProvider` abstraction.
- Gold and Silver market-data provider: TBD before integration work begins.

## TBD before implementation design

- Exact module public contracts and dependency relationships.
- Persistence model, schema ownership, and transaction boundaries.
- Authentication/authorization requirements and API surface.
- UI client structure and its integration boundary with the API.
- External provider implementation details.
