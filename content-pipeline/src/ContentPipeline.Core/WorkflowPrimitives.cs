namespace ContentPipeline.Core;

public enum WorkflowPhase
{
    Intake,
    Brief,
    Create,
    Evaluate,
    Approve,
    Publish,
    Done,
    Failed,
    Cancelled,
}

public enum WorkflowStatus
{
    Running,
    Waiting,
    Passed,
    Failed,
    Cancelled,
}

public enum PublishMode
{
    ApprovalRequired,
    AutoPublish,
}

public enum EvaluationDecision
{
    Pass,
    Fail,
    Blocked,
}

public enum ArtifactStatus
{
    Draft,
    Trashed,
    Ready,
    Published,
}
