using ContentPipeline.Core;

namespace ContentPipeline.Adapters.OpenClaw;

public sealed class OpenClawAgentClientStub : IOpenClawAgentClient
{
    public Task<ArtifactRecord> RequestCreationAsync(WorkflowRequest request, int attempt, IReadOnlyList<string> revisionNotes, CancellationToken cancellationToken = default)
    {
        var artifact = new ArtifactRecord(
            ArtifactId: $"art_{request.RequestId}_{attempt}",
            Attempt: attempt,
            Category: request.Request.Category,
            Format: request.Request.Format,
            LocalPath: $"/tmp/{request.RequestId}/attempt-{attempt}/draft.png",
            DriveFileId: null,
            ManifestPath: $"/tmp/{request.RequestId}/attempt-{attempt}/manifest.json",
            TextOverlay: request.Brief.TextOverlay,
            Caption: request.Brief.Caption,
            CreatedBy: "creator",
            CreatedAt: DateTimeOffset.UtcNow,
            Status: ArtifactStatus.Draft);

        return Task.FromResult(artifact);
    }

    public Task<EvaluationRecord> RequestEvaluationAsync(WorkflowRequest request, ArtifactRecord artifact, CancellationToken cancellationToken = default)
    {
        var evaluation = new EvaluationRecord(
            ArtifactId: artifact.ArtifactId,
            Attempt: artifact.Attempt,
            Decision: EvaluationDecision.Pass,
            Score: 0.9,
            Reasons: ["Stub evaluator passed artifact."],
            RequiredFixes: Array.Empty<string>(),
            EvaluatedBy: "evaluator",
            EvaluatedAt: DateTimeOffset.UtcNow);

        return Task.FromResult(evaluation);
    }
}
