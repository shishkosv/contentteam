# Content gateway auth fix note

## Problem

The content gateway worker agents had empty per-agent auth files:

- `~/.openclaw-content/agents/creator/agent/auth-profiles.json`
- `~/.openclaw-content/agents/manager/agent/auth-profiles.json`
- `~/.openclaw-content/agents/researcher/agent/auth-profiles.json`
- `~/.openclaw-content/agents/publisher/agent/auth-profiles.json`

As a result, live `sessions_send` calls reached the target session successfully but failed at model execution time with OAuth refresh errors.

## Symptom

Bridge probe reached the gateway and target session, but the reply payload contained:

- `FallbackSummaryError: All models failed ... OAuth token refresh failed for openai-codex ...`

## Root cause

The content profile had the logical auth profile configured in `openclaw.json`, but the actual per-agent `auth-profiles.json` files were empty.

The working main profile had populated per-agent auth files.

## Fix applied

Copied a known-good populated auth file from the main profile into content gateway agent directories:

- source: `~/.openclaw/agents/arist/agent/auth-profiles.json`
- targets:
  - `~/.openclaw-content/agents/creator/agent/auth-profiles.json`
  - `~/.openclaw-content/agents/manager/agent/auth-profiles.json`
  - `~/.openclaw-content/agents/researcher/agent/auth-profiles.json`
  - `~/.openclaw-content/agents/publisher/agent/auth-profiles.json`

## Verification

After copying auth profiles, the live bridge probe succeeded:

- target session: `agent:creator:telegram:direct:1185522850`
- reply status: `ok`
- reply payload: `{"ok":true}`

## Operational lesson

For separate OpenClaw profiles/gateways, gateway-level auth config is not sufficient by itself if the runtime expects per-agent auth files.

When cloning or creating a parallel gateway profile, verify both:
- top-level `auth.profiles` in config
- per-agent `agents/<agent>/agent/auth-profiles.json`
