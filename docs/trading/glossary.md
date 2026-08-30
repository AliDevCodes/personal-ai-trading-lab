# Glossary

- **AI provider**: An implementation behind `IAIProvider` that supplies non-authoritative reasoning and explanations.
- **Market-data provider**: An implementation behind `IMarketDataProvider` that supplies market data.
- **Paper trading**: Simulated trading using virtual capital; it never sends a real order.
- **Paper portfolio**: The virtual cash, positions, quantities, P&L, and state required by paper trading.
- **Paper account**: The authoritative virtual-account record for paper trading; it owns virtual cash, positions, orders, P&L, and paper-trading state.
- **Position**: An open or closed long or short paper-trading exposure.
- **Realized P&L**: Profit or loss locked in when a position is closed.
- **Unrealized P&L**: Profit or loss on an open position based on its current valuation.
- **Signal**: A strategy output for review; it is not real execution authority.
- **Strategy hypothesis**: A proposed ruleset that remains unvalidated until evidence supports it.
- **Trader profile**: User-specific facts, preferences, goals, constraints, and behavioral hypotheses; it does not set or override deterministic risk rules.
- **Trading configuration**: Versioned, changeable operating assumptions such as markets, valuation currency, fees, spread, slippage, risk settings, timeframes, and paper-trading settings.
- **Risk decision**: The deterministic approval or rejection and permitted sizing calculated by applying a risk policy to a proposal and account state; AI cannot override it.
