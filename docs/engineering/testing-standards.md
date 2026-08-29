# Testing Standards

- Test deterministic financial calculations, risk rules, accounting, and trade-state transitions thoroughly and independently of AI.
- Use unit tests for domain rules and module behavior; add integration/contract tests at provider boundaries when implemented.
- Test both long and short paper-trading flows.
- Make test inputs, pricing assumptions, and expected outcomes explicit.
- Treat AI outputs as variable: test contracts, safety boundaries, and fallback handling rather than asserting financial correctness from generated prose.
- Backtesting and paper-trading evaluation criteria are TBD; do not claim validation before evidence exists.
