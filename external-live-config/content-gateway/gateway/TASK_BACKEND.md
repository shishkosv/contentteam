# TASK_BACKEND.md

## Current Backend

The current workflow backend is the OpenClaw content pipeline with durable local orchestration state.

Optional external systems such as GitHub or Google Drive may be used for visibility, sync, or operator workflows, but they are not mandatory prerequisites for agent execution.

## Abstraction Rule

Agents must reason in terms of abstract workflow concepts:

- request_id
- phase
- owner
- dependencies
- approval state
- artifacts
- evaluation result
- execution log
- blocker record

Avoid requiring GitHub-specific fields unless a request explicitly says the workflow is GitHub-backed.

## Future-Proofing Guidance

Preserve concepts that can map cleanly across storage or coordination backends:

- request_id
- parent_request_id
- campaign_id
- owner_agent
- status
- priority
- content_type
- target_channels
- acceptance_criteria
- inputs
- expected_outputs
- artifact_links
- approval_status
- blockers

## Migration Principle

If the backend changes, agent behavior should remain stable.
Only the adapters and field mappings should need to change.

## Practical Rule

Write outputs so they remain readable and machine-parseable outside any single backend.
Do not invent mandatory GitHub-only conventions when the request is coming from the OpenClaw pipeline.