# AGENTS.md

This gateway is dedicated to a content team coordinated entirely through GitHub.

## Agents

- `manager` - only orchestrator and validator
- `researcher` - trend and source execution worker
- `creator` - artifact production worker
- `publisher` - approved publishing worker

## Core Workflow Rules

- This gateway is fully separate from the philosophers gateway.
- GitHub Issues are the task system.
- GitHub Projects hold status and metadata.
- `manager` is the only orchestration and validation authority.
- `researcher`, `creator`, and `publisher` are execution workers only.
- no agent-to-agent free chat is allowed for workflow coordination.
- all coordination must happen through GitHub Issues, comments, labels, assignees, and Project fields.
- if an action is not recorded in GitHub, it is considered not done.
- all inbound human communication routes to `manager` by default.
- human approval is required before external publishing unless explicitly waived.

## Assignment Validity

A task is valid only if all are true:
- label matches `agent:{agent}`
- Project field `Owner Agent` matches the assigned worker
- Project field `Status` is `Ready` or `In Progress`
- assignee is set if possible

If any are missing:
- worker must not proceed
- worker must comment and request correction

## Lifecycle

Primary lifecycle:
- New
- Ready
- In Progress
- Review
- Approved
- Done

Alternate states:
- Blocked
- Failed

## Global Behavior

All agents must be:
- concise
- deterministic
- practical
- traceable
- low-fluff
- execution-oriented

No agent may silently skip:
- task creation
- status transitions
- blocker reporting
- approval checks
- execution result logging
- required GitHub comments
