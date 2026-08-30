# Domain Model v1 — Personal AI Trading Lab

## Status and intent

This is the canonical **foundation model** for V1 of the Personal AI Trading Lab. It turns the approved product, architecture, trader-profile, strategy, and risk decisions into an explicit, deliberately small model. It is a model for a paper-trading laboratory, not a design for live execution, portfolio management, or a validated trading system. Unresolved implementation and business rules remain explicitly TBD.

Nothing in this document validates the Strategy v1 hypothesis. Deterministic logic owns accounting, sizing, risk, and paper-trading state. AI provides attributed advisory material only.

## Modeling conventions

- An **entity** has a stable identity and can be referred to over time.
- A **value object** is identified by its values, is immutable, and has no independent lifecycle.
- An **aggregate root** is the only write entry point for the consistency boundary named here. This is a boundary for future implementation, not a database schema.
- An identifier reference does not imply ownership or a synchronous cross-aggregate transaction.
- “Historical snapshot” means the values used at creation are retained or reconstructable from the referenced immutable version. Its storage representation is intentionally unspecified.

## Confirmed valuation and market baseline

The following are confirmed V1 decisions: the PaperAccount reporting and valuation currency is **USD**, and the initial V1 markets are:

| Market | Base asset | Quote asset |
| --- | --- | --- |
| BTC/USDT | BTC | USDT |
| ETH/USDT | ETH | USDT |
| XAU/USD | XAU | USD |
| XAG/USD | XAG | USD |

USD and USDT are distinct Assets. The following is a V1 simplification **assumption**, not a confirmed equivalence: paper valuation may treat USD/USDT as 1:1 when an applicable TradingConfiguration version explicitly enables that assumption. A historical paper result must remain associated with the configuration version that supplied that assumption.

## A. Domain concepts

The catalog gives the required modeling facts for each formal concept. “Not Domain” means the concept belongs at an application or infrastructure boundary rather than in the core model.

