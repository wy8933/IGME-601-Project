using System.Collections.Generic;
using System;
using UnityEngine;

namespace RuleViolationSystem {
    public enum ConditionLogic 
    {
        All,
        Any
    }

    [CreateAssetMenu(fileName = "RuleSO", menuName = "Scriptable Objects/RuleSO")]
    public abstract class RuleConditionSO : ScriptableObject
    {
        public abstract bool IsViolated(IRuleQuery query);

        public virtual IEnumerable<string> ReferencedVariableKeys() => Array.Empty<string>();

        public virtual string DebugSummary(IRuleQuery query) => name;
    }
}