using System.Text.Json;
using ContentPipeline.Core;

namespace ContentPipeline.Adapters.OpenClaw;

public sealed class OpenClawAgentClient(
    IOpenClawTransport transport,
    OpenClawAgentRoutingOptions routingOptions) : IOpenClawAgentClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = false,
    };

    public async Task<ArtifactRecord> RequestCreationAsync(WorkflowRequest request, int attempt, IReadOnlyList<string> revisionNotes, CancellationToken cancellationToken = default)
    {
        var payload = new
        {
            type = attempt == 1 ? "creation_request" : "creation_retry_request",
            requestId = request.RequestId,
            attempt,
            category = request.Request.Category,
            format = request.Request.Format,
            brief = request.Brief,
            revisionFromPreviousAttempt = revisionNotes,
        };

        var response = await transport.SendToSessionAsync(new OpenClawSessionSendRequest(
            SessionKey: routingOptions.CreatorSessionKey,
            Message: BuildPrompt("creator", payload)), cancellationToken);

        var result = DeserializeOrThrow<CreationResponseEnvelope>(OpenClawResponseParser.ExtractJsonPayload(response.ReplyText, "creator"), "creator");
        return result.Artifact;
    }

    public async Task<EvaluationRecord> RequestEvaluationAsync(WorkflowRequest request, ArtifactRecord artifact, CancellationToken cancellationToken = default)
    {
        var payload = new
        {
            type = "evaluation_request",
            requestId = request.RequestId,
            attempt = artifact.Attempt,
            artifact,
            rubric = new
            {
                relevance = 0.20,
                clarity = 0.20,
                readability = 0.20,
                visual_quality = 0.15,
                brand_fit = 0.15,
                publishability = 0.10,
            }
        };

        var response = await transport.SendToSessionAsync(new OpenClawSessionSendRequest(
            SessionKey: routingOptions.EvaluatorSessionKey,
            Message: BuildPrompt("evaluator", payload)), cancellationToken);

        var result = DeserializeOrThrow<EvaluationResponseEnvelope>(OpenClawResponseParser.ExtractJsonPayload(response.ReplyText, "evaluator"), "evaluator");
        return result.Record;
    }

    private static string BuildPrompt(string agentRole, object payload)
    {
        var json = JsonSerializer.Serialize(payload, JsonOptions);
        return $"Return only valid JSON. You are acting as the {agentRole} worker for the OpenClaw content pipeline. Process this request and respond with JSON matching the agreed contract.\n\n{json}";
    }

    private static T DeserializeOrThrow<T>(string replyText, string agentRole)
    {
        try
        {
            var result = JsonSerializer.Deserialize<T>(replyText, JsonOptions);
            if (result is null)
            {
                throw new WorkerCallException(agentRole, $"{agentRole} returned empty JSON.", replyText);
            }

            if (result is CreationResponseEnvelope creation && creation.Artifact is null)
            {
                throw new WorkerCallException(agentRole, $"{agentRole} response did not contain artifact.", replyText);
            }

            if (result is EvaluationResponseEnvelope evaluation && evaluation.Record is null)
            {
                throw new WorkerCallException(agentRole, $"{agentRole} response did not contain record.", replyText);
            }

            return result;
        }
        catch (WorkerCallException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new WorkerCallException(agentRole, $"Failed to parse {agentRole} JSON response.", replyText, ex);
        }
    }

    private sealed record CreationResponseEnvelope(ArtifactRecord Artifact);
    private sealed record EvaluationResponseEnvelope(EvaluationRecord Record);
}
