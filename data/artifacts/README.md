# Artifact Storage Conventions

This directory is the canonical local storage root for the content publishing pipeline.

## Principles

- local storage is the workflow source of truth
- Google Drive is a synchronized external surface, not the canonical state store
- failed artifacts are preserved in trash for auditability
- each request keeps separate attempt history

## Layout

- `requests/` - per-request working history
- `trash/` - failed attempts moved out of active flow
- `ready/` - approved assets ready for publish or awaiting publish
- `published/` - published artifacts and publish receipts

## Suggested per-request structure

```text
requests/<request-id>/
  brief.json
  attempts/
    1/
      draft.png
      manifest.json
      evaluation.json
    2/
      draft.png
      manifest.json
      evaluation.json
  ready/
    final.png
    manifest.json
  published/
    publish-receipt.json
```

## Rules

- never overwrite a failed draft in place
- every attempt should have its own manifest and evaluation record
- publish receipts should be preserved for auditability
- artifact ids and request ids should match TaskFlow state where possible
