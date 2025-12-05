using UnityEngine;
using static PopupEvent;

namespace RuleViolationSystem
{
    [CreateAssetMenu(fileName = "EmitRuleBrokenEventActionSO", menuName = "Rules/Actions/EmitRuleBrokenEventActionSO")]
    public class EmitRuleBrokenEventActionSO : RuleActionSO
    {
        [SerializeField] private bool _isBroken;

        public override void Execute(IRuleQuery query, RuleAssetSO rule) 
        {
            EventBus<RuleBroken>.Raise(new RuleBroken { isBroken = _isBroken, 
                target = ElevatorController.Player.transform.position });

            if (_isBroken)
            {
                //Debug.Log((int.Parse(VariableConditionManager.Instance.Get("rule_broken_count:int")) + 1).ToString()); // keep this comment, it fix the problem somehow
                VariableConditionManager.Instance.Set("rule_broken_count:int", (int.Parse(VariableConditionManager.Instance.Get("rule_broken_count:int")) + 1).ToString());
                SpeakerManager.Instance.StartStatic();
                SpeakerManager.Instance.StopMusic();
            }
            else
            {
                SpeakerManager.Instance.StopStatic();
                SpeakerManager.Instance.StartMusic();
            }
        }
    }
}