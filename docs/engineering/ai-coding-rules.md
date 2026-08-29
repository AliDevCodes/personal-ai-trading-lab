# AI Coding Rules

## Allowed AI responsibilities

- Reasoning, interpretation, explanation, and contextual analysis.
- Assistance with architecture, implementation, mentoring, and review, subject to normal engineering verification.

## Prohibited AI authority

- Deterministic financial calculations.
- Position sizing and risk limits.
- Accounting and trade-state management.
- Real-money execution, auto-trading, or authoritative portfolio actions.

## Integration rules

- Use `IAIProvider`; OpenAI API is the initial provider direction.
- Keep API credentials server-side, secret, and out of source control, consistent with official OpenAI API guidance.
- Label AI-produced reasoning clearly and preserve deterministic source data separately.
- Treat model output as non-deterministic and potentially fallible; require deterministic validation before any paper-trade state change.
- Model selection, prompts, retention, cost limits, logging, and fallback behavior are TBD.
