using ContentPipeline.Core;

namespace ContentPipeline.Storage;

public sealed class InMemoryWorkflowStateStore : IWorkflowStateStore
{
    private readonly Dictionary<string, WorkflowState> _states = new(StringComparer.Ordinal);

    public Task SaveAsync(WorkflowState state, CancellationToken cancellationToken = default)
    {
        _states[state.RequestId] = state;
        return Task.CompletedTask;
    }

    public Task<WorkflowState?> LoadAsync(string requestId, CancellationToken cancellationToken = default)
    {
        _states.TryGetValue(requestId, out var state);
        return Task.FromResult(state);
    }
}
