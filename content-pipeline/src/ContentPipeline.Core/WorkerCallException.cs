namespace ContentPipeline.Core;

public sealed class WorkerCallException : Exception
{
    public WorkerCallException(string workerRole, string message, string rawReply, Exception? innerException = null)
        : base(message, innerException)
    {
        WorkerRole = workerRole;
        RawReply = rawReply;
    }

    public string WorkerRole { get; }
    public string RawReply { get; }
}
