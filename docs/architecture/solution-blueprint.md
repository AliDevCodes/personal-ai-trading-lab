# Solution Blueprint

## Status and purpose

This is the approved architecture and scaffolding blueprint for the Personal AI Trading Lab. It defines the structure to create after an explicit scaffolding task is authorized. It does not authorize feature implementation or resolve the remaining business rules.

The system remains a paper-trading laboratory. Strategy V1 remains unvalidated, deterministic logic owns financial state, and AI is advisory only.

## 1. Approved architecture decisions

- Use one modular monolith with Clean Architecture.
- Use four backend projects: `TradingLab.Api`, `TradingLab.Application`, `TradingLab.Domain`, and `TradingLab.Infrastructure`.
- Use one separate React + TypeScript + Vite client at `web/trading-lab-web`.
- Use three test projects: `TradingLab.UnitTests`, `TradingLab.IntegrationTests`, and `TradingLab.ApiTests`.
- Keep modules as logical boundaries within the four backend projects; do not create one project per module.
- Do not introduce microservices, CQRS, Event Sourcing, MediatR, a generic Repository Pattern, background queues, a generic Common framework, or unjustified provider abstractions.

## 2. Planned physical solution structure

```text
src/
  TradingLab.Api/
  TradingLab.Application/
  TradingLab.Domain/
  TradingLab.Infrastructure/
tests/
  TradingLab.UnitTests/
  TradingLab.IntegrationTests/
  TradingLab.ApiTests/
web/
  trading-lab-web/
docs/
```

This physical layout implements the logical dependency direction; it does not make modules independently deployable services.

## 3. Project reference matrix

| Project | Permitted references |
| --- | --- |
| `TradingLab.Domain` | None |
| `TradingLab.Application` | `TradingLab.Domain` |
| `TradingLab.Infrastructure` | `TradingLab.Application`, `TradingLab.Domain` |
| `TradingLab.Api` | `TradingLab.Application`, `TradingLab.Infrastructure` only for composition-root/dependency-injection wiring |
| `TradingLab.UnitTests` | `TradingLab.Domain`, `TradingLab.Application` |
| `TradingLab.IntegrationTests` | `TradingLab.Application`, `TradingLab.Infrastructure`, `TradingLab.Domain` |
| `TradingLab.ApiTests` | `TradingLab.Api` |

Dependency direction is:

```text
API → Application → Domain
API → Infrastructure → Application + Domain
```

The frontend has no backend project reference and communicates with the API over HTTP.

## 4. Modules and layer placement

| Module | Domain | Application | Infrastructure | API |
| --- | --- | --- | --- | --- |
| Market | Asset, Market, Timeframe, Candle | Market-data use cases; `IMarketDataProvider` | Provider adapters | Market HTTP features |
| MarketAnalysis | MarketObservation and deterministic analysis | Analysis use cases | Persistence when approved | Analysis HTTP features |
| Strategy | StrategyDefinition, Setup, StrategyEvaluator | Evaluation use cases | Persistence when approved | Strategy/signal HTTP features |
| Risk | RiskPolicy, RiskDecision, risk evaluation | Risk-assessment use cases | Persistence when approved | No financial-rule logic |
| Signal | Signal lifecycle | Signal review use cases | Persistence when approved | Signal HTTP features |
| AI | No financial authority | Advisory use cases; `IAIProvider` | OpenAI adapter | Advisory-response HTTP features |
| PaperTrading | PaperAccount, VirtualOrder, Position | Paper-entry, valuation, and close use cases | Persistence when approved | Paper-trading HTTP features |
| Journal | JournalEntry | Journal use cases | Persistence when approved | Journal HTTP features |
| Portfolio | No separate accounting aggregate | Read-model use cases only | Read-model implementation when approved | Portfolio-view HTTP features |
| Notification | No Domain Events required | In-app-alert orchestration when specified | Delivery implementation when specified | Alert-read HTTP features |
| ProfileConfiguration | TraderProfile, TradingConfiguration | Profile/configuration use cases | Persistence when approved | Profile/configuration HTTP features |

## 5. Domain-to-project placement

`TradingLab.Domain` contains deterministic domain concepts, value objects, aggregate behavior, invariants, and the existing domain services. It must not reference ASP.NET, persistence, PostgreSQL, provider SDKs, React, or configuration files.

`TradingLab.Application` contains use cases and the small set of application-facing ports required by those use cases. It coordinates cross-module workflows, such as accepting a Signal, obtaining a RiskDecision, and invoking PaperAccount behavior. It does not contain provider SDK code or financial rules that belong in Domain.

`TradingLab.Infrastructure` implements approved application ports and persistence concerns. It contains provider-specific types and mappings.

`TradingLab.Api` contains HTTP concerns, request/response contracts and mapping, authentication/authorization boundaries, and dependency-injection composition. It contains no business logic.

## 6. Folder and namespace conventions

Organize folders by module inside each layer project. Avoid generic catch-all folders.

