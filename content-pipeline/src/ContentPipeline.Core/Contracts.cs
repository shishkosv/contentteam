namespace ContentPipeline.Core;

public sealed record WorkflowRequest(
    string RequestId,
    SourceContext Source,
    NormalizedRequest Request,
    ContentBrief Brief,
    WorkflowRuntime Runtime);

public sealed record SourceContext(
    string Channel,
    string AccountId,
    string ChatId,
    string UserId,
    string? MessageId);

public sealed record NormalizedRequest(
    string RawText,
    string NormalizedIntent,
    string Category,
    IReadOnlyList<PublishTarget> Targets,
    string Format,
    string Tone,
    PublishMode PublishMode,
    DateTimeOffset? DeadlineUtc);

public sealed record PublishTarget(
    string Platform,
    string AccountId,
    string Destination);

public sealed record ContentBrief(
    string? Title,
    string? Objective,
    string? Audience,
    IReadOnlyList<string> Constraints,
    string? VisualDirection,
    string? TextOverlay,
    string? Caption,
    IReadOnlyList<string> Hashtags,
    IReadOnlyList<string> Sources);

public sealed record WorkflowRuntime(
    int Attempt,
    int MaxAttempts,
    bool RequiresHumanApproval);

public sealed record ArtifactRecord(
    string ArtifactId,
    int Attempt,
    string Category,
    string Format,
    string LocalPath,
    string? DriveFileId,
    string ManifestPath,
    string? TextOverlay,
    string? Caption,
    string CreatedBy,
    DateTimeOffset CreatedAt,
    ArtifactStatus Status);

public sealed record EvaluationRecord(
    string ArtifactId,
    int Attempt,
    EvaluationDecision Decision,
    double? Score,
    IReadOnlyList<string> Reasons,
    IReadOnlyList<string> RequiredFixes,
    string EvaluatedBy,
    DateTimeOffset EvaluatedAt);

public sealed record PublishReceipt(
    string ArtifactId,
    string Platform,
    string AccountId,
    string Destination,
    string? MessageId,
    DateTimeOffset PublishedAt,
    string Status);
