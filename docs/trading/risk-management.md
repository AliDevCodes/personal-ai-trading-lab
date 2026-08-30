# Risk Management

## Confirmed principles

- Capital preservation is the highest priority.
- Predefined risk is required for overnight positions.
- Deterministic application logic owns position sizing, risk limits, accounting, and trade state.
- AI must not calculate, override, or authorize these controls.
- V1 uses virtual capital only; live execution is out of scope.

## Virtual paper portfolio

V1 requires cash, positions, quantities, realized P&L, unrealized P&L, and portfolio state to support paper trading. This is not portfolio-management functionality.

## Risk decision boundary

The Risk module owns RiskPolicy and RiskDecision. A RiskDecision is an immutable value object produced by deterministic risk validation and consumed by PaperTrading; it is not a separate persisted entity in V1. Its persistence, snapshot, and audit requirements remain TBD for the Solution Blueprint.

## TBD

Risk per trade, maximum exposure, maximum concurrent positions, daily/weekly loss limits, short-selling assumptions, fees, spreads, slippage, fills, and order types.
