# TASK_BACKEND.md

## Current Backend

The current task backend is:

- GitHub Issues for task records
- GitHub Projects for workflow visibility and board state

## Abstraction Rule

Treat GitHub as the current implementation of task storage and workflow visibility, not as the permanent workflow engine.

Agents must reason in terms of abstract task concepts:

- task record
- status
- owner
- dependencies
- approval state
- artifacts
- execution log
- blocker record

Not in terms of GitHub-specific assumptions whenever avoidable.

## Future-Proofing Guidance

When creating or updating tasks, preserve concepts that can map cleanly to another backend later:

- `task_id`
- `parent_task_id`
- `project_id`
- `campaign_id`
- `owner_agent`
- `status`
- `priority`
- `task_type`
- `target_channels`
- `acceptance_criteria`
- `inputs`
- `expected_outputs`
- `artifact_links`
- `approval_status`
- `blockers`

## Migration Principle

If the backend later changes from GitHub to another system, agent behavior should remain stable.

Only the storage adapter and field mapping should need to change.

## Practical Rule

Write task updates so they remain readable and machine-parseable outside GitHub.
Avoid relying on GitHub-only conventions as the sole source of workflow meaning.
