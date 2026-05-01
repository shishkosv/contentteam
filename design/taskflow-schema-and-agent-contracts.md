# TaskFlow Schema and Agent Message Contracts

## Purpose

This document defines the durable workflow state and the structured contracts passed between manager, researcher, creator, evaluator, and publisher for the OpenClaw content publishing service.

## Guiding rule

Agent conversations may be natural language, but workflow-critical payloads must be representable as structured JSON.

The manager is responsible for maintaining canonical workflow state.

## Managed TaskFlow model

Each content request becomes one managed TaskFlow.

Suggested controller id:
- `content-pipeline/v1`

## Flow lifecycle

### createManaged
Called by manager after request intake is normalized.

Suggested initial values:
- `goal`: `create and publish social content`
- `currentStep`: `intake`
- `stateJson`: see schema below

### runTask
Use for bounded child work such as:
- research brief expansion
- content creation attempt
- artifact evaluation
- publish execution

### setWaiting
Use when waiting for:
- human approval
- external publish confirmation
- delayed scheduling window

### resume
Use when a waiting condition is satisfied.

### finish
Use only when publish succeeded or a successful terminal state was reached.

### fail
Use when the workflow is terminally failed.

## Canonical stateJson schema

```json
{
  "requestId": "content-2026-04-30-001",
  "version": 1,
  "source": {
    "channel": "telegram",
    "accountId": "manager",
    "chatId": "-1001234567890",
    "userId": "123456789",
    "messageId": "42"
  },
  "request": {
    "rawText": "Create a new post for the philosophy channel",
    "normalizedIntent": "create_and_publish_content",
    "category": "philosophy",
    "targets": [
      {
        "platform": "telegram",
        "accountId": "publisher",
        "destination": "@example_channel"
      }
    ],
    "format": "image_text",
    "tone": "clear, thoughtful, concise",
    "publishMode": "approval_required",
    "deadlineUtc": null
  },
  "workflow": {
    "phase": "brief",
    "status": "running",
    "attempt": 0,
    "maxAttempts": 3,
    "currentStep": "prepare_brief",
    "failureReason": null,
    "requiresHumanApproval": true
  },
  "brief": {
    "title": null,
    "objective": null,
    "audience": null,
    "constraints": [],
    "visualDirection": null,
    "textOverlay": null,
    "caption": null,
    "hashtags": [],
    "sources": []
  },
  "artifacts": {
    "activeArtifactId": null,
    "drafts": [],
    "trashed": [],
    "ready": null,
    "published": null
  },
  "evaluation": {
    "latestDecision": null,
    "latestScore": null,
    "latestReasons": [],
    "history": []
  },
  "publish": {
    "status": null,
    "platform": null,
    "destination": null,
    "publishedAt": null,
    "receipt": null
  },
  "audit": {
    "createdAt": "2026-04-30T23:00:00Z",
    "updatedAt": "2026-04-30T23:00:00Z",
    "events": []
  }
}
```

## Artifact object schema

```json
{
  "artifactId": "art_001",
  "attempt": 1,
  "category": "philosophy",
  "format": "image_text",
  "localPath": "/home/.../artifacts/requests/content-2026-04-30-001/attempts/1/draft.png",
  "driveFileId": null,
  "manifestPath": "/home/.../manifest.json",
  "textOverlay": "Small daily habits shape a whole life.",
  "caption": "A short reflection on habit and character.",
  "createdBy": "creator",
  "createdAt": "2026-04-30T23:05:00Z",
  "status": "draft"
}
```

## Evaluation record schema

```json
{
  "artifactId": "art_001",
  "attempt": 1,
  "decision": "fail",
  "score": 0.62,
  "rubric": {
    "relevance": 0.8,
    "clarity": 0.5,
    "readability": 0.4,
    "visual_quality": 0.7,
    "brand_fit": 0.6,
    "publishability": 0.5
  },
  "reasons": [
    "Text overlay is too dense for mobile reading",
    "Contrast between background and text is insufficient"
  ],
  "requiredFixes": [
    "Reduce text length by at least 30 percent",
    "Increase text-background contrast"
  ],
  "evaluatedBy": "evaluator",
  "evaluatedAt": "2026-04-30T23:06:00Z"
}
```

## Publish receipt schema

```json
{
  "artifactId": "art_003",
  "platform": "telegram",
  "accountId": "publisher",
  "destination": "@example_channel",
  "messageId": "314",
  "publishedAt": "2026-04-30T23:10:00Z",
  "status": "published"
}
```

## Manager to researcher contract

Use when the human request is too vague or needs angle exploration.

### Input
```json
{
  "type": "research_request",
  "requestId": "content-2026-04-30-001",
  "category": "philosophy",
  "goal": "propose 3 post directions for a Telegram philosophy audience",
  "constraints": [
    "must fit image+text format",
    "must be concise"
  ]
}
```

### Output
```json
{
  "type": "research_result",
  "requestId": "content-2026-04-30-001",
  "directions": [
    {
      "title": "Habit and character",
      "hook": "Small daily habits shape a whole life.",
      "notes": ["strong fit for Aristotle theme"]
    }
  ]
}
```

## Manager to creator contract

### Input
```json
{
  "type": "creation_request",
  "requestId": "content-2026-04-30-001",
  "attempt": 1,
  "category": "philosophy",
  "format": "image_text",
  "brief": {
    "objective": "create one publishable Telegram image post",
    "audience": "people interested in philosophy and psychology",
    "visualDirection": "minimalist, high contrast, calm",
    "textOverlay": "Small daily habits shape a whole life.",
    "caption": "A short reflection on habit and character."
  },
  "revisionFromPreviousAttempt": null
}
```

