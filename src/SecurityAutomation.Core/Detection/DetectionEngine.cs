using SecurityAutomation.Core.Events;

namespace SecurityAutomation.Core.Detection;

public sealed class DetectionEngine
{
    private readonly IReadOnlyCollection<IDetectionRule> _rules;

    public DetectionEngine(IEnumerable<IDetectionRule> rules)
    {
        ArgumentNullException.ThrowIfNull(rules);
        _rules = rules.ToArray();
    }

    public IReadOnlyCollection<SecurityFinding> Evaluate(SecurityEvent securityEvent)
    {
        ArgumentNullException.ThrowIfNull(securityEvent);

        return _rules
            .SelectMany(rule => rule.Evaluate(securityEvent))
            .ToArray();
    }
}
