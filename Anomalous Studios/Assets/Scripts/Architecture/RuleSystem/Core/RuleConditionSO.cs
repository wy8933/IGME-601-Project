using UnityEngine;

namespace RuleViolationSystem {
    public enum ConditionLogic 
    {
        All,
        Any
    }

    [CreateAssetMenu(fileName = "RuleSO", menuName = "Scriptable Objects/RuleSO")]
    public class RuleConditionSO : ScriptableObject
    {
        public bool IsViolated(IRuleQuery query) 
        {
            return false;
        }

        public string GetShortDebug(IRuleQuery query) 
        {
            return "";
        }
    }
}