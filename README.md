# contentteam

Content management team gateway setup for OpenClaw.

## What this repo contains

- `openclaw.content.json` - separate OpenClaw gateway config
- `content-gateway/` - shared gateway policies, routing, templates, and examples
- `workspaces/manager` - manager agent workspace prompt files
- `workspaces/creator` - creator agent workspace prompt files
- `workspaces/publisher` - publisher agent workspace prompt files

## Key properties

- separate gateway process
- separate port: `18889`
- separate Telegram bot accounts for manager, creator, publisher
- human requests route to manager by default
- GitHub Issues are the current task records
- GitHub Projects are the current workflow surface
- default rule: human approval required before publishing unless explicitly waived

## Important

- run this gateway separately from the philosophers gateway
- do not run the same Telegram bot token in multiple gateways
- refresh `gh` auth with `read:project` if you want exact GitHub Project ids wired in place of placeholders

## Suggested start pattern

```bash
openclaw gateway start --config /home/sergiy_shyshko/.openclaw-content/openclaw.content.json
```

## Model tiers

- manager: openai-codex/gpt-5.4
- creator: openai-codex/gpt-5.4-mini
- publisher: openai-codex/gpt-5.4-nano

## Runtime note

- content gateway runs separately on port 18889
- human-facing traffic defaults to manager
- creator requires mention in group chat
- publisher only executes approved publishing tasks


## Research agent

- researcher: openai-codex/gpt-5.4-mini
- manager remains the workflow brain and review gate for research-derived work
