# POLICIES.md

## Escalation Rules

### Escalate to Manager Immediately When

- required inputs are missing
- assignment metadata is invalid
- approval state is unclear
- artifact package is incomplete
- duplicate or overlapping task is discovered
- a dependency is unresolved
- a platform-specific publish failure occurs
- a worker task requires scope change
- external platform policy or auth issue blocks execution
- Project fields and issue labels disagree
- task is missing `task_id`

## Assignment Enforcement Rules

A worker task is executable only if all are true:
- issue label includes `agent:{agent}`
- Project field `Owner Agent` matches the worker
- Project field `Status` is `Ready` or `In Progress`
- assignee is set if possible

If any are missing:
- do not proceed
- add concise GitHub comment requesting fix
- wait for manager correction

## Permission Rules

### Workers may set only:
- In Progress
- Review
- Blocked
- Failed

### Workers must:
- add an update comment on each meaningful action
- include `task_id`
- record artifacts, blockers, and next action

### Workers may not:
- set Approved
- set Done
- reassign work
- create downstream tasks as workflow truth

### Manager only may:
- approve
- mark done
- reassign
- create downstream tasks
- validate outputs and transition post-review states

## Retry Rules

- Never retry silently.
- Record every retry attempt in issue comments.
- Retry only when the failure mode is transient or explicitly authorized.
- For publish failures, preserve per-platform result granularity.
- After 1 failed attempt caused by incomplete inputs, do not retry until manager updates the task.
- After transient platform or network failure, allow one controlled retry if policy permits.
- If second attempt fails, mark task `Failed` or `Blocked` and escalate to manager.

## Handoff Comment Format

Every meaningful worker update must use:

### Update
- task_id:
- agent:
- status:
- done:
- artifacts:
- blockers:
- next_action:

## Artifact Handling Rules

- Every artifact must be linked from the task record or update comment.
- Use stable links when possible.
- Distinguish draft artifacts from approved final artifacts.
- Never overwrite final approved artifacts without recording a revision.
- Publisher must use only the approved final artifact package.
- If an artifact link is broken, missing, or ambiguous, block execution and escalate.

## Audit Rules

- All meaningful task progress must be visible through issue comments and project field updates.
- If not recorded in GitHub, it is considered not done.
- No silent state changes.
- No silent retries.
- No silent approval assumptions.
- No silent task closures.
- Keep comments concise.
