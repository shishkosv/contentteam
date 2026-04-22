# ROUTING.md

## Default Routing

- All inbound human requests route to `manager`.
- `manager` is the only planning, decomposition, review, approval-gate, and orchestration authority.
- `creator` and `publisher` are worker agents only.
- No human request should route directly to `creator` or `publisher` unless `manager` explicitly delegates through a tracked task.

## Routing Rules

- all human requests -> `manager`
- planning -> `manager`
- task decomposition -> `manager`
- deduplication against existing work -> `manager`
- GitHub issue creation/update/assignment -> `manager`
- creative work -> `creator`
- artifact production -> `creator`
- caption drafting -> `creator`
- overlay text creation -> `creator`
- publishing work -> `publisher`
- blocked tasks -> `manager`
- review decisions -> `manager`
- approval decisions -> `manager`
- retries after publish failure -> `manager`

## Hard Boundaries

- `creator` must reject tasks with missing required inputs.
- `creator` must never publish directly.
- `publisher` must reject tasks without explicit approval unless the task explicitly states approval is waived.
- `publisher` must never invent strategy, missing copy, or missing creative direction.
- `publisher` must never repair incomplete packages by guesswork. Escalate to `manager`.

## Human Approval Rule

- Default rule: human approval is required before any external publishing.
- Approval waiver must be explicit in the task record.
- If approval state is ambiguous, treat as not approved.

## Lifecycle Routing

- `new` -> `manager` triage
- `ready` -> assigned worker execution
- `in_progress` -> assigned owner works
- `review` -> `manager`
- `approved` -> `publisher` only if target is publishing and approval is satisfied
- `blocked` -> `manager`
- `failed` -> `manager`
- `done` -> `manager` closes loop
