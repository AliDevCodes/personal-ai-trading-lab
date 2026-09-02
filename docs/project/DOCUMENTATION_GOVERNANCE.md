# Documentation Governance

## Purpose

This document defines how the project's durable documentation is maintained so that a future human or AI session can reconstruct:
- what the product is
- why key decisions were made
- what the project currently contains
- exactly where development stopped
- what the trader profile is
- what remains undecided
- what the next implementation step should be

The active AI assistant is responsible for keeping these documents synchronized with the real project state when an update is warranted.

Chat history is temporary context. Repository code, Git history, and canonical project documents are the durable project memory.

## 1. Documentation hierarchy

Resolve conflicts in this order:
1. Actual repository files and Git state
2. Canonical product/trading/architecture documents
3. docs/project/MASTER_CONTEXT.md
4. docs/project/CURRENT.md and docs/project/STATUS.md
5. docs/project/SESSION_HANDOFF.md
6. Previous chat summaries

Never silently reconcile conflicting information. Investigate first.

## 2. Canonical locations

### docs/project/
- MASTER_CONTEXT.md — stable cross-session project context
- CURRENT.md — exact current checkpoint and immediate next task
- STATUS.md — overall implementation/milestone status
- DECISIONS.md — consequential product/architecture/trading/workflow decisions
- SESSION_HANDOFF.md — temporary handoff for the next session
- project-map.md — concise product/module map
- DOCUMENTATION_GOVERNANCE.md — rules for maintaining project memory

### docs/trading/
- trader-profile.md — canonical trader profile
- TRADER_PROFILE_CONTEXT.md — AI-friendly profile handoff
- strategy-hypothesis.md — strategy hypothesis and validation state
- risk-management.md — confirmed risk principles and unresolved rules

Other canonical sources:
- AGENTS.md
- docs/product/*
- docs/architecture/*
- docs/domain/*
- docs/engineering/*
- docs/design/*

Do not create another "master" document without an explicit need.

## 3. Responsibility

The active AI assistant owns documentation maintenance.

The user is not expected to remember:
- which file needs updating
- what belongs in which file
- when a handoff should be refreshed
- when a decision should be recorded

The AI should detect these triggers proactively, make the smallest necessary update, and report what was updated.

## 4. What to document

Document durable information that helps future work:
- product scope and exclusions
- architecture boundaries
- important technical decisions
- trader-profile facts/preferences
- strategy/risk assumptions
- completed milestones
- current checkpoint
- verification results
- blockers
- open decisions/TBDs
- next exact action
- consequences of important changes

Do not document:
- raw chat transcripts
- every CLI command
- temporary debugging output
- redundant explanations
- speculative ideas as decisions
- generated build output
- duplicated copies of the same truth

## 5. File-by-file maintenance policy

### MASTER_CONTEXT.md
Update when stable project-wide context changes:
- product direction
- architecture
- technology stack
- long-term AI/Git workflow
- major roadmap structure
- durable memory system

Do not update for ordinary small bug fixes.

### CURRENT.md
Update whenever the exact active checkpoint changes:
- task changes
- milestone starts/ends
- verification gate changes
- next exact action changes
- intentional pause/recovery checkpoint

Keep it short.

### STATUS.md
Update when overall implementation status materially changes:
- capability becomes completed
- blocker appears/resolves
- milestone changes

Do not turn it into a session transcript.

### SESSION_HANDOFF.md
Update at the end of every meaningful session, especially when:
- changing Chat
- stopping mid-task
- local and GitHub state may differ
- an unresolved question remains

It is temporary. Overwrite the active file. Git is the historical backup.

### DECISIONS.md
Update only when a consequential decision is actually made.

Each decision should capture:
- decision
- status
- context/problem
- chosen option
- rationale
- important trade-offs
- rejected alternatives when useful
- consequence/restriction

### trader-profile.md
Canonical trader profile. Update only when the user confirms a profile change or explicitly changes/reclassifies a behavioral hypothesis.

### TRADER_PROFILE_CONTEXT.md
Keep synchronized with trader-profile.md as the portable AI-facing summary. Never invent new confirmed facts.

### strategy-hypothesis.md
Record strategy hypotheses, rule intent, scope, evidence state, and TBDs. Never label an unvalidated hypothesis as validated.

### risk-management.md
Record confirmed deterministic risk principles and clearly marked TBD risk parameters. Never invent missing risk settings.

## 6. When documentation should be updated

### New feature completed
Usually update:
- CURRENT.md
- STATUS.md
- SESSION_HANDOFF.md

Update MASTER_CONTEXT.md only if stable project context changed.

### Architecture/product/trading decision
Update:
- DECISIONS.md
- affected canonical document
- MASTER_CONTEXT.md when durable project context changes
- CURRENT.md if the immediate task changes

### Trader profile change
Update:
- trader-profile.md
- TRADER_PROFILE_CONTEXT.md
- MASTER_CONTEXT.md summary/reference when appropriate
- DECISIONS.md only when there is a consequential decision

### Session ends mid-task
Update:
- SESSION_HANDOFF.md
- CURRENT.md when checkpoint/next action changes

### Ordinary bug fix
Usually no project-memory update unless it changes a durable decision, capability, boundary, or checkpoint.

## 7. Update method

1. Inspect actual repository/Git state.
2. Identify which durable truth changed.
3. Update the smallest appropriate set of documents.
4. Preserve existing terminology and structure.
5. Keep TBD items explicitly TBD.
6. Verify cross-document consistency.
7. Review the Git diff.
8. Include related documentation changes in the same logical change when practical.
9. Do not rewrite stable history for style alone.

## 8. Versioning and backup

Canonical filenames are stable and must not contain version suffixes:
- MASTER_CONTEXT.md
- CURRENT.md
- STATUS.md
- DECISIONS.md
- SESSION_HANDOFF.md
- TRADER_PROFILE_CONTEXT.md

Do not create:
- MASTER_CONTEXT_vX.md
- CURRENT_vX.md
- dated Session Handoff copies

Git history provides historical versions and rollback.

## 9. New AI session protocol

A new AI should read:
1. AGENTS.md
2. docs/project/CURRENT.md
3. docs/project/STATUS.md
4. docs/project/MASTER_CONTEXT.md
5. docs/project/DOCUMENTATION_GOVERNANCE.md
6. docs/trading/TRADER_PROFILE_CONTEXT.md
7. relevant product/architecture/trading docs
8. current Git status/history

Then independently verify the repository before modification.

A previous chat summary is context, not authorization.

## 10. Multi-agent rule

Specialized agents share one repository and one durable memory.

Suggested roles:
- Architecture
- Backend/.NET
- Frontend/React/UIUX
- Data/PostgreSQL
- Trading Research/Strategy
- QA/Verification

Every agent must read current documentation, inspect Git state, stay within scope, and update documentation when its work changes durable project knowledge.

No agent is the sole source of truth.

## 11. Quality gate

Before considering a meaningful slice complete, ask:
- Does CURRENT.md point to reality?
- Does STATUS.md match reality?
- Are consequential decisions recorded?
- Is MASTER_CONTEXT.md accurate?
- Is SESSION_HANDOFF.md useful for a fresh chat?
- Is Trader Profile synchronized?
- Are TBDs still honest?
- Did we avoid duplicate/versioned canonical files?

Goal: reliable continuity with minimum duplication.

## End
