# Module Map

The following are future modules within one modular monolith. They are boundaries, not independently deployable services. The canonical V1 scope is in `docs/product/vision-and-scope.md`.

| Module | Responsibility |
| --- | --- |
| Market Data | Retrieve and normalize provider data. |
| Market Analysis | Produce market observations from normalized data. |
| Strategy | Evaluate the unvalidated strategy hypothesis and emit signals. |
| Risk | Apply deterministic risk rules and limits. |
| Signals | Represent reviewable strategy outputs. |
| AI | Request advisory reasoning and explanations through `IAIProvider`. |
| Paper Trading | Simulate long/short trades; no real execution. |
| Portfolio | Maintain virtual paper portfolio state required by paper trading; no portfolio-management features. |
| Journal | Record trades, rationale, and observations. |
| Notifications | Deliver basic in-app alerts only; external channels are V2+. |

Cross-module contracts and ownership details are TBD until implementation design begins.