| Concept | Purpose and classification | Identity / ownership / mutability | Important state and invariants | Relationships, module, and domain rationale |
| --- | --- | --- | --- | --- |
| TraderProfile | Capture a user's facts, preferences, goals, constraints, and behavioral hypotheses. **Entity; aggregate root.** | `TraderProfileId`; owned by its user, represented by an external user reference rather than a User entity. Mutable by user; no versioning requirement is imposed in V1. | Facts such as capital range, risk tolerance, preferences, goals, constraints, and hypotheses. It must not authorize or override risk, strategy, or account state. | May inform UI, journal prompts, and advisory context. Owner: Trader Profile supporting domain area. It belongs in Domain because it is user-owned trading context, not a provider or UI preference bag. |
| TradingConfiguration | Hold changeable system/user assumptions. **Versioned entity; aggregate root.** | `TradingConfigurationId` plus immutable `ConfigurationVersion`; owned by the configuration owner. A change creates a new version rather than changing the meaning of an old version. | Valuation currency, enabled markets, fee/spread/slippage assumptions, risk settings, timeframes, paper-trading settings, and the USD/USDT valuation assumption if used. A version is internally valid and cannot silently change once referenced historically. | Referenced by Signals, RiskDecisions, VirtualOrders, Positions, and PaperAccount accounting where applicable. Owner: Trading Configuration supporting domain area. It belongs in Domain because it changes business meaning without source changes. |
| Asset | Name a distinct asset/currency/commodity. **Value object.** | Value identity (canonical code); no owner or mutation. | Examples: USD, USDT, BTC, ETH, XAU, XAG. USD != USDT. | Composes Money, Quantity, and Market. Owner: Market Data shared kernel. It is domain language, independent of provider symbols. |
| Market | Describe a tradable base/quote pair. **Value object.** | Value identity: base Asset + quote Asset; no owner or mutation. | Base and quote must be distinct; it carries no provider-specific symbol. | Used by Candle, MarketObservation, Setup, Signal, VirtualOrder, and Position. Owner: Market Data shared kernel. It preserves a provider-neutral market vocabulary. |
| Timeframe | Define the interval of a market observation. **Value object.** | Value identity; no owner or mutation. | V1 config can enable 4H, 1H, and 15M. A timeframe is positive and unambiguous. | Used by Candle, MarketObservation, Setup, Signal, and configuration. Owner: Market Data shared kernel. |
| Candle | Represent normalized OHLCV market data for one market interval. **Value object.** | Value identity: market, timeframe, interval start, and values; no owner or mutation. | Open/high/low/close must be valid Prices for the Market quote Asset; high is not below low; interval data is coherent. Provider data quality/availability remains outside this model. | Input to MarketObservation and StrategyEvaluator. Owner: Market Data. It belongs in Domain as normalized market language, not a provider DTO. |
| MarketObservation | Preserve an inspectable normalized observation or analysis input at a point in time. **Entity.** | `MarketObservationId`; owned by Market Analysis; append-oriented/immutable after capture. | Market, timeframe, observed-at time, supporting candles/derived facts, and source provenance suitable for explanation. It contains observations, not orders, account changes, or AI authority. | May support Setup, Signal evidence, journal links, and StrategyEvaluator input. Owner: Market Analysis. It is an entity because an observed record may be referenced later. |
| Setup | Describe a candidate market condition assembled from observations. **Value object.** | Value identity; owned by the containing evaluation/signal; immutable. | Market, timeframe, direction candidate, criteria/context, and evidence references. It is not a validated rule or execution instruction. | Used by StrategyEvaluator to support a Signal. Owner: Strategy. It is a value because it has no lifecycle independent of its evaluation. |
| StrategyDefinition | Describe a named, versioned unvalidated strategy hypothesis. **Entity; aggregate root.** | `StrategyDefinitionId` and `StrategyVersion`; owned by Strategy. Versions are immutable once a Signal cites them. | Human-readable hypothesis, declared inputs/conditions, and status “unvalidated” in V1. It cannot create orders, positions, account changes, or execute trades. | Evaluated by StrategyEvaluator; version referenced by Signal. Owner: Strategy. It is a domain entity because its evolving hypothesis must be traceable. |
| Signal | Make a strategy output reviewable and traceable. **Entity; aggregate root.** | `SignalId`; owned by Signals. State changes occur through its lifecycle, whose exact transition set is still open. | Market, timeframe, proposed direction and entry/SL/TP, strategy/version references, evidence/context, optional risk outcome reference, timestamps, and lifecycle state. A proposal must be coherent for its direction (for example, a long stop is below entry when all values are defined). It never changes account state itself. | Built from Strategy output/Setup and may be reviewed by Risk; JournalEntry and VirtualOrder may reference its ID. Owner: Signals. It belongs in Domain because a signal is a first-class review artifact. |
| RiskPolicy | Express deterministic risk limits and sizing rules selected by configuration. **Value object.** | Value identity; owned by a TradingConfiguration version; immutable. | Applicable risk limits and sizing inputs. It must be complete enough for deterministic evaluation and cannot be modified in place for a historical decision. Exact numeric limits remain TBD. | Input to Risk evaluation. Owner: Risk. It is a value because it has no independent lifecycle apart from its configuration version. |
| RiskDecision | Record the deterministic assessment of a proposed paper entry. **Value object.** | Value identity; owned by the Risk assessment result; immutable. | Approved/rejected result, reasons, computed allowed quantity, and inputs/references necessary to explain it. AI cannot produce or override it. | Produced by the Risk evaluation service from RiskPolicy, proposal, account state, and configuration; consumed by PaperTrading. It is not a separate persisted entity in V1. Persistence, snapshot, and audit requirements remain TBD for the Solution Blueprint. Owner: Risk. |
| PaperAccount | Maintain one virtual trading account and its accounting state. **Entity; aggregate root.** | `PaperAccountId`; owned by its user. Mutable only through paper-trading behaviors. | USD reporting currency; virtual cash by Asset; owned VirtualOrders and Positions; realized/unrealized P&L; account state. It supports simulated long and short only, no leverage, real margin, or liquidation. | References configuration versions, Signals, and RiskDecisions; owns VirtualOrder and Position. Owner: Paper Trading. It is the aggregate root because account-changing operations must preserve accounting consistency. |
| VirtualOrder | Record the simple paper instruction and its simulated market-entry outcome. **Entity inside PaperAccount.** | `VirtualOrderId`; owned exclusively by PaperAccount; mutable only through PaperAccount. | Market, side/direction, requested/filled quantity and price, fee/spread/slippage assumptions/results, entry status, optional SL/TP, timestamps, and source Signal reference. It is not a real venue order and has no order-book simulation. | May open/close a Position; referenced by JournalEntry. Owner: Paper Trading. It needs identity for traceability but cannot be modified independently. |
| Position | Represent an open or closed virtual long/short exposure. **Entity inside PaperAccount.** | `PositionId`; owned exclusively by PaperAccount; mutable only through PaperAccount. | Market, direction, open/closed state, quantity, entry/exit information, attached SL/TP, realized and unrealized P&L, and linked configuration version(s). Quantity cannot be negative; open state cannot coexist with zero quantity; a close cannot exceed the open quantity. | Created/changed by VirtualOrder processing; referenced by JournalEntry. Owner: Paper Trading. There is deliberately no separate Holding entity. |
| JournalEntry | Keep a user-owned record of rationale, observation, and reflection. **Entity; aggregate root.** | `JournalEntryId`; owned by its user; editable by that user subject to an implementation audit policy that is TBD. | User-authored content, timestamps, optional references to SignalId, VirtualOrderId, PositionId, and separately attributed AI commentary. AI commentary is advisory, never user-authored or authoritative financial state. | References, but does not own or mutate, trading records. Owner: Journal. It belongs in Domain because personal review is a core product capability. |
| Money | Express an amount in a specific Asset. **Value object.** | Value identity: amount + Asset; no owner or mutation. | Arithmetic requires compatible Assets or an explicit conversion. USD and USDT cannot be added merely by matching a numeric amount. | Used for cash, prices, fees, and P&L. Owner: Paper Trading shared value vocabulary. |
| Price | Express a market price in the Market quote Asset. **Value object.** | Value identity: amount + quote Asset; no owner or mutation. | Positive; quote Asset must match the associated Market. | Used by Candle, Signal proposal, VirtualOrder, and Position. Owner: Market Data shared value vocabulary. |
| Quantity | Express an amount of a specific Asset. **Value object.** | Value identity: amount + Asset; no owner or mutation. | Positive where it denotes an order/position size; Asset must match Market base Asset for a market quantity. | Used by VirtualOrder, Position, and RiskDecision. Owner: Paper Trading shared value vocabulary. |
| Percentage | Express a bounded ratio where a percentage is meaningful. **Value object.** | Value identity; no owner or mutation. | Its valid range depends on its use and must be declared by the containing policy; it is not an untyped decimal. | Used by RiskPolicy and configurable fee/spread/slippage assumptions where appropriate. Owner: shared value vocabulary. |
| TradeDirection / OrderSide | Express long/short exposure and buy/sell order intent without conflating them. **Value object / constrained enum.** | Value identity; no owner or mutation. | Direction is Long or Short in V1. Side is Buy or Sell. Mapping depends on whether an order opens or closes an exposure; it must be explicit. | Used by Signal, VirtualOrder, Position, and risk. Owner: Paper Trading shared value vocabulary. |
| StrategyEvaluator | Deterministically evaluate a StrategyDefinition against normalized inputs and produce a proposed Signal or no signal. **Domain service.** | No identity; stateless. | Must preserve strategy/version traceability and must not create orders, positions, account changes, or a claim of validation. | Uses StrategyDefinition, MarketObservation/Candle, Setup; produces Signal proposal. Owner: Strategy. It belongs in Domain because this is core business evaluation, not provider orchestration. |
| Risk evaluation service | Deterministically validate a proposal and calculate allowed quantity. **Domain service.** | No identity; stateless. | Applies RiskPolicy against account state and proposal; returns RiskDecision. AI cannot bypass it. | Uses RiskPolicy, Signal/proposal, PaperAccount read state, and configuration. Owner: Risk. It belongs in Domain because risk/sizing rules are business logic. |
| PaperTrading service/behavior | Apply a validated simple paper market entry and subsequently value/close it. **Aggregate behavior, not a separate entity.** | Operates through PaperAccount only. | Applies configured fee/spread/slippage assumptions, preserves cash/P&L/quantity invariants, and does not model a complex book, leverage, margin, or liquidation. | Uses PaperAccount, VirtualOrder, Position, current normalized price, and RiskDecision. Owner: Paper Trading. This is behavior within the aggregate boundary. |
| IMarketDataProvider | Obtain external market data. **Application port; not a Domain concept.** | Implementation identity is infrastructure-owned. | Domain consumes normalized data only; provider symbols and SDKs do not enter Asset/Market. Crypto implementation initially uses Nobitex public APIs; XAU/XAG provider is TBD. | Called by Market Data application layer; implementations live in Infrastructure. |
| IAIProvider | Obtain AI-generated reasoning or commentary. **Application port; not a Domain concept.** | Implementation identity is infrastructure-owned. | Output is advisory and attributed; it cannot mutate financial state or produce authoritative risk/accounting decisions. OpenAI is the initial implementation direction. | Called by AI application layer; implementation lives in Infrastructure. |

