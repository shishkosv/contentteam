# GitHub Issue Backend

## Purpose

Allow `manager` to create GitHub Issues for content tasks.

## Rules

- `manager` creates issues only.
- `creator` and `publisher` do not create issues.
- On create, write issue title, body, labels, and milestone/project fields if available.
- Before create, check for duplicates by title/search terms.
- Use the shared issue template.
- Keep the backend abstract enough to swap later.

## Required Inputs

- repository
- title
- body fields
- labels
- project id or project placeholder
- owner agent
- task metadata

## Failure Handling

- If GitHub auth is missing, block and escalate to manager.
- If duplicate is found, link existing issue instead of creating a new one.
