# OpenClaw-managed orchestration integration

## Intended control flow

1. OpenClaw `manager` receives a Telegram request.
2. Manager or a thin bridge converts that request into a `WorkflowRequest` object.
3. `ContentWorkflowOrchestrator` runs the state machine in C#.
4. The orchestrator uses `OpenClawAgentClient` to send JSON-only work requests to:
   - creator session
   - evaluator session
5. Worker responses are parsed back into typed contracts.
6. The orchestrator calls artifact, Drive, and Telegram adapters.
7. Final workflow state is persisted and can be reported back to manager.

## Live adapter shape

The real OpenClaw adapter path now consists of:
- `IOpenClawTransport` in `ContentPipeline.Core`
- `OpenClawAgentClient` in `ContentPipeline.Adapters.OpenClaw`
- `OpenClawHttpTransport` as the HTTP transport candidate
- `OpenClawAgentRoutingOptions` for creator/evaluator session routing

## Current status

Implemented now:
- typed transport abstraction
- JSON prompt generation for creator/evaluator
- JSON response parsing into C# records
- stub transport for test execution
- HTTP transport skeleton for future live gateway calls

Not implemented yet:
- confirmed production endpoint contract for the gateway-exposed `sessions_send` path
- manager callback/update path
- approval wait/resume handshake
- richer worker error handling and re-prompt strategy

## Practical recommendation

Use the stub transport while building orchestration logic and tests.
Switch to `OpenClawHttpTransport` only after verifying the exact gateway endpoint and response shape in the target environment.
