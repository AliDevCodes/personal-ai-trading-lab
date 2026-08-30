# Module Map

The following modules are boundaries within one modular monolith, not independently deployable services. They are organized primarily within the four backend layers and do not each require a separate project. The canonical V1 product scope is in `docs/product/vision-and-scope.md`.

| Module | Responsibility |
| --- | --- |
| Market | Market-data acquisition, normalization, and market-data contracts. |
| MarketAnalysis | Deterministic market calculations and derived market observations. |
| Strategy | Unvalidated strategy rules and setup evaluation. |
| Risk | Deterministic risk validation, risk limits, and position-sizing rules. |
| Signal | Normalized trade-signal representation and lifecycle. |
| AI | AI orchestration, model-provider abstraction, advisory reasoning, and explanation. |
| PaperTrading | Virtual orders, positions, and trade simulation; never real execution. |
| Journal | Trade records and journal entries. |
| Portfolio | V1 virtual portfolio/accounting only; no portfolio-management features. |
| Notification | V1 basic in-app alerts only; external channels are V2+. |

## Cross-module rules

- A module must not access another module's database tables directly.
- Modules communicate through explicit contracts; exact contracts and ownership are TBD before implementation design.
- Strategy consumes normalized market-data contracts, not provider-specific APIs.
- PaperTrading remains separate from Strategy.
- AI remains advisory and does not own deterministic risk, accounting, or trade-state logic.

## TBD before implementation design

- Public contracts, dependency relationships, and data ownership for each module.
- Which layer owns each contract implementation and persistence concern.
- Cross-module transaction and consistency approach.
