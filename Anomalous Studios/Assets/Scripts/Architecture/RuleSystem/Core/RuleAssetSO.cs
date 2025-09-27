using UnityEngine;

namespace RuleViolationSystem { 
    [CreateAssetMenu(fileName = "RuleAsset", menuName = "Scriptable Objects/RuleAsset")]
    public class RuleAssetSO : ScriptableObject
    {
        public string ruleId;
        public ConditionLogic conditionLogic;
        public RuleConditionSO[] ruleConditions;
        public RuleActionSO[] violationActions;
        public RuleActionSO[] resolveActions;

        public bool fireOnceUntilResolved;

        [Min(0.5f)]
        public float cooldownSeconds;
    }
}