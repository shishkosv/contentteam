# ContentPipeline.Bridge

Tiny executable bridge app for the C# content pipeline.

## Modes

### stub
Runs the workflow with stub OpenClaw transport.

```bash
dotnet run --project src/ContentPipeline.Bridge -- stub
```

### live
Runs the workflow with the live OpenClaw HTTP transport.

```bash
OPENCLAW_GATEWAY_TOKEN=... \
OPENCLAW_BASE_URL=http://127.0.0.1:18889 \
OPENCLAW_CREATOR_SESSION=... \
OPENCLAW_EVALUATOR_SESSION=... \
dotnet run --project src/ContentPipeline.Bridge -- live
```

### probe
Sends one direct transport request to a target session and prints the raw reply.

```bash
OPENCLAW_GATEWAY_TOKEN=... \
OPENCLAW_BASE_URL=http://127.0.0.1:18889 \
dotnet run --project src/ContentPipeline.Bridge -- probe <sessionKey> 'Return exactly this JSON and nothing else: {"ok":true}'
```

## Confirmed live contract

The gateway expects:

- `POST /tools/invoke`
- JSON body with:
  - `tool: "sessions_send"`
  - `args: { sessionKey, message, timeoutSeconds }`

Important: the field is `args`, not `arguments`.

### live-flow
Runs a live creator/evaluator workflow probe through the orchestrator using the live OpenClaw transport.

```bash
OPENCLAW_GATEWAY_TOKEN=... \
OPENCLAW_BASE_URL=http://127.0.0.1:18889 \
dotnet run --project src/ContentPipeline.Bridge -- live-flow
```

## Note

The probe mode exists specifically to verify that contract end to end. A successful HTTP/tool response can still contain an agent-side runtime error in the returned JSON payload, for example model authentication failures in the target agent session.

The `live-flow` mode currently depends on worker sessions returning strict JSON matching the content-pipeline contracts. It is useful for validating creator/evaluator wiring even before full publishing is live.
