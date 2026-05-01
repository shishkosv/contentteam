# OpenClaw Content Publishing Service Architecture

## Goal

Build an OpenClaw-native content publishing service that accepts a human request from Telegram, creates image-based content in a predefined category, validates quality, stores artifacts, and publishes approved content to Telegram. The system must behave like a durable state machine rather than a loose prompt chain.

## Why OpenClaw-native instead of LangGraph-native

The orchestration pattern resembles LangGraph, but the runtime should be centered on OpenClaw primitives already present in this environment:

- Telegram channel/account routing into OpenClaw agents
- manager-led multi-agent coordination
- agent-to-agent messaging
- TaskFlow-managed durable workflow state
- tool or plugin adapters for file storage, Google Drive, image generation, and publishing

This keeps the workflow aligned with the existing content gateway and avoids building a second orchestration substrate beside OpenClaw.

## Core design principles

1. Manager owns orchestration.
2. Worker agents do not own workflow transitions.
3. Workflow state is durable and explicit.
4. Artifact storage is separate from agent chat history.
5. Validation is independent from creation.
6. Publish failure is distinct from content-quality failure.
7. Human approval should default to required in v1 unless explicitly waived.

## Recommended agents

### manager
Purpose: intake, orchestration, routing, retries, final user updates.

Responsibilities:
- receive Telegram request from human
- normalize request into a content brief
- create and advance TaskFlow state
- optionally call researcher for ideation or source shaping
- assign creation to creator
- assign validation to evaluator
- decide retry, fail, approve, or publish
- send final success or failure outcome to human

### researcher (optional but useful)
Purpose: prepare brief inputs when the request is vague or research-heavy.

Responsibilities:
- suggest angles, hooks, references, and category framing
- produce concise structured brief additions
- never publish or approve

### creator
Purpose: generate candidate content artifact.

Responsibilities:
- create image-with-text artifact from brief
- emit structured artifact metadata
- store output in local draft storage
- request external storage sync when needed
- revise content on retry using evaluator feedback

### evaluator
Purpose: validate content quality independently from creator.

Responsibilities:
- assess candidate artifact against a rubric
- return pass or fail plus structured reasons
- recommend fixes when failing
- never publish and never rewrite artifacts directly

### publisher
Purpose: publish only approved artifacts.

Responsibilities:
- consume approved artifact package
- publish to target Telegram channel/account
- return publish receipt and identifiers
- never generate original content or modify approved creative direction

## Recommended runtime architecture

### Layer 1: human intake
Telegram message arrives on manager account.

Examples:
- update one social media account with new content
- update all registered social accounts
- create philosophy content in category X

Manager extracts:
- target channels/accounts
- category
- format
- tone/style
- deadline or schedule
- whether immediate publishing is allowed

### Layer 2: managed workflow
Each human request becomes one managed TaskFlow.

TaskFlow owns:
- flow identity
- owner context
- current step
- persisted state JSON
- wait state
- child task linkage
- revision-safe state transitions

Business logic remains in the manager or plugin orchestration layer.

### Layer 3: worker execution
Manager delegates bounded tasks to creator, evaluator, researcher, and publisher.

### Layer 4: storage adapters
Artifacts should be stored in two places:

#### local durable storage
Use local disk as source of truth for workflow debugging and recovery.

Suggested structure:
- `artifacts/requests/<request-id>/brief.json`
- `artifacts/requests/<request-id>/attempts/1/draft.png`
- `artifacts/requests/<request-id>/attempts/1/manifest.json`
- `artifacts/requests/<request-id>/attempts/1/evaluation.json`
- `artifacts/requests/<request-id>/ready/final.png`
- `artifacts/requests/<request-id>/published/publish-receipt.json`
- `artifacts/trash/<request-id>/attempt-1.png`

#### Google Drive
Use Drive as external asset storage and operator-facing artifact access.

Suggested folders:
- `OpenClaw Content/Drafts`
- `OpenClaw Content/Trash`
- `OpenClaw Content/Ready`
- `OpenClaw Content/Published`

Recommendation: local storage remains canonical, Drive is a synchronized external surface.

## State machine

Use normalized state fields rather than encoding every transition into the state name.

### Top-level fields
- `phase`: `intake | brief | create | evaluate | approve | publish | done | failed | cancelled`
- `status`: `running | waiting | passed | failed | cancelled`
- `attempt`: integer
- `maxAttempts`: integer, default `3`

### Typical flow
1. `phase=intake`
2. `phase=brief`
3. `phase=create`, `attempt=1`
4. `phase=evaluate`, `attempt=1`
5. on fail -> draft moved to trash -> `phase=create`, `attempt=2`
6. `phase=evaluate`, `attempt=2`
7. on fail -> draft moved to trash -> `phase=create`, `attempt=3`
8. `phase=evaluate`, `attempt=3`
9. if fail again -> `phase=failed`, `status=failed`, reason `failed_validation`
10. if pass -> `phase=approve`
11. if human approval required, wait
12. if approved -> move to ready -> `phase=publish`
13. publish success -> `phase=done`, `status=passed`
14. publish failure -> `phase=failed`, `status=failed`, reason `failed_publish`

## Retry policy

Validation retries should apply only to creation/evaluation loops.

Rules:
- max 3 creation attempts
- each failed attempt must preserve evaluation record
- failed artifact must be moved to trash, not overwritten in place
- creator receives evaluator feedback as structured revision input
- after 3 failures, mark the workflow failed and stop

## Approval policy

Recommended v1 default:
- `requireHumanApproval = true`

Possible overrides:
- explicit user instruction to publish automatically
- pre-approved category/channel policy in config

Human approval should be treated as a distinct waiting state, not mixed into evaluation.

## Error classes

Distinguish these failures:

1. `brief_error`
2. `creation_error`
3. `validation_failed`
4. `storage_error`
5. `publish_error`
6. `approval_timeout`
7. `cancelled_by_user`

This distinction will matter for operational metrics and recovery.

## Suggested future steps beyond v1

Not required for first implementation, but useful later:
- scheduling and delayed publish windows
- multi-platform publisher adapters
- A/B creative variants
- metrics on evaluator pass rate
- policy-driven category templates
- campaign grouping across multiple requests
- human inline review with previews

## Recommended v1 scope

Build first:
- manager
- creator
- evaluator
- publisher
- TaskFlow state
- local artifact store
- Google Drive sync adapter
- Telegram publishing
- retry loop with 3-attempt cap

Defer until later:
- broad multi-platform support
- analytics dashboards
- campaign calendars
- autonomous trend research loops

## Practical implementation note

OpenClaw should be the orchestration spine. Tool or plugin adapters should perform the side effects:
- image generation
- file movement
- Drive upload/move
- Telegram publish

Agents decide and explain. Tools execute and record.