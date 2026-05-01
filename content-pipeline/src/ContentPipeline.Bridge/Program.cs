using ContentPipeline.Adapters.Artifacts;
using ContentPipeline.Adapters.GoogleDrive;
using ContentPipeline.Adapters.OpenClaw;
using ContentPipeline.Adapters.Telegram;
using ContentPipeline.Core;
using ContentPipeline.Orchestration;
using ContentPipeline.Orchestration.Bridge;
using ContentPipeline.Storage;

var mode = args.FirstOrDefault() ?? "stub";

if (string.Equals(mode, "probe", StringComparison.OrdinalIgnoreCase))
{
    await RunProbeAsync(args.Skip(1).ToArray());
    return;
}

if (string.Equals(mode, "live-flow", StringComparison.OrdinalIgnoreCase))
{
    await RunLiveFlowProbeAsync();
    return;
}

await RunWorkflowAsync(mode);

static async Task RunWorkflowAsync(string mode)
{
    var stateStore = new InMemoryWorkflowStateStore();
    var artifactStore = new ArtifactStoreStub();
    var driveSync = new GoogleDriveSyncStub();
    var approval = new ImmediateApprovalCoordinator(approved: true);
    var publisher = new TelegramPublisherStub();

    IOpenClawTransport transport = string.Equals(mode, "live", StringComparison.OrdinalIgnoreCase)
        ? CreateLiveTransportFromEnvironment()
        : new OpenClawSessionsTransportStub();

    var openClawClient = new OpenClawAgentClient(
        transport,
        new OpenClawAgentRoutingOptions(
            ManagerSessionKey: GetEnv("OPENCLAW_MANAGER_SESSION", "manager-session"),
            CreatorSessionKey: GetEnv("OPENCLAW_CREATOR_SESSION", "creator-session"),
            EvaluatorSessionKey: GetEnv("OPENCLAW_EVALUATOR_SESSION", "evaluator-session")));

    var orchestrator = new ContentWorkflowOrchestrator(
        stateStore,
        artifactStore,
        driveSync,
        openClawClient,
        publisher,
        approval);

    var request = BridgeRequestFactory.CreateMinimal(
        requestId: $"bridge-{DateTimeOffset.UtcNow:yyyyMMddHHmmss}",
        chatId: "telegram:1185522850",
        userId: "1185522850",
        text: "Create and publish one philosophy image post",
        destination: "@channel",
        requiresApproval: false);

    var runner = new WorkflowRunner(orchestrator);
    var result = await runner.RunAsync(request);

    Console.WriteLine($"Phase: {result.Phase}");
    Console.WriteLine($"Status: {result.Status}");
    Console.WriteLine($"FailureReason: {result.FailureReason ?? "<none>"}");
    Console.WriteLine($"PublishMessageId: {result.PublishReceipt?.MessageId ?? "<none>"}");
}

static async Task RunProbeAsync(string[] args)
{
    var sessionKey = args.Length > 0 ? args[0] : GetEnv("OPENCLAW_CREATOR_SESSION", "agent:creator:telegram:direct:1185522850");
    var prompt = args.Length > 1 ? args[1] : "Return exactly this JSON and nothing else: {\"ok\":true}";

    var transport = CreateLiveTransportFromEnvironment();
    var response = await transport.SendToSessionAsync(new OpenClawSessionSendRequest(sessionKey, prompt, 60));

    Console.WriteLine("SessionKey: " + response.SessionKey);
    Console.WriteLine("ReplyText:");
    Console.WriteLine(response.ReplyText);
}

static async Task RunLiveFlowProbeAsync()
{
    var stateStore = new InMemoryWorkflowStateStore();
    var artifactStore = new LocalArtifactStore("/home/sergiy_shyshko/.openclaw-content/src/data/artifacts");
    var driveSync = new GoogleDriveSyncStub();
    var approval = new ImmediateApprovalCoordinator(approved: true);
    var publisher = new TelegramPublisherStub();

    var transport = CreateLiveTransportFromEnvironment();
    var openClawClient = new OpenClawAgentClient(
        transport,
        new OpenClawAgentRoutingOptions(
            ManagerSessionKey: GetEnv("OPENCLAW_MANAGER_SESSION", "agent:manager:telegram:direct:1185522850"),
            CreatorSessionKey: GetEnv("OPENCLAW_CREATOR_SESSION", "agent:creator:telegram:direct:1185522850"),
            EvaluatorSessionKey: GetEnv("OPENCLAW_EVALUATOR_SESSION", "agent:creator:telegram:direct:1185522850")));

    var orchestrator = new ContentWorkflowOrchestrator(
        stateStore,
        artifactStore,
        driveSync,
        openClawClient,
        publisher,
        approval);

    var request = BridgeRequestFactory.CreateMinimal(
        requestId: $"liveflow-{DateTimeOffset.UtcNow:yyyyMMddHHmmss}",
        chatId: "telegram:1185522850",
        userId: "1185522850",
        text: "Create and validate one philosophy image post",
        destination: "@channel",
        requiresApproval: false);

    try
    {
        var result = await new WorkflowRunner(orchestrator).RunAsync(request);
        Console.WriteLine($"Phase: {result.Phase}");
        Console.WriteLine($"Status: {result.Status}");
        Console.WriteLine($"FailureReason: {result.FailureReason ?? "<none>"}");
        Console.WriteLine($"Drafts: {result.Drafts.Count}");
        Console.WriteLine($"Evaluations: {result.Evaluations.Count}");
    }
    catch (WorkerCallException ex)
    {
        Console.WriteLine("WorkerCallException: " + ex.WorkerRole);
        Console.WriteLine(ex.Message);
        Console.WriteLine("RawReply:");
        Console.WriteLine(ex.RawReply);
    }
    catch (Exception ex)
    {
        Console.WriteLine("UnhandledException: " + ex.GetType().Name);
        Console.WriteLine(ex.Message);
        if (ex.InnerException is not null)
        {
            Console.WriteLine("InnerException: " + ex.InnerException.GetType().Name);
            Console.WriteLine(ex.InnerException.Message);
        }
    }
}

static OpenClawHttpTransport CreateLiveTransportFromEnvironment()
{
    var httpClient = new HttpClient();
    var options = new OpenClawAdapterOptions(
        BaseUrl: GetEnv("OPENCLAW_BASE_URL", "http://127.0.0.1:18889"),
        GatewayToken: GetEnv("OPENCLAW_GATEWAY_TOKEN"),
        ManagerSessionKey: GetEnv("OPENCLAW_MANAGER_SESSION", "manager-session"),
        CreatorSessionKey: GetEnv("OPENCLAW_CREATOR_SESSION", "creator-session"),
        EvaluatorSessionKey: GetEnv("OPENCLAW_EVALUATOR_SESSION", "evaluator-session"));

    return new OpenClawHttpTransport(httpClient, options);
}

static string GetEnv(string name, string? fallback = null)
{
    var value = Environment.GetEnvironmentVariable(name);
    if (!string.IsNullOrWhiteSpace(value))
    {
        return value;
    }

    if (fallback is not null)
    {
        return fallback;
    }

    throw new InvalidOperationException($"Missing required environment variable: {name}");
}
