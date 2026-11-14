using SecurityAutomation.Core.Events;

namespace SecurityAutomation.Core.Detection;

public sealed class DetectionEngine
{
    private readonly IDetectionRule[] _rules;

    public DetectionEngine(IEnumerable<IDetectionRule> rules)
    {
        ArgumentNullException.ThrowIfNull(rules);
        _rules = rules.ToArray();
    }

    public IReadOnlyCollection<SecurityFinding> Evaluate(SecurityEvent securityEvent)
    {
        ArgumentNullException.ThrowIfNull(securityEvent);

        List<SecurityFinding>? findings = null;

        foreach (var rule in _rules)
        {
            foreach (var finding in rule.Evaluate(securityEvent))
            {
                findings ??= new List<SecurityFinding>();
                findings.Add(finding);
            }
        }

        return findings is null ? Array.Empty<SecurityFinding>() : findings;
    }
}