## B. Entities

The V1 entity set is intentionally limited:

| Entity | Root? | Notes |
| --- | --- | --- |
| TraderProfile | Yes | Separate user-specific context. |
| TradingConfiguration | Yes | Versioned, historical business assumptions. |
| MarketObservation | No separate aggregate prescribed | Immutable/append-oriented analysis record. |
| StrategyDefinition | Yes | Versioned unvalidated hypothesis. |
| Signal | Yes | Reviewable strategy output; never execution authority. |
| PaperAccount | Yes | Owns financial state. |
| VirtualOrder | No | Inside PaperAccount. |
| Position | No | Inside PaperAccount. |
| JournalEntry | Yes | User-owned record with optional identifier links. |

No primary `Trade` entity is defined. See deferred concepts.

## C. Value objects

`Money`, `Price`, `Quantity`, `Percentage`, `Timeframe`, `Asset`, `Market`, `Candle`, `Setup`, `RiskPolicy`, `RiskDecision`, and `TradeDirection` / `OrderSide` are V1 value objects. They may be embedded, serialized, or reconstructed in different ways later; this document makes no persistence decision.

`MarketObservation`, `Signal`, `VirtualOrder`, and `Position` are not value objects because they require stable references or lifecycle state. Not every value object must be stored independently.

## D. Domain services

