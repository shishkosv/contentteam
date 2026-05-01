# Bridge

This folder contains the first small manager-side bridge surface for the C# orchestrator.

## Purpose

Provide a minimal integration boundary where a manager-facing caller can:
- build a `WorkflowRequest`
- invoke the orchestrator
- receive final workflow state

## Current contents

- `BridgeRequestFactory` - creates a minimal workflow request from manager-side inputs
- `WorkflowRunner` - thin wrapper over `ContentWorkflowOrchestrator`

## Next likely step

Turn this into a small console app or HTTP service that the OpenClaw manager can call directly.
