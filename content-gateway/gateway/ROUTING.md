# ROUTING.md

## Default Routing

- All inbound human requests route to `manager`.
- `manager` is the only planning, decomposition, review, approval, reassignment, and orchestration authority.
- `researcher`, `creator`, and `publisher` are worker agents only.
- No workflow coordination happens through free agent chat.
- All execution handoff happens through GitHub task assignment and GitHub comments.

## Routing Rules

- all human requests -> `manager`
- planning -> `manager`
- duplicate check -> `manager`
- task decomposition -> `manager`
- GitHub issue creation/update/assignment -> `manager`
- trend scans and source research -> `researcher`
- artifact production -> `creator`
- approved publishing execution -> `publisher`
- blocked tasks -> `manager`
- review decisions -> `manager`
- approval decisions -> `manager`
- downstream task creation -> `manager`
- retries after publish failure -> `manager`

## Assignment Gate

A worker may proceed only when:
- `agent:{worker}` label is present
- Project `Owner Agent` matches the worker
- Project `Status` is `Ready` or `In Progress`
- assignee is set if possible

If any requirement is missing:
- do not proceed
- add concise comment requesting correction
- wait for manager fix

## Hard Boundaries

- `researcher` does trends and sources only.
- `creator` does artifacts only.
- `publisher` executes only approved publish tasks.
- workers may not coordinate workflow by chat.
- workers may not approve or mark tasks done.
- `creator` must never publish directly.
- `publisher` must reject tasks without explicit approval unless the task explicitly states approval is waived.
- `publisher` must never invent strategy, missing copy, or missing creative direction.
- `publisher` must never repair incomplete packages by guesswork.

## Human Approval Rule

- Default rule: human approval is required before any external publishing.
- Approval waiver must be explicit in the task record or project metadata.
- If approval state is ambiguous, treat as not approved.

## Lifecycle Routing

- `New` -> `manager` triage
- `Ready` -> assigned worker execution
- `In Progress` -> assigned owner works
- `Review` -> `manager`
- `Approved` -> `publisher` only if target is publishing and approval is satisfied
- `Blocked` -> `manager`
- `Failed` -> `manager`
- `Done` -> `manager` closes loop
