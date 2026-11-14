using SecurityAutomation.Benchmarks.Generation;
using SecurityAutomation.Benchmarks.Pipeline;
using SecurityAutomation.Core.Detection;

const long warmupEvents = 1_000_000;
const long measuredEvents = 10_000_000;
const int channelCapacity = 10_000;

static BenchmarkResult RunGeneration(long eventCount)
{
    var generator = new SyntheticEventGenerator(seed: 42);
    var runner = new GenerationBenchmarkRunner(generator);

    return runner.Run(eventCount);
}

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

Console.WriteLine("Security Automation Allocation Attribution Benchmark");
Console.WriteLine($"Warmup events:    {warmupEvents:N0}");
Console.WriteLine($"Measured events:  {measuredEvents:N0}");
Console.WriteLine($"Channel capacity: {channelCapacity:N0}");
Console.WriteLine();

Console.WriteLine("Warming generation...");
RunGeneration(warmupEvents);

Console.WriteLine("Warming transport...");
await RunTransportAsync(warmupEvents);

Console.WriteLine("Warming detection...");
await RunDetectionAsync(warmupEvents);

Console.WriteLine();
Console.WriteLine("Running generation-only...");
var generation = RunGeneration(measuredEvents);

Console.WriteLine("Running transport...");
var transport = await RunTransportAsync(measuredEvents);

Console.WriteLine("Running detection...");
var detection = await RunDetectionAsync(measuredEvents);

Console.WriteLine();
Console.WriteLine("GENERATION ONLY");
Console.WriteLine($"  Throughput:      {generation.EventsPerSecond:N0} events/sec");
Console.WriteLine($"  Allocated/event: {generation.AllocatedBytesPerEvent:F2} bytes");

Console.WriteLine();
Console.WriteLine("GENERATION + TRANSPORT");
Console.WriteLine($"  Throughput:      {transport.EventsPerSecond:N0} events/sec");
Console.WriteLine($"  Allocated/event: {transport.AllocatedBytesPerEvent:F2} bytes");

Console.WriteLine();
Console.WriteLine("GENERATION + TRANSPORT + DETECTION");
Console.WriteLine($"  Throughput:      {detection.EventsPerSecond:N0} events/sec");
Console.WriteLine($"  Allocated/event: {detection.AllocatedBytesPerEvent:F2} bytes");

Console.WriteLine();
Console.WriteLine("ALLOCATION COST BY STAGE");
Console.WriteLine($"  Generation:       {generation.AllocatedBytesPerEvent:F2} bytes/event");
Console.WriteLine($"  Transport added:  {transport.AllocatedBytesPerEvent - generation.AllocatedBytesPerEvent:F2} bytes/event");
Console.WriteLine($"  Detection added:  {detection.AllocatedBytesPerEvent - transport.AllocatedBytesPerEvent:F2} bytes/event");
