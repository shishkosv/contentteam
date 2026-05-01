using System.Text.Json;
using ContentPipeline.Core;

namespace ContentPipeline.Storage;

public sealed class LocalArtifactStore(string rootPath) : IArtifactStore
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web) { WriteIndented = true };

    public async Task<ArtifactRecord> SaveDraftAsync(WorkflowRequest request, int attempt, CancellationToken cancellationToken = default)
    {
        var attemptDir = Path.Combine(rootPath, "requests", request.RequestId, "attempts", attempt.ToString());
        Directory.CreateDirectory(attemptDir);

        var manifestPath = Path.Combine(attemptDir, "manifest.json");
        var draftPath = Path.Combine(attemptDir, "draft.json");

        var artifact = new ArtifactRecord(
            ArtifactId: $"art_{request.RequestId}_{attempt}",
            Attempt: attempt,
            Category: request.Request.Category,
            Format: request.Request.Format,
            LocalPath: draftPath,
            DriveFileId: null,
            ManifestPath: manifestPath,
            TextOverlay: request.Brief.TextOverlay,
            Caption: request.Brief.Caption,
            CreatedBy: "creator",
            CreatedAt: DateTimeOffset.UtcNow,
            Status: ArtifactStatus.Draft);

        await File.WriteAllTextAsync(manifestPath, JsonSerializer.Serialize(artifact, JsonOptions), cancellationToken);
        await File.WriteAllTextAsync(draftPath, JsonSerializer.Serialize(new { artifact.TextOverlay, artifact.Caption }, JsonOptions), cancellationToken);
        return artifact;
    }

    public Task<ArtifactRecord> MoveToTrashAsync(ArtifactRecord artifact, CancellationToken cancellationToken = default)
    {
        var targetPath = Path.Combine(rootPath, "trash", $"{artifact.ArtifactId}.json");
        Directory.CreateDirectory(Path.GetDirectoryName(targetPath)!);
        File.Copy(artifact.LocalPath, targetPath, overwrite: true);
        return Task.FromResult(artifact with { LocalPath = targetPath, Status = ArtifactStatus.Trashed });
    }

    public Task<ArtifactRecord> MoveToReadyAsync(ArtifactRecord artifact, CancellationToken cancellationToken = default)
    {
        var targetPath = Path.Combine(rootPath, "ready", $"{artifact.ArtifactId}.json");
        Directory.CreateDirectory(Path.GetDirectoryName(targetPath)!);
        File.Copy(artifact.LocalPath, targetPath, overwrite: true);
        return Task.FromResult(artifact with { LocalPath = targetPath, Status = ArtifactStatus.Ready });
    }
}
