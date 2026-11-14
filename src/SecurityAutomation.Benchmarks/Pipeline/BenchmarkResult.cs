namespace SecurityAutomation.Benchmarks.Pipeline;

public sealed record BenchmarkResult(
    long EventsProcessed,
    long FindingsGenerated,
    TimeSpan Elapsed,
    double EventsPerSecond,
    long AllocatedBytes,
    double AllocatedBytesPerEvent,
    long WorkingSetBytes,
    int Gen0Collections,
    int Gen1Collections,
    int Gen2Collections
);
