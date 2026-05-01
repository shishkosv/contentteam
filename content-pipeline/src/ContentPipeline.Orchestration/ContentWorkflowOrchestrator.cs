using ContentPipeline.Core;

namespace ContentPipeline.Orchestration;

public sealed class ContentWorkflowOrchestrator(
    IWorkflowStateStore stateStore,
    IArtifactStore artifactStore,
    IDriveSync driveSync,
    IOpenClawAgentClient openClawAgentClient,
    ITelegramPublisher telegramPublisher,
    IApprovalCoordinator approvalCoordinator)
{
    public async Task<WorkflowState> RunAsync(WorkflowRequest request, CancellationToken cancellationToken = default)
    {
        var state = new WorkflowState(
            RequestId: request.RequestId,
            Phase: WorkflowPhase.Intake,
            Status: WorkflowStatus.Running,
            Attempt: 0,
            MaxAttempts: request.Runtime.MaxAttempts,
            RequiresHumanApproval: request.Runtime.RequiresHumanApproval,
            FailureReason: null,
            ActiveArtifact: null,
            Drafts: [],
            Trashed: [],
            ReadyArtifact: null,
            PublishReceipt: null,
            Evaluations: []);

        state = state with { Phase = WorkflowPhase.Brief };
        await stateStore.SaveAsync(state, cancellationToken);

        while (true)
        {
            state = await CreateAndEvaluateAsync(request, state, cancellationToken);

            var lastEvaluation = state.Evaluations.Last();
            if (lastEvaluation.Decision == EvaluationDecision.Pass)
            {
                break;
            }

            if (!state.CanRetry)
            {
                state = state with
                {
                    Phase = WorkflowPhase.Failed,
                    Status = WorkflowStatus.Failed,
                    FailureReason = "failed_validation",
                };
                await stateStore.SaveAsync(state, cancellationToken);
                return state;
            }
        }

        state = await ApproveAndPublishAsync(request, state, cancellationToken);
        return state;
    }

    private async Task<WorkflowState> CreateAndEvaluateAsync(WorkflowRequest request, WorkflowState state, CancellationToken cancellationToken)
    {
        var attempt = state.Attempt + 1;
        var revisionNotes = state.Evaluations.LastOrDefault()?.RequiredFixes ?? Array.Empty<string>();

        state = state with
        {
            Phase = WorkflowPhase.Create,
            Status = WorkflowStatus.Running,
            Attempt = attempt,
        };
        await stateStore.SaveAsync(state, cancellationToken);

        var workerArtifact = await openClawAgentClient.RequestCreationAsync(request, attempt, revisionNotes, cancellationToken)
            ?? throw new InvalidOperationException("Creator returned null artifact.");

        if (string.IsNullOrWhiteSpace(workerArtifact.ArtifactId))
        {
            throw new InvalidOperationException("Creator returned artifact without artifactId.");
        }

        var storedArtifact = await artifactStore.SaveDraftAsync(request, attempt, cancellationToken)
            ?? throw new InvalidOperationException("Artifact store returned null draft artifact.");

        var createdArtifact = storedArtifact with
        {
            ArtifactId = workerArtifact.ArtifactId,
            Category = string.IsNullOrWhiteSpace(workerArtifact.Category) ? storedArtifact.Category : workerArtifact.Category,
            Format = string.IsNullOrWhiteSpace(workerArtifact.Format) ? storedArtifact.Format : workerArtifact.Format,
            TextOverlay = workerArtifact.TextOverlay ?? storedArtifact.TextOverlay,
            Caption = workerArtifact.Caption ?? storedArtifact.Caption,
            CreatedBy = string.IsNullOrWhiteSpace(workerArtifact.CreatedBy) ? storedArtifact.CreatedBy : workerArtifact.CreatedBy,
            CreatedAt = workerArtifact.CreatedAt == default ? storedArtifact.CreatedAt : workerArtifact.CreatedAt,
            DriveFileId = workerArtifact.DriveFileId ?? storedArtifact.DriveFileId,
            ManifestPath = string.IsNullOrWhiteSpace(workerArtifact.ManifestPath) ? storedArtifact.ManifestPath : workerArtifact.ManifestPath,
            LocalPath = string.IsNullOrWhiteSpace(workerArtifact.LocalPath) ? storedArtifact.LocalPath : workerArtifact.LocalPath,
            Status = workerArtifact.Status,
        };
        var draftDriveId = await driveSync.SyncDraftAsync(createdArtifact, cancellationToken);
        if (!string.IsNullOrWhiteSpace(draftDriveId))
        {
            createdArtifact = createdArtifact with { DriveFileId = draftDriveId };
        }

        state = state with
        {
            ActiveArtifact = createdArtifact,
            Drafts = [.. state.Drafts, createdArtifact],
            Phase = WorkflowPhase.Evaluate,
        };
        await stateStore.SaveAsync(state, cancellationToken);

        var evaluation = await openClawAgentClient.RequestEvaluationAsync(request, createdArtifact, cancellationToken);
        state = state with
        {
            Evaluations = [.. state.Evaluations, evaluation],
        };
        await stateStore.SaveAsync(state, cancellationToken);

        if (evaluation.Decision == EvaluationDecision.Pass)
        {
            return state;
        }

        var trashed = await artifactStore.MoveToTrashAsync(createdArtifact, cancellationToken);
        var trashDriveId = await driveSync.SyncTrashAsync(trashed, cancellationToken);
        if (!string.IsNullOrWhiteSpace(trashDriveId))
        {
            trashed = trashed with { DriveFileId = trashDriveId };
        }

        state = state with
        {
            ActiveArtifact = null,
            Trashed = [.. state.Trashed, trashed],
        };
        await stateStore.SaveAsync(state, cancellationToken);

        return state;
    }

    private async Task<WorkflowState> ApproveAndPublishAsync(WorkflowRequest request, WorkflowState state, CancellationToken cancellationToken)
    {
        if (state.ActiveArtifact is null)
        {
            throw new InvalidOperationException("Cannot publish without an active artifact.");
        }

        if (state.RequiresHumanApproval)
        {
            state = state with
            {
                Phase = WorkflowPhase.Approve,
                Status = WorkflowStatus.Waiting,
            };
            await stateStore.SaveAsync(state, cancellationToken);

            var approvalTarget = request.Request.Targets.First();
            var decision = await approvalCoordinator.WaitForDecisionAsync(new ApprovalWait(
                RequestId: request.RequestId,
                ArtifactId: state.ActiveArtifact.ArtifactId,
                Channel: approvalTarget.Platform,
                Destination: approvalTarget.Destination,
                RequestedAt: DateTimeOffset.UtcNow,
                Reason: "Human approval required before publish."), cancellationToken);

            if (!decision.Approved)
            {
                state = state with
                {
                    Phase = WorkflowPhase.Failed,
                    Status = WorkflowStatus.Failed,
                    FailureReason = "approval_rejected",
                };
                await stateStore.SaveAsync(state, cancellationToken);
                return state;
            }
        }

        var readyArtifact = await artifactStore.MoveToReadyAsync(state.ActiveArtifact, cancellationToken);
        var readyDriveId = await driveSync.SyncReadyAsync(readyArtifact, cancellationToken);
        if (!string.IsNullOrWhiteSpace(readyDriveId))
        {
            readyArtifact = readyArtifact with { DriveFileId = readyDriveId };
        }

        state = state with
        {
            ReadyArtifact = readyArtifact,
            Phase = WorkflowPhase.Publish,
            Status = WorkflowStatus.Running,
        };
        await stateStore.SaveAsync(state, cancellationToken);

        var target = request.Request.Targets.First();
        var receipt = await telegramPublisher.PublishAsync(readyArtifact, target, cancellationToken);

        state = state with
        {
            PublishReceipt = receipt,
            Phase = WorkflowPhase.Done,
            Status = WorkflowStatus.Passed,
        };
        await stateStore.SaveAsync(state, cancellationToken);

        return state;
    }
}
