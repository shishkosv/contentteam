# content-pipeline

OpenClaw-managed orchestration implemented in C#.

## Purpose

This directory now contains a first-pass C# solution for the content publishing workflow.

The intended execution model is:
- OpenClaw `manager` receives the human Telegram request
- manager normalizes it into a workflow request payload
- C# orchestrator runs the content pipeline state machine
- OpenClaw worker agents perform creation and evaluation work
- adapters handle artifact storage, Drive sync, and Telegram publish

## Solution layout

- `ContentPipeline.sln`
- `src/ContentPipeline.Core` - workflow primitives, contracts, ports, state
- `src/ContentPipeline.Orchestration` - orchestrator implementation
- `src/ContentPipeline.Adapters.OpenClaw` - OpenClaw worker client stubs
- `src/ContentPipeline.Adapters.Telegram` - Telegram publisher stubs
- `src/ContentPipeline.Adapters.GoogleDrive` - Google Drive sync stubs
- `src/ContentPipeline.Adapters.Artifacts` - local artifact store stubs
- `src/ContentPipeline.Storage` - workflow state store
- `src/ContentPipeline.Tests` - orchestration tests

## Implemented orchestration skeleton

The current orchestrator implements a first-pass happy-path loop:
1. initialize workflow state
2. enter brief phase
3. create artifact attempt
4. sync draft
5. evaluate artifact
6. if fail, move artifact to trash and retry up to max attempts
7. if pass, move to approval or publish path
8. if auto-publish, move to ready and publish
9. persist final workflow state

## Current limitations

This is a scaffold, not a finished service.

Implemented now:
- typed OpenClaw transport abstraction
- confirmed live HTTP transport contract against `/tools/invoke`
- manager-side bridge helpers for building and running workflow requests
- approval coordinator abstraction with stub approve/reject behavior
- tiny executable bridge app with probe mode

Not implemented yet:
- robust handling of agent-side runtime/model-auth failures returned by live worker calls
- real manager callback/update path
- real Telegram publish integration
- real Google Drive integration
- real local file writes and manifest generation
- durable database-backed state store
- long-lived wait/resume persistence beyond the stub coordinator
- scheduler / resume semantics
- multi-target publish fan-out
- structured logging and metrics

## Verification

Current solution builds and tests successfully with:

```bash
dotnet test /home/sergiy_shyshko/.openclaw-content/src/content-pipeline/ContentPipeline.sln
```

## Next recommended implementation steps

- replace remaining stubs with concrete adapters
- turn the bridge helpers into a console app or HTTP service
- implement persisted approval wait/resume handling
- add persistence beyond in-memory storage
- map OpenClaw agent messages to the JSON contracts in `../design/taskflow-schema-and-agent-contracts.md`
