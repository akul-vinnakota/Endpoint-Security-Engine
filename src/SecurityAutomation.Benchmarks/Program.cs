using SecurityAutomation.Benchmarks.Generation;
using SecurityAutomation.Benchmarks.Pipeline;
using SecurityAutomation.Core.Detection;

const long warmupEvents = 1_000_000;
const long measuredEvents = 10_000_000;
const int channelCapacity = 10_000;

static async Task<BenchmarkResult> RunTransportAsync(long eventCount)
{
    var generator = new SyntheticEventGenerator(seed: 42);
    var channel = new TelemetryChannel(channelCapacity);

    var runner = new TransportBenchmarkRunner(
        generator,
        channel);

    return await runner.RunAsync(eventCount);
}

static async Task<BenchmarkResult> RunDetectionAsync(long eventCount)
{
    var generator = new SyntheticEventGenerator(seed: 42);
    var channel = new TelemetryChannel(channelCapacity);
    var engine = new DetectionEngine(Array.Empty<IDetectionRule>());

    var runner = new BenchmarkRunner(
        generator,
        channel,
        engine);

    return await runner.RunAsync(eventCount);
}

Console.WriteLine("Security Automation Pipeline Cost Benchmark");
Console.WriteLine($"Warmup events:    {warmupEvents:N0}");
Console.WriteLine($"Measured events:  {measuredEvents:N0}");
Console.WriteLine($"Channel capacity: {channelCapacity:N0}");
Console.WriteLine();

Console.WriteLine("Warming transport path...");
await RunTransportAsync(warmupEvents);

Console.WriteLine("Warming detection path...");
await RunDetectionAsync(warmupEvents);

Console.WriteLine();
Console.WriteLine("Running transport-only benchmark...");
var transport = await RunTransportAsync(measuredEvents);

Console.WriteLine("Running detection-engine benchmark...");
var detection = await RunDetectionAsync(measuredEvents);

Console.WriteLine();
Console.WriteLine("TRANSPORT ONLY");
Console.WriteLine($"  Throughput:      {transport.EventsPerSecond:N0} events/sec");
Console.WriteLine($"  Allocated/event: {transport.AllocatedBytesPerEvent:F2} bytes");
Console.WriteLine($"  Working set:     {transport.WorkingSetBytes / 1024.0 / 1024.0:F2} MB");

Console.WriteLine();
Console.WriteLine("WITH DETECTION ENGINE");
Console.WriteLine($"  Throughput:      {detection.EventsPerSecond:N0} events/sec");
Console.WriteLine($"  Allocated/event: {detection.AllocatedBytesPerEvent:F2} bytes");
Console.WriteLine($"  Working set:     {detection.WorkingSetBytes / 1024.0 / 1024.0:F2} MB");

Console.WriteLine();
Console.WriteLine("DETECTION ENGINE COST");
Console.WriteLine($"  Throughput delta: {detection.EventsPerSecond - transport.EventsPerSecond:N0} events/sec");
Console.WriteLine($"  Allocation delta: {detection.AllocatedBytesPerEvent - transport.AllocatedBytesPerEvent:F2} bytes/event");
