# Sample Tasks

Repository: `shishkosv/contentteam`
Project placeholder: `CONTENTTEAM-PROJECT-1 / Content Team Operations`

## Example 1: Content Creation Task

### Title
[TASK] Create Instagram and Telegram image package for spring AI course promo

### Task Metadata

- task_id: CT-104
- parent_task_id: CT-100
- project_id: CONTENTTEAM-PROJECT-1
- project_name: Content Team Operations
- repository: shishkosv/contentteam
- campaign_id: SPRING-AI-2026
- owner_agent: creator
- status: ready
- priority: high
- task_type: image
- target_channels: instagram, telegram
- approval_status: required

### Objective

Create a visual content package for promoting the spring AI course.

## Inputs

- campaign theme: practical AI for creators
- target audience: beginner to intermediate creators
- required formats: square post plus telegram-friendly version
- message angle: clear benefits, not hype
- brand constraints: use existing palette and modern clean feel

## Acceptance Criteria

- [ ] 3 image concepts produced
- [ ] 3 image generation prompts produced
- [ ] overlay text options included
- [ ] final recommended overlay text included
- [ ] Instagram caption variant included
- [ ] Telegram caption variant included
- [ ] risks or assumptions documented

## Expected Outputs

- concept set
- prompt set
- overlay text set
- final recommended copy package

## Artifact Links

- concept doc: <link>
- prompt doc: <link>
- copy doc: <link>

## Dependencies

- parent task CT-100
- brand asset pack issue #81

## Approval

- default_rule: human approval required before external publishing unless explicitly waived
- current_state: required

## Notes

Ready for creator execution.

## Example 2: Publish Task

### Title
[TASK] Publish approved spring AI promo to Instagram and Telegram

### Task Metadata

- task_id: CT-105
- parent_task_id: CT-100
- project_id: CONTENTTEAM-PROJECT-1
- project_name: Content Team Operations
- repository: shishkosv/contentteam
- campaign_id: SPRING-AI-2026
- owner_agent: publisher
- status: approved
- priority: high
- task_type: publish
- target_channels: instagram, telegram
- approval_status: approved

## Objective

Publish the approved content package to Instagram and Telegram.

## Inputs

- final image asset: <link>
- approved Telegram caption: <link>
- approved Instagram caption: <link>
- approval reference: issue comment link
- desired publish window: 2026-04-23 09:00 UTC

## Acceptance Criteria

- [ ] Instagram publish attempted and result recorded
- [ ] Telegram publish attempted and result recorded
- [ ] exact timestamps recorded
- [ ] URL or post ID recorded where available
- [ ] failures recorded with exact error details

## Expected Outputs

- live post records
- execution log in issue comments
- final task disposition

## Artifact Links

- final asset: <link>
- captions package: <link>

## Dependencies

- approval comment from manager
- final asset package complete

## Approval

- default_rule: human approval required before external publishing unless explicitly waived
- current_state: approved
- approved_by: human
- approval_timestamp: 2026-04-22T15:00:00Z

## Notes

Do not modify captions during execution.
