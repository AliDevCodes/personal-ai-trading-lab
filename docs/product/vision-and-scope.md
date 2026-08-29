# Vision and Scope

This document is the canonical source of truth for V1 product scope. Other project documents may summarize it or link here, but must not redefine it.

## Vision

Build a personal laboratory that supports disciplined, explainable evaluation of selected trading opportunities before any real-money activity is considered.

## Confirmed V1 scope

- Monitor BTC, ETH, Gold, and Silver on 4H, 1H, and 15M timeframes.
- Analyze markets using a strategy hypothesis and generate trade signals.
- Use AI for reasoning, interpretation, explanation, and contextual analysis.
- Paper trade long and short positions with a virtual portfolio.
- Maintain a trade journal and track performance.
- Provide basic in-app alerts that support the V1 workflow.

## Confirmed V1 exclusions

- Real-money trading, live execution, and auto-trading.
- Portfolio management beyond the virtual portfolio state needed for paper trading.
- External notifications, including Telegram, mobile push, email, and SMS. These are V2 or later.
- Advanced agents, large-scale scanning, mobile application, Kubernetes, microservices, and advanced ML/fine-tuning.

## Product principles

- Capital preservation precedes profitability and growth.
- Make decisions inspectable: show inputs, deterministic outputs, and AI explanations distinctly.
- Validate strategy claims with evidence, not confidence.

See [roadmap](roadmap.md) for the phased direction and `docs/trading` for the working trading hypotheses.
