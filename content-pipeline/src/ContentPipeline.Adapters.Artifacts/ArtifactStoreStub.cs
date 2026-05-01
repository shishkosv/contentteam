using ContentPipeline.Core;

namespace ContentPipeline.Adapters.Artifacts;

public sealed class ArtifactStoreStub : IArtifactStore
{
    public Task<ArtifactRecord> SaveDraftAsync(WorkflowRequest request, int attempt, CancellationToken cancellationToken = default)
    {
        var artifact = new ArtifactRecord(
            ArtifactId: $"art_{request.RequestId}_{attempt}",
            Attempt: attempt,
            Category: request.Request.Category,
            Format: request.Request.Format,
            LocalPath: $"/home/sergiy_shyshko/.openclaw-content/src/data/artifacts/requests/{request.RequestId}/attempts/{attempt}/draft.png",
            DriveFileId: null,
            ManifestPath: $"/home/sergiy_shyshko/.openclaw-content/src/data/artifacts/requests/{request.RequestId}/attempts/{attempt}/manifest.json",
            TextOverlay: request.Brief.TextOverlay,
            Caption: request.Brief.Caption,
            CreatedBy: "creator",
            CreatedAt: DateTimeOffset.UtcNow,
            Status: ArtifactStatus.Draft);

        return Task.FromResult(artifact);
    }

    public Task<ArtifactRecord> MoveToTrashAsync(ArtifactRecord artifact, CancellationToken cancellationToken = default)
        => Task.FromResult(artifact with
        {
            LocalPath = $"/home/sergiy_shyshko/.openclaw-content/src/data/artifacts/trash/{artifact.ArtifactId}.png",
            Status = ArtifactStatus.Trashed,
        });

    public Task<ArtifactRecord> MoveToReadyAsync(ArtifactRecord artifact, CancellationToken cancellationToken = default)
        => Task.FromResult(artifact with
        {
            LocalPath = $"/home/sergiy_shyshko/.openclaw-content/src/data/artifacts/ready/{artifact.ArtifactId}.png",
            Status = ArtifactStatus.Ready,
        });
}
