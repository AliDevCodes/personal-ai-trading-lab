# Project Context File Placement & Maintenance

## Save these files into the repository

### docs/project
- MASTER_CONTEXT.md
- CURRENT.md
- STATUS.md
- DECISIONS.md
- SESSION_HANDOFF.md

### docs/trading
- trader-profile.md
- TRADER_PROFILE_CONTEXT.md
- strategy-hypothesis.md
- risk-management.md

---

## What each file is for

AGENTS.md
Engineering rules and hard guardrails. Read before implementation.

MASTER_CONTEXT.md
Stable project-wide context. Read at the start of a new AI session.

CURRENT.md
Exact current checkpoint and immediate task. Keep very current.

STATUS.md
Overall implementation status. Update when the project state materially changes.

DECISIONS.md
Consequential decisions and rationale.

SESSION_HANDOFF.md
Temporary handoff for the next session/chat.

trader-profile.md
Canonical human/trading profile.

TRADER_PROFILE_CONTEXT.md
AI-friendly portable summary of the trader profile.

strategy-hypothesis.md
Current strategy hypothesis and its validation status.

risk-management.md
Current risk rules and their evidence/decision status.

---

## Update cadence

Do not update everything after every commit.

Update CURRENT.md:
when the immediate task/checkpoint changes.

Update STATUS.md:
when overall project status changes materially.

Update MASTER_CONTEXT.md:
when stable project-wide context changes.

Update SESSION_HANDOFF.md:
at the end of a meaningful session or before switching to another AI chat.

Update TRADER_PROFILE_CONTEXT.md:
only when the confirmed trader profile or its AI-facing summary changes.

Update DECISIONS.md:
only after consequential product/architecture/trading decisions.

---

## Important rule

A new AI session should read:
1. AGENTS.md
2. CURRENT.md
3. STATUS.md
4. MASTER_CONTEXT.md
5. TRADER_PROFILE_CONTEXT.md
6. relevant strategy/risk/module docs
7. Git status/history

Then independently inspect the repository before modifying anything.

Do not assume a previous AI summary equals the actual repository state.
