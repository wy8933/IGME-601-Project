using System;
using System.Collections.Generic;

namespace RuleViolationSystem { 
    public interface IRuleQuery
    {
        bool TryGet(string key, out string raw);
        bool TryGetInt(string key, out int value);
        bool TryGetFloat(string key, out float value);

        bool Check(VariableCondition cond);
        bool CheckAll(IEnumerable<VariableCondition> conds);

        bool WasInteractionSeen(string interactionId, float withinSeconds);

        public string FloorId { get; }
        public DateTime UtcNow { get; }

        void setFloorID(string id);
    }
}