```text
TradingLab.Domain/
  Market/  MarketAnalysis/  Strategy/  Risk/  Signal/
  PaperTrading/  Journal/  ProfileConfiguration/

TradingLab.Application/
  Abstractions/
    AI/  MarketData/
  Modules/
    Market/  MarketAnalysis/  Strategy/  Risk/  Signal/  AI/
    PaperTrading/  Journal/  Portfolio/  Notification/  ProfileConfiguration/

TradingLab.Infrastructure/
  AI/  MarketData/  Persistence/  Configuration/

TradingLab.Api/
  Features/  Contracts/  Composition/
```

Use namespaces in the form `TradingLab.<Layer>.<ModuleOrConcern>`, for example `TradingLab.Domain.PaperTrading` and `TradingLab.Application.Modules.Risk`.

## 7. Boundaries

### React/API boundary

The React client owns presentation, user interaction, and API consumption. It must not implement risk calculations, position sizing, accounting, P&L, strategy evaluation, or paper-trading state transitions. The API owns HTTP mapping only; Application and Domain own behavior.

### AI boundary

`IAIProvider` belongs in Application. Its implementation belongs in Infrastructure. AI output is attributed advisory reasoning or explanation; it cannot calculate, approve, waive, or mutate risk, accounting, position sizing, paper-account state, or execution state.

### Market-data provider boundary

`IMarketDataProvider` belongs in Application. Provider implementations belong in Infrastructure. Domain and Strategy use normalized market concepts only, never Nobitex types, symbols, SDKs, or APIs. Gold and silver integration remains blocked until a provider is selected.

### Paper-trading boundary

PaperTrading owns PaperAccount, VirtualOrder, Position, virtual cash, P&L, and paper-trading state. It consumes a valid deterministic RiskDecision but remains separate from Strategy. It has no real-money execution, leverage, margin, liquidation, or auto-trading path.

### Configuration/Profile boundary

`ProfileConfiguration` is one supporting boundary. TraderProfile holds user context, preferences, constraints, and behavioral hypotheses only. TradingConfiguration holds versioned, changeable operational assumptions, including risk settings and paper-trading assumptions. TradingConfiguration is business configuration; infrastructure settings such as connection strings and API keys do not belong to it.

## 8. Anti-coupling and maintainability rules

- Modules communicate through application orchestration and explicit contracts; they do not access one another’s database tables.
- Use identifiers and immutable snapshots/references across aggregate boundaries; do not share mutable aggregate internals.
- Keep workflow dependencies one-way: Market data and analysis support Strategy; Strategy produces Signals; Risk evaluates proposals; PaperTrading changes financial state.
- Keep Portfolio read-only in V1 and do not create a second accounting authority.
- Keep provider types, persistence models, HTTP contracts, and React view models at their own boundaries.
- Create a folder or abstraction only when a concrete approved responsibility needs it.
- Keep each change focused on one small vertical slice, with related tests and documentation updates.
- Do not place business logic in API endpoints, UI components, Infrastructure adapters, or configuration files.

## 9. Testing structure

Unit tests cover deterministic Domain and Application behavior, especially PaperTrading invariants, Risk calculations, Strategy evaluation, and ProfileConfiguration behavior.

Integration tests cover Infrastructure implementations, persistence behavior, provider adapters, and configuration wiring once those exist.

API tests cover HTTP contracts, validation, mapping, authentication/authorization boundaries, and API-to-Application behavior.

Test folders should mirror modules where useful. AI output must not be used as the correctness oracle for deterministic financial tests.

## 10. Scaffolding sequence

After an explicit scaffolding task is approved:

1. Create the solution, four backend projects, three test projects, and React/Vite client.
2. Apply only the project references in this blueprint.
3. Create the empty project/module folder structure and namespace conventions.
4. Create the minimal API composition root without business behavior.
5. Add test-project references and verify the dependency graph builds.
6. Verify the React client starts without adding feature behavior.
7. Select and authorize a first small vertical slice before implementing any domain or infrastructure behavior.

## 11. Intentionally not created during scaffolding

Scaffolding creates structure only. It must not implement a strategy, risk formula, paper-trading mechanics, AI behavior, market-data integration, database schema, authentication implementation, notification mechanism, Docker configuration, or external providers. It must not add unnecessary dependencies.

## 12. Remaining TBD items

The following are deferred to the relevant feature implementation phase and must not be invented during scaffolding:

- Initial virtual capital and virtual-cash representation.
- Risk limits, sizing formula, fees, spread, slippage, fills, rounding, valuation, and SL/TP behavior.
- Signal, order, and position lifecycles; partial closes, concurrent positions, reversals, and simulated-short mechanics.
- Strategy rule language, entry/exit rules, and validation evidence.
- Gold and silver data provider.
- Persistence, identity/authentication, cross-module consistency, and RiskDecision snapshot/audit details.
- Basic in-app alert triggering and delivery mechanism.
- The first feature slice’s API and frontend interaction details.

## 13. Scaffolding Exit Criteria

Scaffolding is complete only when:

- The solution builds.
- All project references match the approved matrix.
- Test projects build.
- The React application starts.
- No business logic exists.
- No unnecessary dependencies were added.
- No database schema, provider integration, Docker configuration, or feature behavior was created.
- The repository structure matches this blueprint.
