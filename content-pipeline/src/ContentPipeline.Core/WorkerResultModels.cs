namespace ContentPipeline.Core;

public sealed record WorkerExecutionEnvelope
{
    public string? RunId { get; init; }
    public string? Status { get; init; }
    public string? Reply { get; init; }
    public string? Error { get; init; }
    public string? SessionKey { get; init; }
}
