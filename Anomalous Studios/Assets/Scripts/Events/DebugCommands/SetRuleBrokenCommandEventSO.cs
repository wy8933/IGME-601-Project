using UnityEngine;


[CreateAssetMenu(fileName = "SetRuleBrokenCommandEventSO", menuName = "Events/Commands/SetRuleBrokenCommandEventSO")]
public sealed class SetRuleBrokenCommandEventSO : BaseEventSO<CommandRequested>
{
    protected override void OnEvent(CommandRequested e)
    {
        EventBus<RuleBroken>.Raise(new RuleBroken { isBroken = bool.Parse(e.Args[0])});
    }
}