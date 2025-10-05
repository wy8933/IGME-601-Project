using UnityEngine;

namespace RuleViolationSystem
{
    [CreateAssetMenu(fileName = "EmitRuleBrokenEventActionSO", menuName = "Rules/Actions/EmitRuleBrokenEventActionSO")]
    public class EmitRuleBrokenEventActionSO : RuleActionSO
    {
        [SerializeField]private bool _isBroken;
        public override void Execute(IRuleQuery query, RuleAssetSO rule) 
        {
            EventBus<RuleBroken>.Raise(new RuleBroken { isBroken = _isBroken }) ;
        }
    }
}