# Trader Profile

## Confirmed user profile

The following facts and preferences were confirmed through the trader interview and guide early product decisions.

- Capital: $500–$1,000.
- Priority: capital preservation → profitability → capital growth.
- Risk tolerance: low to moderate.
- No leverage initially.
- Preference: selective, high-quality setups.
- Style: swing-biased hybrid.
- Direction: long and short in paper trading.
- Overnight positions: acceptable when risk is predefined.

## Behavioral hypotheses to validate

- Early-exit risk.
- Perfectionism and over-analysis.
- Drawdown sensitivity.
- Excessive strategy tweaking.

These are interpretations of behavior, not confirmed facts. Validate them through paper trading and observed behavior before treating them as established design inputs. Until then, the UI, journal, and risk workflow may make the relevant behavior visible without assuming it is present.

## Boundary

TraderProfile is user context only: facts, preferences, constraints, and behavioral hypotheses. It does not set or override deterministic risk rules. Versioned, changeable operational assumptions—including risk settings, valuation, markets, and paper-trading settings—belong to TradingConfiguration, not TraderProfile.
