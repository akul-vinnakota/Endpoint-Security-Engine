using System.Diagnostics;
using SecurityAutomation.Benchmarks.Generation;
using SecurityAutomation.Core.Detection;

namespace SecurityAutomation.Benchmarks.Pipeline;

public sealed class BenchmarkRunner
{
    private readonly SyntheticEventGenerator _generator;
    private readonly TelemetryChannel _channel;
    private readonly DetectionEngine _engine;

    public BenchmarkRunner(
        SyntheticEventGenerator generator,
        TelemetryChannel channel,
        DetectionEngine engine)
    {
        _generator = generator;
        _channel = channel;
        _engine = engine;
    }

    public async Task<BenchmarkResult> RunAsync(
        long eventCount,
        CancellationToken cancellationToken = default)
    {
        if (eventCount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(eventCount));
        }

        long processed = 0;
        long findings = 0;

        var allocatedBefore = GC.GetTotalAllocatedBytes(true);
        var gen0Before = GC.CollectionCount(0);
        var gen1Before = GC.CollectionCount(1);
        var gen2Before = GC.CollectionCount(2);

        var stopwatch = Stopwatch.StartNew();

        var consumer = Task.Run(async () =>
        {
            await foreach (var securityEvent in
                _channel.Reader.ReadAllAsync(cancellationToken))
            {
                var results = _engine.Evaluate(securityEvent);

                processed++;
                findings += results.Count;
            }
        }, cancellationToken);

        for (long i = 0; i < eventCount; i++)
        {
            await _channel.Writer.WriteAsync(
                _generator.Next(),
                cancellationToken);
        }

        _channel.Writer.Complete();
        await consumer;

        stopwatch.Stop();

        var allocatedBytes =
            GC.GetTotalAllocatedBytes(true) - allocatedBefore;

        var eventsPerSecond =
            processed / stopwatch.Elapsed.TotalSeconds;

        var process = Process.GetCurrentProcess();

        return new BenchmarkResult(
            processed,
            findings,
            stopwatch.Elapsed,
            eventsPerSecond,
            allocatedBytes,
            (double)allocatedBytes / processed,
            process.WorkingSet64,
            GC.CollectionCount(0) - gen0Before,
            GC.CollectionCount(1) - gen1Before,
            GC.CollectionCount(2) - gen2Before);
    }
}