Only two independent V1 domain services are required by the model:

- `StrategyEvaluator` evaluates the unvalidated hypothesis using normalized market inputs and produces a reviewable proposal/Signal or no signal.
- The Risk evaluation service applies `RiskPolicy`, validates the proposal against current PaperAccount state, and computes permitted sizing as a `RiskDecision`.

Paper-account operations are behavior of `PaperAccount`, not a broad “trading engine” service. Provider access, scheduling, persistence, notifications, AI prompting, and orchestration are application/infrastructure concerns. Basic in-app alerts are confirmed V1 scope, but their implementation mechanism is TBD until the Solution Blueprint; this model does not introduce Domain Events, background queues, or other infrastructure for them.

## E. Aggregate roots and F. aggregate boundaries

| Aggregate root | Boundary | Consistency responsibility |
| --- | --- | --- |
| TraderProfile | A user's profile facts, preferences, goals, constraints, and hypotheses. | Profile changes cannot grant execution or risk authority. |
| TradingConfiguration | One configuration identity and its immutable versions. | A cited version remains interpretable; modifications create a new version. |
| StrategyDefinition | One strategy hypothesis and versions. | A cited version remains unvalidated and understandable. |
| Signal | One reviewable output and its lifecycle. | Signal evidence, proposal, and references stay coherent; it does not mutate PaperAccount. |
| PaperAccount | Virtual cash, all owned VirtualOrders and Positions, and account P&L/state. | Every entry/valuation/close preserves virtual cash, quantity, and P&L consistency. |
| JournalEntry | One user-owned journal record. | References are identifiers only; journal editing cannot alter referred trading state. |

`MarketObservation` has an identity for traceability, but V1 does not require a richer aggregate boundary around it. Market Data and Market Analysis records are append-oriented inputs/evidence. Cross-boundary actions—such as accepting a Signal, obtaining a RiskDecision, and entering it into a PaperAccount—are application-layer orchestration. They must not bypass either the Risk decision or PaperAccount boundary.

## G. Domain invariants

