using ContentPipeline.Core;

namespace ContentPipeline.Orchestration.Bridge;

public sealed class WorkflowRunner(ContentWorkflowOrchestrator orchestrator)
{
    public Task<WorkflowState> RunAsync(WorkflowRequest request, CancellationToken cancellationToken = default)
        => orchestrator.RunAsync(request, cancellationToken);
}
