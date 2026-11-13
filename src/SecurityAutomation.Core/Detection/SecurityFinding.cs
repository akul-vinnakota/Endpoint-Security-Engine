namespace SecurityAutomation.Core.Detection;

public enum FindingSeverity
{
    Informational,
    Low,
    Medium,
    High,
    Critical
}

public sealed record SecurityFinding(
    Guid FindingId,
    Guid SourceEventId,
    string RuleId,
    string Title,
    FindingSeverity Severity,
    string Description,
    DateTimeOffset Timestamp
);
