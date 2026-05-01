using System.Text.Json;
using ContentPipeline.Core;

namespace ContentPipeline.Adapters.OpenClaw;

internal static class OpenClawResponseParser
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public static string ExtractJsonPayload(string rawReplyText, string workerRole)
    {
        if (string.IsNullOrWhiteSpace(rawReplyText))
        {
            throw new WorkerCallException(workerRole, $"{workerRole} returned an empty reply.", rawReplyText);
        }

        var trimmed = rawReplyText.Trim();
        if (trimmed.StartsWith("{"))
        {
            try
            {
                var envelope = JsonSerializer.Deserialize<WorkerExecutionEnvelope>(trimmed, JsonOptions);
                if (envelope is not null && !string.IsNullOrWhiteSpace(envelope.Status))
                {
                    if (string.Equals(envelope.Status, "ok", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(envelope.Reply))
                    {
                        return envelope.Reply!;
                    }

                    if (string.Equals(envelope.Status, "error", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new WorkerCallException(workerRole, $"{workerRole} run failed: {envelope.Error ?? "unknown error"}", rawReplyText);
                    }
                }
            }
            catch (JsonException)
            {
                // fall through, raw reply may itself be the expected contract payload
            }
        }

        return rawReplyText;
    }
}
