using UnityEngine;

namespace RuleViolationSystem
{
    [CreateAssetMenu(fileName = "VariableThresholdConditionSO", menuName = "Rules/Conditions/VariableThresholdConditionSO")]
    public class VariableThresholdConditionSO : RuleConditionSO
    {
        public StringKeyValue keyValuePair;
        public VariableCompareType compareType;

        public override bool IsViolated(IRuleQuery query)
        {
            return query.Check(new VariableCondition(keyValuePair.key,compareType,keyValuePair.value));
        }
    }
}