1. Paper trading is virtual only. No domain path represents a real-money order, live execution, leverage, real margin, or liquidation.
2. PaperAccount reports and values in USD. USD and USDT remain different Assets; any USD/USDT 1:1 valuation is explicit in the relevant configuration version.
3. A Market consists of distinct base and quote Assets. A Price is quoted in that Market's quote Asset, and a market Quantity is in its base Asset.
4. Money arithmetic requires the same Asset or an explicit configured conversion; silent cross-asset arithmetic is invalid.
5. V1 markets are BTC/USDT, ETH/USDT, XAU/USD, and XAG/USD. Enabled markets and timeframes remain configuration-controlled, within approved V1 scope.
6. A Signal is a proposal, not execution authority. Strategy evaluation cannot create a VirtualOrder, Position, account change, or financial calculation.
7. A RiskDecision is deterministic, explainable from its inputs, and cannot be supplied, overridden, or waived by AI.
8. A paper entry requires a valid RiskDecision and active configuration assumptions. The configuration version active for a created order/position must be retained or referenceable.
9. A VirtualOrder and Position can be changed only through PaperAccount behavior. A JournalEntry can only reference them.
10. Position quantities are non-negative; an open Position has positive quantity. A close cannot close more than the current open quantity. Long and Short are simulated directions, not leverage or margin mechanics.
11. Simple V1 execution means simulated market entry with optional Stop Loss and Take Profit, plus configured fee, spread, and slippage assumptions. Complex order-book/fill simulation is excluded.
12. Realized P&L is fixed by an applicable closing action; unrealized P&L is a current valuation of an open Position using a stated price/assumption. Neither may be authored by AI.
13. A StrategyDefinition version remains an unvalidated hypothesis until separate evidence says otherwise; a Signal must cite the strategy/version used when one generated it.
14. AI content is advisory and clearly attributed. It cannot be represented as user journal text, deterministic risk output, market fact, or authoritative financial state.
15. No Domain Events are introduced in V1. Application notifications or audit needs do not by themselves require domain-event modeling.

## H. Module ownership

| Module / domain area | Owns | Does not own |
| --- | --- | --- |
| Market Data | Asset/Market/Timeframe/Candle vocabulary, normalization, and provider boundary use. | Provider SDK details, strategy rules, account state. |
| Market Analysis | MarketObservation. | Provider contracts, Signal lifecycle, trades. |
| Strategy | StrategyDefinition, Setup, StrategyEvaluator. | Orders, Positions, cash, and a claim that the hypothesis is validated. |
| Signals | Signal lifecycle and review record. | Risk calculation or paper-account mutation. |
| Risk | RiskPolicy and RiskDecision evaluation. | AI exceptions and account accounting. |
| Paper Trading | PaperAccount, VirtualOrder, Position, virtual cash, P&L, and paper trade-state changes. | Real execution, portfolio allocation/rebalancing, a Holding entity. |
| Portfolio | V1 read models of virtual account/portfolio state only. | Portfolio management, allocation, optimization, or an authoritative parallel accounting model. |
| Journal | JournalEntry and AI-commentary attribution. | Financial-state mutation. |
| AI | Advisory requests/responses through IAIProvider. | Authoritative market, risk, account, or execution state. |
| ProfileConfiguration supporting boundary | TraderProfile and TradingConfiguration versions. | Treating user context as risk authority or treating operational assumptions as profile preferences. |
| Infrastructure | Nobitex/OpenAI/provider implementations, provider symbols, SDKs, persistence mechanisms. | Domain semantics and financial authority. |

The `ProfileConfiguration` supporting boundary preserves the distinct responsibilities of TraderProfile and TradingConfiguration without creating separate modules. Its exact application-module placement is an implementation decision, not permission to merge either responsibility into hard-coded settings.

## I. Trader Profile vs. Trading Configuration

| Aspect | TraderProfile | TradingConfiguration |
| --- | --- | --- |
| Answers | “Who is this trader and what personal context may help them reflect?” | “What assumptions and deterministic controls apply to this run?” |
| Contains | User facts, preferences, goals, constraints, behavioral hypotheses. | Valuation currency, markets, fees, spread, slippage, risk settings, timeframes, and paper-trading settings. |
| Authority | Informational/advisory context only. | Defines active operational assumptions; RiskPolicy is deterministic. |
| Change behavior | User can update profile context. | Changeable without source modifications; new version preserves history. |
| Historical linkage | No V1 versioning requirement. | Signals, risk outcomes, and paper records must retain the applicable version/reference. |

Trader Profile is not a disguised risk configuration, and Trading Configuration is not a profile preference. For example, “drawdown-sensitive” is a profile hypothesis; a daily loss limit, once decided, is configuration/RiskPolicy.

## J. Concepts explicitly rejected or deferred

