using System.Diagnostics;
using SecurityAutomation.Benchmarks.Generation;

namespace SecurityAutomation.Benchmarks.Pipeline;

public sealed class TransportBenchmarkRunner
{
    private readonly SyntheticEventGenerator _generator;
    private readonly TelemetryChannel _channel;

    public TransportBenchmarkRunner(
        SyntheticEventGenerator generator,
        TelemetryChannel channel)
    {
        _generator = generator;
        _channel = channel;
    }

    public async Task<BenchmarkResult> RunAsync(
        long eventCount,
        CancellationToken cancellationToken = default)
    {
        long processed = 0;

        var allocatedBefore = GC.GetTotalAllocatedBytes(true);
        var gen0Before = GC.CollectionCount(0);
        var gen1Before = GC.CollectionCount(1);
        var gen2Before = GC.CollectionCount(2);

        var stopwatch = Stopwatch.StartNew();

        var consumer = Task.Run(async () =>
        {
            await foreach (var _ in
                _channel.Reader.ReadAllAsync(cancellationToken))
            {
                processed++;
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

        var process = Process.GetCurrentProcess();

        return new BenchmarkResult(
            processed,
            0,
            stopwatch.Elapsed,
            processed / stopwatch.Elapsed.TotalSeconds,
            allocatedBytes,
            (double)allocatedBytes / processed,
            process.WorkingSet64,
            GC.CollectionCount(0) - gen0Before,
            GC.CollectionCount(1) - gen1Before,
            GC.CollectionCount(2) - gen2Before);
    }
}
