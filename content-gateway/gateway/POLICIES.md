# POLICIES.md

## Escalation and Retry Rules

### Escalate to Manager Immediately When

- required inputs are missing
- approval state is unclear
- artifact package is incomplete
- duplicate or overlapping task is discovered
- a dependency is unresolved
- a platform-specific publish failure occurs
- a worker task requires scope change
- external platform policy or auth issue blocks execution

### Retry Rules

- Never retry silently.
- Record every retry attempt in task comments.
- Retry only when the failure mode is transient or explicitly authorized.
- For publish failures, preserve per-platform result granularity.
- After 1 failed attempt caused by incomplete inputs, do not retry until manager updates the task.
- After transient platform or network failure, allow one controlled retry if policy permits.
- If second attempt fails, mark task `failed` or `blocked` and escalate to manager.
- Manager decides whether to reopen, split, or abandon failed work.

## Artifact Handling Rules

- Every artifact must be linked from the task record.
- Use stable links when possible.
- Distinguish draft artifacts from approved final artifacts.
- Never overwrite final approved artifacts without recording a revision.
- Caption sets, overlays, prompts, and images should each be separately referenceable when practical.
- Publisher must use only the approved final artifact package.
- If an artifact link is broken, missing, or ambiguous, block execution and escalate.
- Artifact records should be sufficient for later audit and reuse.

## Audit Rules

- All meaningful task progress must be visible through issue comments and project field updates.
- No silent state changes.
- No silent retries.
- No silent approval assumptions.
- No silent task closures.
