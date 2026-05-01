namespace ContentPipeline.Core;

public sealed record WorkflowState(
    string RequestId,
    WorkflowPhase Phase,
    WorkflowStatus Status,
    int Attempt,
    int MaxAttempts,
    bool RequiresHumanApproval,
    string? FailureReason,
    ArtifactRecord? ActiveArtifact,
    IReadOnlyList<ArtifactRecord> Drafts,
    IReadOnlyList<ArtifactRecord> Trashed,
    ArtifactRecord? ReadyArtifact,
    PublishReceipt? PublishReceipt,
    IReadOnlyList<EvaluationRecord> Evaluations)
{
    public bool CanRetry => Attempt < MaxAttempts;
}
