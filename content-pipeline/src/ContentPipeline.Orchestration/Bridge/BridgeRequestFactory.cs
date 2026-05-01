using ContentPipeline.Core;

namespace ContentPipeline.Orchestration.Bridge;

public static class BridgeRequestFactory
{
    public static WorkflowRequest CreateMinimal(string requestId, string chatId, string userId, string text, string destination, bool requiresApproval)
    {
        return new WorkflowRequest(
            RequestId: requestId,
            Source: new SourceContext("telegram", "manager", chatId, userId, null),
            Request: new NormalizedRequest(
                RawText: text,
                NormalizedIntent: "create_and_publish_content",
                Category: "philosophy",
                Targets: [new PublishTarget("telegram", "publisher", destination)],
                Format: "image_text",
                Tone: "clear",
                PublishMode: requiresApproval ? PublishMode.ApprovalRequired : PublishMode.AutoPublish,
                DeadlineUtc: null),
            Brief: new ContentBrief(
                Title: null,
                Objective: "Create one publishable image post",
                Audience: "Telegram audience",
                Constraints: Array.Empty<string>(),
                VisualDirection: "minimalist, high contrast",
                TextOverlay: "Small daily habits shape a whole life.",
                Caption: "Short reflection.",
                Hashtags: Array.Empty<string>(),
                Sources: Array.Empty<string>()),
            Runtime: new WorkflowRuntime(
                Attempt: 0,
                MaxAttempts: 3,
                RequiresHumanApproval: requiresApproval));
    }
}
