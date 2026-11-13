using SecurityAutomation.Core.Detection;
using SecurityAutomation.Core.Events;

namespace SecurityAutomation.Core.Tests;

public sealed class DetectionEngineTests
{
    [Fact]
    public void Evaluate_ReturnsFinding_WhenRuleMatches()
    {
        var engine = new DetectionEngine(
            new IDetectionRule[]
            {
                new TestProcessStartRule()
            });

        var securityEvent = new SecurityEvent(
            Guid.NewGuid(),
            DateTimeOffset.UtcNow,
            "synthetic-test",
            SecurityEventType.ProcessStart,
            ProcessId: 4242,
            ProcessName: "test.exe",
            User: "test-user");

        var findings = engine.Evaluate(securityEvent);

        var finding = Assert.Single(findings);

        Assert.Equal("TEST-001", finding.RuleId);
        Assert.Equal(FindingSeverity.Medium, finding.Severity);
        Assert.Equal(securityEvent.EventId, finding.SourceEventId);
    }

    private sealed class TestProcessStartRule : IDetectionRule
    {
        public string RuleId => "TEST-001";

        public string Name => "Synthetic Process Start Detection";

        public IEnumerable<SecurityFinding> Evaluate(SecurityEvent securityEvent)
        {
            if (securityEvent.EventType != SecurityEventType.ProcessStart)
            {
                yield break;
            }

            yield return new SecurityFinding(
                Guid.NewGuid(),
                securityEvent.EventId,
                RuleId,
                Name,
                FindingSeverity.Medium,
                "Synthetic process-start event matched the test rule.",
                DateTimeOffset.UtcNow);
        }
    }
}
