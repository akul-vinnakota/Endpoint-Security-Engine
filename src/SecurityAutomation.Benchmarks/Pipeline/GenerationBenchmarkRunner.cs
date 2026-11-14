using System.Diagnostics;
using SecurityAutomation.Benchmarks.Generation;

namespace SecurityAutomation.Benchmarks.Pipeline;

public sealed class GenerationBenchmarkRunner
{
    private readonly SyntheticEventGenerator _generator;

    public GenerationBenchmarkRunner(SyntheticEventGenerator generator)
    {
        _generator = generator;
    }

    public BenchmarkResult Run(long eventCount)
    {
        long processed = 0;

        var allocatedBefore = GC.GetTotalAllocatedBytes(true);
        var gen0Before = GC.CollectionCount(0);
        var gen1Before = GC.CollectionCount(1);
        var gen2Before = GC.CollectionCount(2);

        var stopwatch = Stopwatch.StartNew();

        for (long i = 0; i < eventCount; i++)
        {
            _ = _generator.Next();
            processed++;
        }

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
