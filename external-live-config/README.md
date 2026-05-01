# External Live Config Mirror

This folder mirrors important content-gateway files that are live outside the git repo.

Purpose:
- keep the real runtime prompt/config shape versioned in the repo
- document external source-of-truth files that affect the running gateway
- avoid committing live secrets directly

Notes:
- `openclaw.redacted.json` is a redacted mirror of `~/.openclaw-content/openclaw.json`
- workspace and gateway prompt files here are copied from the live external paths
- if live external files change, this mirror should be updated intentionally
