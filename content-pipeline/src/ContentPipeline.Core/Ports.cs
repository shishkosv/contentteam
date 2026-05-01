namespace ContentPipeline.Core;

public interface IWorkflowStateStore
{
    Task SaveAsync(WorkflowState state, CancellationToken cancellationToken = default);
    Task<WorkflowState?> LoadAsync(string requestId, CancellationToken cancellationToken = default);
}

public interface IArtifactStore
{
    Task<ArtifactRecord> SaveDraftAsync(WorkflowRequest request, int attempt, CancellationToken cancellationToken = default);
    Task<ArtifactRecord> MoveToTrashAsync(ArtifactRecord artifact, CancellationToken cancellationToken = default);
    Task<ArtifactRecord> MoveToReadyAsync(ArtifactRecord artifact, CancellationToken cancellationToken = default);
}

public interface IDriveSync
{
    Task<string?> SyncDraftAsync(ArtifactRecord artifact, CancellationToken cancellationToken = default);
    Task<string?> SyncTrashAsync(ArtifactRecord artifact, CancellationToken cancellationToken = default);
    Task<string?> SyncReadyAsync(ArtifactRecord artifact, CancellationToken cancellationToken = default);
}

public interface IOpenClawAgentClient
{
    Task<ArtifactRecord> RequestCreationAsync(WorkflowRequest request, int attempt, IReadOnlyList<string> revisionNotes, CancellationToken cancellationToken = default);
    Task<EvaluationRecord> RequestEvaluationAsync(WorkflowRequest request, ArtifactRecord artifact, CancellationToken cancellationToken = default);
}

public interface ITelegramPublisher
{
    Task<PublishReceipt> PublishAsync(ArtifactRecord artifact, PublishTarget target, CancellationToken cancellationToken = default);
}
