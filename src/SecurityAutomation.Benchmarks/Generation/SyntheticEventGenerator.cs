using SecurityAutomation.Core.Events;

namespace SecurityAutomation.Benchmarks.Generation;

public sealed class SyntheticEventGenerator
{
    private readonly Random _random;
    private long _sequence;

    private static readonly DateTimeOffset BaseTimestamp =
        new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

    private static readonly string[] ProcessNames =
    [
        "explorer.exe",
        "powershell.exe",
        "cmd.exe",
        "chrome.exe",
        "svchost.exe",
        "dotnet.exe"
    ];

    private static readonly string[] Users =
    [
        "user-a",
        "user-b",
        "service-account",
        "administrator"
    ];

    public SyntheticEventGenerator(int seed = 42)
    {
        _random = new Random(seed);
    }

    public SecurityEvent Next()
    {
        var sequence = Interlocked.Increment(ref _sequence);

        var eventType = (SecurityEventType)_random.Next(
            (int)SecurityEventType.ProcessStart,
            (int)SecurityEventType.Health + 1);

        return new SecurityEvent(
            CreateDeterministicGuid(sequence),
            BaseTimestamp.AddTicks(sequence),
            "synthetic-benchmark",
            eventType,
            ProcessId: _random.Next(100, 50000),
            ProcessName: ProcessNames[_random.Next(ProcessNames.Length)],
            User: Users[_random.Next(Users.Length)]);
    }

    private static Guid CreateDeterministicGuid(long sequence)
    {
        Span<byte> bytes = stackalloc byte[16];
        BitConverter.TryWriteBytes(bytes, sequence);
        return new Guid(bytes);
    }
}
