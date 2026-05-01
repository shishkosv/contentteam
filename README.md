# contentteam

OpenClaw-native content publishing gateway and workflow repository.

## Goal

This repository defines a separate OpenClaw content team that accepts human requests from Telegram, creates content artifacts, validates them, stores them durably, and publishes approved outputs.

The orchestration model is now moving from a GitHub-task-centered workflow toward an OpenClaw-managed TaskFlow content pipeline.

## Current design direction

The target workflow is:
- human request arrives to `manager` on Telegram
- `manager` creates or resumes a durable workflow
- `researcher` optionally helps shape a brief
- `creator` generates a candidate artifact
- `evaluator` validates quality and returns pass or fail
- failed attempts move to trash and can retry up to 3 times
- approved content moves to ready
- `publisher` publishes approved content to Telegram
- workflow finishes with a success or failure record

## Agents

- `manager` - intake, orchestration, retries, approval handling, final status
- `researcher` - optional ideation and source shaping
- `creator` - artifact generation
- `evaluator` - independent quality gate
- `publisher` - approved publishing executor

## Repository layout

- `openclaw.content.json` - current gateway config
- `design/` - architecture, config draft, state machine, contracts
- `content-pipeline/` - implementation area for workflow orchestration
- `data/artifacts/` - canonical local artifact storage root
- `content-gateway/` - shared gateway policies and legacy workflow materials
- `workspaces/manager` - manager agent workspace prompt files
- `workspaces/researcher` - researcher agent workspace prompt files
- `workspaces/creator` - creator agent workspace prompt files
- `workspaces/evaluator` - evaluator agent workspace prompt files
- `workspaces/publisher` - publisher agent workspace prompt files
- `prompts/` - handoff and continuation prompts

## Runtime properties

- separate gateway process
- separate port: `18889`
- manager is the primary human-facing intake account
- worker agents are intended to operate under manager control
- local storage is canonical for artifact state
- Google Drive is intended as synchronized external storage
- human approval should default to required unless explicitly waived

## Design documents

- `design/content-pipeline-architecture.md`
- `design/openclaw-content-config-v2.json`
- `design/taskflow-schema-and-agent-contracts.md`

## Suggested start pattern

```bash
openclaw gateway start --config /home/sergiy_shyshko/.openclaw-content/openclaw.content.json
```

## Security note

Existing config history has contained live Telegram and gateway tokens.
Treat those credentials as exposed and rotate them before deployment.
Use placeholders in committed or shared config.

## Next implementation steps

- update the real gateway config to add `evaluator`
- implement TaskFlow orchestration in `content-pipeline/`
- add tool or plugin adapters for image generation, Drive sync, and Telegram publishing
- migrate remaining legacy docs from GitHub-only task workflow language to TaskFlow-centric workflow language where appropriate
