namespace ContentPipeline.Core;

public sealed record ApprovalWait(
    string RequestId,
    string ArtifactId,
    string Channel,
    string Destination,
    DateTimeOffset RequestedAt,
    string Reason);

public sealed record ApprovalDecision(
    string RequestId,
    bool Approved,
    string? Comment,
    DateTimeOffset DecidedAt);

public interface IApprovalCoordinator
{
    Task<ApprovalDecision> WaitForDecisionAsync(ApprovalWait wait, CancellationToken cancellationToken = default);
}
