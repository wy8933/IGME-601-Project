using System;

namespace RuleViolationSystem { 
    public interface IRuleQuery
    {
        bool TryGet(string key, out string rawValue);

        bool WasInteractionSeen(string interactionID, float withinSeconds);

        int CurrentFloorIndex { get; }
        string CurrentLoopId { get; }

        DateTime UtcNow { get; }
    }
}