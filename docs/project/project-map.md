# Project Map

## Product purpose

Personal AI-assisted laboratory for disciplined, explainable evaluation of selected market opportunities before real-money activity is considered.

## V1 scope

Monitor BTC, ETH, Gold, and Silver on 4H, 1H, and 15M timeframes; evaluate a strategy hypothesis; generate explainable signals; paper trade long and short positions; maintain a journal and performance view; and provide basic in-app alerts.

## Major trading concepts

Normalized market data and observations feed an unvalidated strategy hypothesis. Signals are reviewable proposals. Deterministic risk decisions govern permitted sizing. A virtual paper account owns cash, positions, orders, and P&L. Journal entries support reflection. Trader profile is advisory context; versioned trading configuration owns operational assumptions.

## Backend layers and modules

Planned backend layers: API, Application, Domain, and Infrastructure. Planned modular-monolith boundaries: Market, Market Analysis, Strategy, Risk, Signal, AI, Paper Trading, Journal, Portfolio read models, Notification, Trader Profile, and Trading Configuration. Modules communicate through explicit contracts, not direct database-table access.

## Boundaries

- **AI:** advisory reasoning and attributed explanations only, behind `IAIProvider`; never authoritative financial state.
- **Risk:** deterministic logic owns risk limits, position sizing, accounting, and trade state.
- **Paper trading:** virtual orders, cash, positions, and P&L only; no real execution, leverage, margin, liquidation, or auto-trading.
- **Frontend:** planned React client is a separate UI boundary that communicates with the API and contains no business rules.

## External integrations

Crypto market data will be accessed through `IMarketDataProvider`; Nobitex public APIs are the initial direction. OpenAI is the initial AI-provider direction. Gold and silver provider selection is TBD and must precede integration work.

## Future direction

After V1 evidence and decisions: Python research/backtesting tooling and external notifications may be considered. Any broader scope, particularly real-money activity, requires a separate explicit decision.