| Concept | V1 disposition |
| --- | --- |
| Trade as a primary entity | Deferred/undefined. “Trade” has no agreed business meaning distinct from VirtualOrder and Position, so it is not introduced by name alone. |
| Holding | Rejected for V1. Position within PaperAccount expresses the necessary exposure. |
| Real exchange order, broker account, live execution | Out of scope. |
| Leverage, real margin, liquidation | Out of scope. Simulated short does not imply them. |
| Complex order book, limit order, partial-fill, and venue-fill simulation | Out of scope. V1 has simple virtual market entry plus optional SL/TP. |
| Portfolio allocation, rebalancing, optimization | Future scope. V1 needs only virtual portfolio/accounting read models. |
| Validated strategy/rule performance claims | Deferred until documented backtesting and paper-trading evidence. |
| Provider-specific market symbols, SDK types, or API models | Infrastructure concerns, excluded from Domain. |
| AI as trader, risk authority, account owner, or source of financial truth | Rejected. AI is advisory only. |
| Domain Events | Not introduced; no concrete V1 requirement needs them. |
| User/identity/authentication model | External concern; only opaque user references are needed for ownership. |
| Advanced multi-profile and configuration-version UX | Future scope. The domain supports separate profile and versioned configuration without prescribing UX. |

## K. Open decisions required before implementation

These decisions are intentionally not invented by this model:

1. Initial virtual capital and the exact cash representation (including whether non-USD virtual cash balances are allowed beyond valuation needs).
2. Exact RiskPolicy limits: risk per trade, maximum exposure, maximum concurrent positions, daily/weekly loss limits, and the deterministic sizing formula.
3. Exact fee, spread, slippage, fill timing, price source, rounding/precision, and Stop Loss/Take Profit trigger assumptions.
4. Whether V1 permits partial closes, multiple simultaneous Positions for the same Market/direction, reversal behavior, and the full VirtualOrder/Position lifecycle state sets.
5. The exact Signal lifecycle, including review/accept/reject/expire semantics and who may cause each transition.
6. Minimum StrategyDefinition structure and StrategyEvaluator rule language; this must not assert unvalidated entry/exit rules.
7. Required MarketObservation fields, retention, normalization, and data-quality/staleness handling.
8. Gold and silver market-data provider selection; Nobitex remains the initial crypto public-data source only.
9. TradingConfiguration ownership/access rules, version activation/retirement policy, and whether each PaperAccount selects one active configuration or supports an explicit selection per entry.
10. RiskDecision persistence, snapshot, and audit requirements; it remains a value object and not a separate persisted entity in V1.
11. JournalEntry edit/audit policy and the required rendering/retention of AI attribution.
12. The precise virtual portfolio/performance read models and how unrealized P&L is refreshed.
13. Identity/authentication boundaries and whether a single-user assumption is formalized for V1.
14. The implementation mechanism and triggering approach for confirmed basic in-app alerts; do not introduce Domain Events, background queues, or other infrastructure solely to answer this before the Solution Blueprint.

## L. Traceability to existing architecture

| Existing source | Domain-model consequence |
| --- | --- |
| `README.md` and `docs/product/vision-and-scope.md` | Keeps V1 to BTC, ETH, Gold, Silver; 4H/1H/15M; paper long/short; journal and performance tracking; excludes real execution and portfolio management. |
| `docs/architecture/principles.md` | Keeps deterministic financial authority in Domain/application logic, AI advisory, and external providers behind abstractions. |
| `docs/architecture/module-map.md` | Assigns concepts to its future modular-monolith boundaries and limits Portfolio to V1 accounting/read-model needs. |
| `docs/trading/trader-profile.md` | Models its capital preferences, behavioral risks, and constraints as a separate, changeable TraderProfile—not risk authority. |
| `docs/trading/risk-management.md` | Makes RiskPolicy/RiskDecision deterministic, keeps numeric rules TBD, and keeps virtual cash/positions/P&L in PaperAccount. |
| `docs/trading/strategy-hypothesis.md` | Models StrategyDefinition as unvalidated, separates StrategyEvaluator, and preserves strategy/version traceability in Signal. |
| `AGENTS.md` | Preserves paper-only safety, provider abstractions, no Holding entity, no real shorting mechanics, and no premature implementation detail. |

## Implementation guardrails

This document intentionally does not prescribe classes, tables, APIs, persistence mappings, events, provider SDKs, or project structure. Those choices must implement these boundaries and invariants after the open decisions are resolved.
