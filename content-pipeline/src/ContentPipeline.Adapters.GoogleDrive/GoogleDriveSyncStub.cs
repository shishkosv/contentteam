using ContentPipeline.Core;

namespace ContentPipeline.Adapters.GoogleDrive;

public sealed class GoogleDriveSyncStub : IDriveSync
{
    public Task<string?> SyncDraftAsync(ArtifactRecord artifact, CancellationToken cancellationToken = default)
        => Task.FromResult<string?>($"drive-draft-{artifact.ArtifactId}");

    public Task<string?> SyncTrashAsync(ArtifactRecord artifact, CancellationToken cancellationToken = default)
        => Task.FromResult<string?>($"drive-trash-{artifact.ArtifactId}");

    public Task<string?> SyncReadyAsync(ArtifactRecord artifact, CancellationToken cancellationToken = default)
        => Task.FromResult<string?>($"drive-ready-{artifact.ArtifactId}");
}
