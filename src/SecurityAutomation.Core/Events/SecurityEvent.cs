namespace SecurityAutomation.Core.Events;

public enum SecurityEventType
{
    Unknown,
    ProcessStart,
    ProcessStop,
    File,
    Registry,
    Network,
    Authentication,
    Health
}

public readonly record struct SecurityEvent(
    Guid EventId,
    DateTimeOffset Timestamp,
    string Source,
    SecurityEventType EventType,
    int? ProcessId = null,
    string? ProcessName = null,
    string? User = null,
    IReadOnlyDictionary<string, string>? Metadata = null
);
