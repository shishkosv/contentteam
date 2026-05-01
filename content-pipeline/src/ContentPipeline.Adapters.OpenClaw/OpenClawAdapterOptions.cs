namespace ContentPipeline.Adapters.OpenClaw;

public sealed record OpenClawAdapterOptions(
    string BaseUrl,
    string GatewayToken,
    string ManagerSessionKey,
    string CreatorSessionKey,
    string EvaluatorSessionKey);
