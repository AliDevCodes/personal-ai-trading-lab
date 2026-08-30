# ADR-001: Architecture Style

## Status

Accepted.

## Context

Personal AI Trading Lab is currently a small project built by a single developer who is learning software engineering and works with AI coding assistance. The architecture must make responsibilities and dependencies easy to read, test, debug, and maintain without introducing premature operational or conceptual complexity.

The product also has strict boundaries: it is a paper-trading laboratory, deterministic logic owns financial and trade-state decisions, and AI is advisory only.

## Options considered

### Simple monolith

A simple monolith would minimize initial structure but provides insufficiently explicit boundaries for financial logic, external integrations, and future growth. It risks mixing HTTP, provider, persistence, and business concerns as implementation expands.

### Modular monolith + four layers

A modular monolith with Clean Architecture and the API, Application, Domain, and Infrastructure layers keeps one deployable application while making dependency direction and module responsibilities explicit.

### Module-first architecture

A module-first organization could make product areas prominent, but it does not by itself define clear rules for dependency direction or external integrations. Adopting it as the primary organizing decision now would add a competing structural model before the four-layer foundation is established. Module boundaries remain important inside the chosen layered modular monolith.

### Microservices

Microservices would create deployment, operational, distributed-data, and debugging complexity that is not justified by the current project size, single-developer workflow, or V1 scope.

## Decision

Use a modular monolith with Clean Architecture and four primary layers:

1. API
2. Application
3. Domain
4. Infrastructure

The detailed layer and module rules are maintained in [architecture principles](../architecture/principles.md) and the [module map](../architecture/module-map.md).

The backend will use four .NET projects: `TradingLab.Api`, `TradingLab.Application`, `TradingLab.Domain`, and `TradingLab.Infrastructure`. The React client is a separate UI/client boundary, not a fifth backend layer. Tests will use separate unit, integration, and API test projects.

## Why this decision

- The current project is small and does not need distributed deployment.
- A single developer with an AI coding workflow benefits from obvious file placement and explicit dependency direction.
- One deployable application makes debugging and local deployment simpler.
- The four layers provide a readable structure for separating business logic from HTTP, persistence, and external providers.
- Explicit boundaries protect deterministic financial logic from provider and AI implementation details.
- The modular monolith leaves room to extend the system later without committing to premature distributed architecture.
- A fixed, small project layout gives a single developer and AI-assisted workflow predictable locations for code and tests.

## Why module-first is not chosen now

Module boundaries are required, but a module-first architecture is not the primary organizational decision at this stage. The project first needs a simple, consistent layer model that makes dependency direction clear. Modules will be applied within that model; their exact public contracts and internal structure are TBD before implementation design.

## Why microservices are explicitly rejected for now

Microservices are rejected for the current stage because they introduce separate deployment units, inter-service communication, distributed data concerns, operational overhead, and more difficult debugging. No current product requirement justifies those costs.

## Consequences and trade-offs

- The project will have one deployable application and explicit module boundaries rather than independently deployed services.
- Business logic can be tested without HTTP, databases, or provider SDKs when it remains in Domain and Application.
- Provider and persistence details are isolated in Infrastructure, which may require explicit mapping and interfaces.
- Developers must preserve layer and module boundaries even when a shortcut appears faster.
- This decision does not adopt Repository Pattern, CQRS, Event Sourcing, microservices, or similar patterns by default; a concrete requirement is required before reconsidering any of them.

## Conditions for revisiting

Revisit this decision only when concrete evidence shows that the current architecture no longer meets a real need, such as:

- independently deployable components are required for a demonstrated operational reason;
- a module has materially different scaling, reliability, security, or release needs;
- the team and operational capacity can support distributed systems;
- provider, persistence, or integration complexity exposes a specific limitation in the four-layer modular monolith; or
- measured development or maintenance evidence shows the current boundaries are inadequate.

Until then, unresolved matters such as module contracts, persistence ownership, transaction boundaries, authentication/authorization requirements, and UI/API integration remain TBD.
