using ContentPipeline.Core;

namespace ContentPipeline.Storage;

public sealed class ImmediateApprovalCoordinator(bool approved = true) : IApprovalCoordinator
{
    public Task<ApprovalDecision> WaitForDecisionAsync(ApprovalWait wait, CancellationToken cancellationToken = default)
        => Task.FromResult(new ApprovalDecision(
            RequestId: wait.RequestId,
            Approved: approved,
            Comment: approved ? "Approved by stub coordinator." : "Rejected by stub coordinator.",
            DecidedAt: DateTimeOffset.UtcNow));
}
