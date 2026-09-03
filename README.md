# Personal AI Trading Lab

A personal, AI-assisted laboratory for monitoring selected markets, evaluating a trading-strategy hypothesis, generating explainable paper-trading signals, maintaining a trade journal, and tracking performance.

## V1 scope

The canonical V1 scope, exclusions, and in-app-alerts decision are in the [vision and scope](docs/product/vision-and-scope.md) document. V1 is paper trading only: deterministic application logic owns financial calculations and trading state, while AI is advisory.

## Documentation

- Project dashboard and active guardrails: [status](docs/project/STATUS.md) and [current context](docs/project/CURRENT.md)
- Product direction: canonical [vision and scope](docs/product/vision-and-scope.md) and [roadmap](docs/product/roadmap.md)
- Trading: [trader profile](docs/trading/trader-profile.md), [strategy hypothesis](docs/trading/strategy-hypothesis.md), and [risk management](docs/trading/risk-management.md)
- Architecture and delivery standards: `docs/architecture`, `docs/design`, and `docs/engineering`

Current implemented state (V1):
- .NET backend and ASP.NET Core API (TradingLab.Api)
- Nobitex market-data provider integration
- Historical market-data API endpoint: GET /api/market-data/{symbol}/history
- Automated test suites (unit, integration, API tests)
- React + TypeScript + Vite frontend (web/trading-lab-web)

See docs/project/CURRENT.md and docs/project/STATUS.md for verification details and the current checkpoint.
