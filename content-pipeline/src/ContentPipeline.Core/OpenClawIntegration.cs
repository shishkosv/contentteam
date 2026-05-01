namespace ContentPipeline.Core;

public sealed record OpenClawSessionSendRequest(
    string SessionKey,
    string Message,
    int TimeoutSeconds = 120);

public sealed record OpenClawSessionSendResponse(
    string SessionKey,
    string ReplyText);

public interface IOpenClawTransport
{
    Task<OpenClawSessionSendResponse> SendToSessionAsync(OpenClawSessionSendRequest request, CancellationToken cancellationToken = default);
}

public sealed record OpenClawAgentRoutingOptions(
    string ManagerSessionKey,
    string CreatorSessionKey,
    string EvaluatorSessionKey);
