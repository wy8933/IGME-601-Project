using UnityEngine;

namespace RuleViolationSystem
{
    [CreateAssetMenu(fileName = "VariableThresholdConditionSO", menuName = "Rules/Conditions/VariableThresholdConditionSO")]
    public class VariableThresholdConditionSO : RuleActionSO
    {
        public StringKeyValue keyValuePair;
        public VariableCompareType compareType;
    }
}