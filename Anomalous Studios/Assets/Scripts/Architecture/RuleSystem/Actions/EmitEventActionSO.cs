using UnityEngine;

namespace RuleViolationSystem
{
    [CreateAssetMenu(fileName = "EmitEventActionSO", menuName = "Rules/Actions/EmitEventActionSO")]
    public class EmitEventActionSO : RuleActionSO
    {
        public StringKeyValue keyValuePair;
        public VariableCompareType compareType;
    }
}