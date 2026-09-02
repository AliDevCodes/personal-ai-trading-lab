# Trader Profile Context v1.0

## Purpose

This is the AI-facing summary of the confirmed trader profile.

Canonical source of truth:
docs/trading/trader-profile.md

This file is a portable context/handoff file for AI assistants. It must not override the canonical profile or deterministic TradingConfiguration.

---

## 1. Confirmed profile

Capital:
$500–$1,000

Priority order:
1. Capital preservation
2. Profitability
3. Capital growth

Risk tolerance:
Low to moderate

Leverage:
No leverage initially

Trade selection:
Selective, high-quality setups are preferred.

Style:
Swing-biased hybrid

Direction:
Long and short are acceptable in paper trading.

Overnight:
Overnight positions are acceptable when risk is predefined.

---

## 2. Behavioral hypotheses

These are NOT confirmed personality facts. They are hypotheses to validate through observation and paper trading:

- Early-exit risk
- Perfectionism / over-analysis
- Drawdown sensitivity
- Excessive strategy tweaking

The system should make these behaviors observable through journal/risk/workflow design without assuming they are present.

---

## 3. Product implications

The application should favor:
- deliberate setup selection
- inspectable decision inputs
- predefined risk
- evidence-driven strategy evaluation
- paper trading before any consideration of real-money activity

The application should not:
- encourage high-frequency overtrading
- silently increase risk
- assume leverage
- treat AI recommendations as authoritative
- convert behavioral hypotheses into hard rules without evidence

---

## 4. Boundary with TradingConfiguration

TraderProfile contains:
- user context
- preferences
- constraints
- behavioral hypotheses

TradingConfiguration contains:
- versioned operational assumptions
- risk settings
- valuation assumptions
- selected operational markets
- paper-trading settings

TraderProfile must not override deterministic risk logic.

---

## 5. AI usage

AI may:
- interpret market context
- explain signals
- explain trade decisions
- surface relevant questions
- help review journal behavior

AI must not:
- override risk rules
- mutate accounting
- approve unauthorized risk
- execute real-money trades
- become the authority for financial state

---

## 6. Maintenance rule

Update this file only when:
- a confirmed trader-profile fact changes
- a confirmed preference changes
- a behavioral hypothesis is explicitly added/removed/reclassified
- the AI-facing summary materially changes

Never silently alter the confirmed profile.

When changed:
1. update this file
2. update canonical docs/trading/trader-profile.md if the underlying fact changed
3. update MASTER_CONTEXT.md reference/summary if needed
4. record a decision when the change has meaningful product/trading consequences

---

## End
