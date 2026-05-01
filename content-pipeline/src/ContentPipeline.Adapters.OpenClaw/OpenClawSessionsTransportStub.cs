using ContentPipeline.Core;

namespace ContentPipeline.Adapters.OpenClaw;

public sealed class OpenClawSessionsTransportStub : IOpenClawTransport
{
    public Task<OpenClawSessionSendResponse> SendToSessionAsync(OpenClawSessionSendRequest request, CancellationToken cancellationToken = default)
    {
        var reply = request.SessionKey.Contains("evaluator", StringComparison.OrdinalIgnoreCase)
            ? "{\"record\":{\"artifactId\":\"art_req-001_1\",\"attempt\":1,\"decision\":0,\"score\":0.91,\"reasons\":[\"Stub evaluator passed artifact.\"],\"requiredFixes\":[],\"evaluatedBy\":\"evaluator\",\"evaluatedAt\":\"2026-04-30T23:30:00Z\"}}"
            : "{\"artifact\":{\"artifactId\":\"art_req-001_1\",\"attempt\":1,\"category\":\"philosophy\",\"format\":\"image_text\",\"localPath\":\"/tmp/req-001/attempt-1/draft.png\",\"driveFileId\":null,\"manifestPath\":\"/tmp/req-001/attempt-1/manifest.json\",\"textOverlay\":\"Small daily habits shape a whole life.\",\"caption\":\"Short reflection.\",\"createdBy\":\"creator\",\"createdAt\":\"2026-04-30T23:29:00Z\",\"status\":0}}";

        return Task.FromResult(new OpenClawSessionSendResponse(request.SessionKey, reply));
    }
}