### Output
```json
{
  "type": "creation_result",
  "requestId": "content-2026-04-30-001",
  "attempt": 1,
  "status": "created",
  "artifact": {
    "artifactId": "art_001",
    "attempt": 1,
    "category": "philosophy",
    "format": "image_text",
    "localPath": "/home/.../draft.png",
    "driveFileId": null,
    "manifestPath": "/home/.../manifest.json",
    "textOverlay": "Small daily habits shape a whole life.",
    "caption": "A short reflection on habit and character.",
    "createdBy": "creator",
    "createdAt": "2026-04-30T23:05:00Z",
    "status": "draft"
  }
}
```

## Manager to evaluator contract

### Input
```json
{
  "type": "evaluation_request",
  "requestId": "content-2026-04-30-001",
  "attempt": 1,
  "artifact": {
    "artifactId": "art_001",
    "localPath": "/home/.../draft.png",
    "textOverlay": "Small daily habits shape a whole life.",
    "caption": "A short reflection on habit and character."
  },
  "rubric": {
    "relevance": 0.2,
    "clarity": 0.2,
    "readability": 0.2,
    "visual_quality": 0.15,
    "brand_fit": 0.15,
    "publishability": 0.1
  }
}
```

### Output
```json
{
  "type": "evaluation_result",
  "requestId": "content-2026-04-30-001",
  "attempt": 1,
  "decision": "fail",
  "record": {
    "artifactId": "art_001",
    "attempt": 1,
    "decision": "fail",
    "score": 0.62,
    "rubric": {
      "relevance": 0.8,
      "clarity": 0.5,
      "readability": 0.4,
      "visual_quality": 0.7,
      "brand_fit": 0.6,
      "publishability": 0.5
    },
    "reasons": [
      "Text overlay is too dense for mobile reading"
    ],
    "requiredFixes": [
      "Shorten overlay text and increase contrast"
    ],
    "evaluatedBy": "evaluator",
    "evaluatedAt": "2026-04-30T23:06:00Z"
  }
}
```

## Manager retry contract back to creator

### Input
```json
{
  "type": "creation_retry_request",
  "requestId": "content-2026-04-30-001",
  "attempt": 2,
  "brief": {
    "objective": "create one publishable Telegram image post",
    "visualDirection": "minimalist, high contrast, calm",
    "textOverlay": "Small daily habits shape a whole life.",
    "caption": "A short reflection on habit and character."
  },
  "revisionFromPreviousAttempt": {
    "failedArtifactId": "art_001",
    "reasons": [
      "Text overlay is too dense for mobile reading"
    ],
    "requiredFixes": [
      "Shorten overlay text and increase contrast"
    ]
  }
}
```

## Manager to publisher contract

### Input
```json
{
  "type": "publish_request",
  "requestId": "content-2026-04-30-001",
  "artifact": {
    "artifactId": "art_003",
    "localPath": "/home/.../ready/final.png",
    "caption": "A short reflection on habit and character."
  },
  "target": {
    "platform": "telegram",
    "accountId": "publisher",
    "destination": "@example_channel"
  }
}
```

### Output
```json
{
  "type": "publish_result",
  "requestId": "content-2026-04-30-001",
  "status": "published",
  "receipt": {
    "artifactId": "art_003",
    "platform": "telegram",
    "accountId": "publisher",
    "destination": "@example_channel",
    "messageId": "314",
    "publishedAt": "2026-04-30T23:10:00Z",
    "status": "published"
  }
}
```

## State transition rules

### On creation success
- set `workflow.phase = create`
- append artifact to `artifacts.drafts`
- set `artifacts.activeArtifactId`
- set `workflow.currentStep = evaluate_artifact`

### On evaluation fail and attempts remaining
- append evaluation record to history
- move artifact from draft to trash
- append artifact to `artifacts.trashed`
- increment `workflow.attempt`
- set `workflow.phase = create`
- issue retry request to creator

### On evaluation fail and no attempts remaining
- append evaluation record to history
- move artifact to trash
- set `workflow.phase = failed`
- set `workflow.status = failed`
- set `workflow.failureReason = failed_validation`

### On evaluation pass
- append evaluation record to history
- set `workflow.phase = approve`
- if approval required, enter waiting state
- otherwise move directly to ready and publish

### On human approval
- move approved artifact to ready
- set `artifacts.ready`
- set `workflow.phase = publish`

### On publish success
- set `publish.status = published`
- set `artifacts.published`
- set `workflow.phase = done`
- set `workflow.status = passed`

### On publish failure
- preserve ready artifact
- set `workflow.phase = failed`
- set `workflow.status = failed`
- set `workflow.failureReason = failed_publish`

## Waiting state examples

### Human approval waitJson
```json
{
  "kind": "human_approval",
  "channel": "telegram",
  "chatId": "-1001234567890",
  "requestId": "content-2026-04-30-001",
  "artifactId": "art_003"
}
```

### Scheduled publish waitJson
```json
{
  "kind": "scheduled_publish",
  "resumeAt": "2026-05-01T08:00:00Z",
  "requestId": "content-2026-04-30-001"
}
```

## Operational recommendations

- Persist every artifact attempt separately.
- Never overwrite a failed artifact in place.
- Keep agent output concise, but structured payloads complete.
- Treat manager as the only authority for state transitions.
- Treat evaluator as independent and non-publishing.
- Keep publish receipts for auditability.
