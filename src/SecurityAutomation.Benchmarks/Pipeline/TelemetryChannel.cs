using System.Threading.Channels;
using SecurityAutomation.Core.Events;

namespace SecurityAutomation.Benchmarks.Pipeline;

public sealed class TelemetryChannel
{
    private readonly Channel<SecurityEvent> _channel;

    public TelemetryChannel(int capacity)
    {
        if (capacity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity));
        }

        _channel = Channel.CreateBounded<SecurityEvent>(
            new BoundedChannelOptions(capacity)
            {
                FullMode = BoundedChannelFullMode.Wait,
                SingleWriter = true,
                SingleReader = true
            });
    }

    public ChannelWriter<SecurityEvent> Writer => _channel.Writer;

    public ChannelReader<SecurityEvent> Reader => _channel.Reader;
}
