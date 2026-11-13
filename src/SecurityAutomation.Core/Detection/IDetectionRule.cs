using SecurityAutomation.Core.Events;

namespace SecurityAutomation.Core.Detection;

public interface IDetectionRule
{
    string RuleId { get; }
    string Name { get; }

    IEnumerable<SecurityFinding> Evaluate(SecurityEvent securityEvent);
}
