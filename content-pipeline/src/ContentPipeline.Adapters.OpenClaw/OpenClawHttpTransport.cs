using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ContentPipeline.Core;

namespace ContentPipeline.Adapters.OpenClaw;

public sealed class OpenClawHttpTransport(HttpClient httpClient, OpenClawAdapterOptions options) : IOpenClawTransport
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<OpenClawSessionSendResponse> SendToSessionAsync(OpenClawSessionSendRequest request, CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Post, BuildToolsInvokeUrl(options.BaseUrl));
        message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", options.GatewayToken);
        message.Content = new StringContent(JsonSerializer.Serialize(new
        {
            tool = "sessions_send",
            args = new
            {
                sessionKey = request.SessionKey,
                message = request.Message,
                timeoutSeconds = request.TimeoutSeconds,
            }
        }, JsonOptions), Encoding.UTF8, "application/json");

        using var response = await httpClient.SendAsync(message, cancellationToken);
        var json = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"OpenClaw HTTP {(int)response.StatusCode} calling /tools/invoke: {json}");
        }

        var parsed = JsonSerializer.Deserialize<ToolsInvokeResponse>(json, JsonOptions)
            ?? throw new InvalidOperationException("OpenClaw tools/invoke response was empty.");

        if (!parsed.Ok)
        {
            throw new InvalidOperationException($"OpenClaw tools/invoke returned error: {parsed.Error?.Message ?? "unknown error"}");
        }

        var replyText = parsed.Result?.Details?.ReplyText
            ?? parsed.Result?.Details?.AssistantReply
            ?? parsed.Result?.Content?.FirstOrDefault(x => x.Type == "text")?.Text
            ?? throw new InvalidOperationException("OpenClaw sessions_send response did not contain assistant reply text.");

        var sessionKey = parsed.Result?.Details?.SessionKey ?? request.SessionKey;
        return new OpenClawSessionSendResponse(sessionKey, replyText);
    }

    private static string BuildToolsInvokeUrl(string baseUrl)
        => $"{baseUrl.TrimEnd('/')}/tools/invoke";

    private sealed record ToolsInvokeResponse(
        bool Ok,
        ToolsInvokeResult? Result,
        ToolsInvokeError? Error);

    private sealed record ToolsInvokeResult(
        IReadOnlyList<ToolContentItem>? Content,
        ToolsInvokeDetails? Details);

    private sealed record ToolContentItem(
        string? Type,
        string? Text);

    private sealed record ToolsInvokeDetails(
        string? SessionKey,
        string? ReplyText,
        string? AssistantReply);

    private sealed record ToolsInvokeError(
        string? Type,
        string? Message);
}
