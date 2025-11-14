
using UnityEngine;


[CreateAssetMenu(fileName = "SetRuleBrokenCommandEventSO", menuName = "Events/Commands/SetRuleBrokenCommandEventSO")]
public sealed class SetRuleBrokenCommandEventSO : BaseEventSO<CommandRequested>
{
    protected override void OnEvent(CommandRequested e)
    {
        if (bool.Parse(e.Args[0]))
        {
            EventBus<RuleBroken>.Raise(new RuleBroken { isBroken = true });
            EventBus<OpenPopup>.Raise(new OpenPopup { RuleName = "Rule Borken Command" });
            GameVariables.Set("rule_broken_count:int", (int.Parse(VariableConditionManager.Instance.Get("rule_broken_count:int")) + 1).ToString());
        }
        else
        {
            EventBus<RuleBroken>.Raise(new RuleBroken { isBroken = false });
        }
    }
}