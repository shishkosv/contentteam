# TASK_BACKEND.md

## Current Backend

The task backend is enforced through:
- GitHub Issues as the task system
- GitHub Projects as the status and metadata system

## Workflow Authority

- `manager` is the only orchestrator and validator
- workers do execution only
- no agent-to-agent free chat is valid workflow coordination
- all coordination must be reflected in GitHub
- if not recorded in GitHub, it is considered not done

## Required Task Fields

Every task must carry, at minimum:
- `task_id`
- `owner_agent`
- `status`
- `task_type`
- `target_channels`
- objective
- inputs
- acceptance criteria
- outputs

Recommended extended fields:
- `parent_task_id`
- `project_id`
- `campaign_id`
- `priority`
- `artifact_links`
- `approval_status`
- `blockers`

## Assignment Validity

A task is valid for execution only if all are true:
- issue label includes `agent:{agent}`
- Project `Owner Agent` matches
- Project `Status` is `Ready` or `In Progress`
- assignee is set if possible

If invalid:
- worker must not proceed
- worker must comment and request fix

## Project Fields

Required GitHub Project fields:
- Status
- Owner Agent
- Task Type
- Priority
- Approval Status

## Status Lifecycle

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

## Permission Model

### Workers
May set:
- In Progress
- Review
- Blocked
- Failed

Must add update comment.
Cannot set:
- Approved
- Done

### Manager
May set any status.
Only manager may:
- approve
- mark done
- reassign
- create downstream tasks

## Practical Rule

Write issues and comments so they are concise, readable, and machine-parseable.
Every meaningful action should have a GitHub trace.
