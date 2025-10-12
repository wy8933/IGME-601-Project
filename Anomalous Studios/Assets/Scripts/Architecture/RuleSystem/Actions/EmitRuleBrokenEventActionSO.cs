using UnityEngine;
using static PopupEvent;

namespace RuleViolationSystem
{
    [CreateAssetMenu(fileName = "EmitRuleBrokenEventActionSO", menuName = "Rules/Actions/EmitRuleBrokenEventActionSO")]
    public class EmitRuleBrokenEventActionSO : RuleActionSO
    {
        [SerializeField]private bool _isBroken;
        public override void Execute(IRuleQuery query, RuleAssetSO rule) 
        {
            Debug.Log($"Rule broken is set to {_isBroken}");
            EventBus<RuleBroken>.Raise(new RuleBroken { isBroken = _isBroken }) ;
            EventBus<OpenPopup>.Raise(new OpenPopup { RuleName = rule.name });
        }
    }
}