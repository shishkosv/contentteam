# OpenClaw adapter

This project contains the OpenClaw-facing worker orchestration adapter.

## Current implementation

Implemented now:
- `OpenClawAgentClient` that turns orchestration requests into JSON worker prompts
- `IOpenClawTransport` abstraction for session messaging
- routing options for manager, creator, evaluator session keys
- stub transport for local testing without live OpenClaw calls

## Intended live integration

A real transport should call OpenClaw session messaging, likely through gateway-exposed session APIs or a thin integration layer that can invoke `sessions_send` semantics safely.

Expected pattern:
- manager session receives human request outside this adapter
- orchestrator sends creator/evaluator work requests to their OpenClaw sessions
- worker sessions return strict JSON only
- adapter parses JSON into typed C# contracts

## Important constraint

Worker prompts instruct agents to return JSON only. In production, the live transport should reject non-JSON replies or wrap them in retry/error handling.
