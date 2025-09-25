using UnityEngine;

namespace RuleViolationSystem
{
    [CreateAssetMenu(fileName = "InteractionForbiddenRecentConditionSO", menuName = "Rules/Conditions/InteractionForbiddenRecentConditionSO")]
    public class InteractionForbiddenRecentConditionSO : RuleActionSO
    {
        public string interactionId;
        public float lookbackSeconds;
    }
}
