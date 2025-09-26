using UnityEngine;

namespace RuleViolationSystem
{
    [CreateAssetMenu(fileName = "InteractionForbiddenRecentConditionSO", menuName = "Rules/Conditions/InteractionForbiddenRecentConditionSO")]
    public class InteractionForbiddenRecentConditionSO : RuleConditionSO
    {
        public string interactionId;
        public float lookbackSeconds;

        public override bool IsViolated(IRuleQuery query)
        {
            throw new System.NotImplementedException();
        }
    }
}
