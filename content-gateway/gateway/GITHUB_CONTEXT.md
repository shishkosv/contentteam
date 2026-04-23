# GITHUB_CONTEXT.md

Shared GitHub task coordination context for all content-team agents.

## Repository

- repository: `shishkosv/contentteam`
- project_id: `CONTENT-OPS`
- task system: GitHub Issues
- status + metadata system: GitHub Projects

## Core Workflow Rule

All workflow coordination happens through GitHub.
If an action is not recorded in GitHub, it is considered not done.

No free agent-to-agent chat is valid workflow coordination.
Use GitHub Issues, comments, labels, assignees, and Project fields as the workflow source of truth.

## Agents

- `manager` = only orchestrator and validator
- `researcher` = trend and source execution worker
- `creator` = artifact production worker
- `publisher` = approved publishing worker

## Assignment Validity

A task is valid for execution only if ALL are true:
- issue label includes `agent:{agent}`
- Project field `Owner Agent` matches the worker
- Project field `Status` is `Ready` or `In Progress`
- assignee is set if possible

If missing:
- do not proceed
- comment and request fix
- wait for manager correction

## Status Lifecycle

Primary lifecycle:
- New
- Ready
- In Progress
- Review
- Approved
- Done

Alternate:
- Blocked
- Failed

## Permissions

### Workers may set only:
- In Progress
- Review
- Blocked
- Failed

Workers must add concise update comments.
Workers cannot set Approved or Done.

### Manager may:
- set any status
- approve
- mark done
- reassign
- create downstream tasks

## Required Labels

Agent labels:
- `agent:manager`
- `agent:researcher`
- `agent:creator`
- `agent:publisher`

Status labels:
- `status:new`
- `status:ready`
- `status:in-progress`
- `status:review`
- `status:approved`
- `status:blocked`
- `status:failed`
- `status:done`

Task type labels:
- `type:trend-scan`
- `type:content-opportunity`
- `type:image`
- `type:publish`

Priority labels:
- `priority:high`
- `priority:medium`
- `priority:low`

## Required Project Fields

- Status
- Owner Agent
- Task Type
- Priority
- Approval Status

## Required Task Shape

## Task Metadata
- task_id:
- owner_agent:
- status:
- task_type:
- target_channels:

## Objective
{{goal}}

## Inputs
{{context}}

## Acceptance Criteria
- clear
- testable

## Outputs
{{expected}}

## Required Handoff Comment

### Update
- task_id:
- agent:
- status:
- done:
- artifacts:
- blockers:
- next_action:

## Hard Rules

- no silent transitions
- no missing required fields
- no duplicate tasks if an existing task covers the work
- no approval bypass
- always include `task_id`
- keep comments concise
