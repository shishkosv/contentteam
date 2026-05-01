using ContentPipeline.Adapters.Artifacts;
using ContentPipeline.Adapters.GoogleDrive;
using ContentPipeline.Adapters.OpenClaw;
using ContentPipeline.Adapters.Telegram;
using ContentPipeline.Core;
using ContentPipeline.Orchestration;
using ContentPipeline.Storage;

namespace ContentPipeline.Tests;

public sealed class ApprovalWorkflowTests
{
    [Fact]
    public async Task RunAsync_WhenApprovalRequiredAndApproved_CompletesWorkflow()
    {
        var orchestrator = new ContentWorkflowOrchestrator(
            new InMemoryWorkflowStateStore(),
            new ArtifactStoreStub(),
            new GoogleDriveSyncStub(),
            new OpenClawAgentClient(
                new OpenClawSessionsTransportStub(),
                new OpenClawAgentRoutingOptions(
                    ManagerSessionKey: "manager-session",
                    CreatorSessionKey: "creator-session",
                    EvaluatorSessionKey: "evaluator-session")),
            new TelegramPublisherStub(),
            new ImmediateApprovalCoordinator(approved: true));

        var request = new WorkflowRequest(
            RequestId: "req-approval-001",
            Source: new SourceContext("telegram", "manager", "chat-1", "user-1", "msg-1"),
            Request: new NormalizedRequest(
                RawText: "Create and publish a new post",
                NormalizedIntent: "create_and_publish_content",
                Category: "philosophy",
                Targets: [new PublishTarget("telegram", "publisher", "@channel")],
                Format: "image_text",
                Tone: "clear",
                PublishMode: PublishMode.ApprovalRequired,
                DeadlineUtc: null),
            Brief: new ContentBrief(
                Title: "Habit",
                Objective: "Create one image",
                Audience: "Telegram audience",
                Constraints: Array.Empty<string>(),
                VisualDirection: "minimal",
                TextOverlay: "Small daily habits shape a whole life.",
                Caption: "Short reflection.",
                Hashtags: Array.Empty<string>(),
                Sources: Array.Empty<string>()),
            Runtime: new WorkflowRuntime(
                Attempt: 0,
                MaxAttempts: 3,
                RequiresHumanApproval: true));

        var result = await orchestrator.RunAsync(request);

        Assert.Equal(WorkflowPhase.Done, result.Phase);
        Assert.Equal(WorkflowStatus.Passed, result.Status);
        Assert.NotNull(result.PublishReceipt);
    }

    [Fact]
    public async Task RunAsync_WhenApprovalRequiredAndRejected_FailsWorkflow()
    {
        var orchestrator = new ContentWorkflowOrchestrator(
            new InMemoryWorkflowStateStore(),
            new ArtifactStoreStub(),
            new GoogleDriveSyncStub(),
            new OpenClawAgentClient(
                new OpenClawSessionsTransportStub(),
                new OpenClawAgentRoutingOptions(
                    ManagerSessionKey: "manager-session",
                    CreatorSessionKey: "creator-session",
                    EvaluatorSessionKey: "evaluator-session")),
            new TelegramPublisherStub(),
            new ImmediateApprovalCoordinator(approved: false));

        var request = new WorkflowRequest(
            RequestId: "req-approval-002",
            Source: new SourceContext("telegram", "manager", "chat-1", "user-1", "msg-1"),
            Request: new NormalizedRequest(
                RawText: "Create and publish a new post",
                NormalizedIntent: "create_and_publish_content",
                Category: "philosophy",
                Targets: [new PublishTarget("telegram", "publisher", "@channel")],
                Format: "image_text",
                Tone: "clear",
                PublishMode: PublishMode.ApprovalRequired,
                DeadlineUtc: null),
            Brief: new ContentBrief(
                Title: "Habit",
                Objective: "Create one image",
                Audience: "Telegram audience",
                Constraints: Array.Empty<string>(),
                VisualDirection: "minimal",
                TextOverlay: "Small daily habits shape a whole life.",
                Caption: "Short reflection.",
                Hashtags: Array.Empty<string>(),
                Sources: Array.Empty<string>()),
            Runtime: new WorkflowRuntime(
                Attempt: 0,
                MaxAttempts: 3,
                RequiresHumanApproval: true));

        var result = await orchestrator.RunAsync(request);

        Assert.Equal(WorkflowPhase.Failed, result.Phase);
        Assert.Equal(WorkflowStatus.Failed, result.Status);
        Assert.Equal("approval_rejected", result.FailureReason);
    }
}
