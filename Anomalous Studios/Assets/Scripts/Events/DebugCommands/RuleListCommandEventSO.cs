using RuleViolationSystem;
using UnityEngine;


[CreateAssetMenu(fileName = "ListRuleCommandEventSO", menuName = "Events/Commands/ListRuleCommandEventSO")]
public sealed class ListRuleCommandEventSO : BaseEventSO<CommandRequested>
{
    protected override void OnEvent(CommandRequested e)
    {
        foreach (RuleAssetSO rule in RuleManager.Instance.ruleSet.rules) 
        {
            EventBus<ConsoleLog>.Raise(new ConsoleLog {Message = rule.name, Type = LogType.Log});
        }
    }
}