using UnityEngine;


[CreateAssetMenu(fileName = "SetTimeCommandEventSO", menuName = "Events/Commands/SetTimeCommandEventSO")]
public sealed class SetTimeCommandEventSO : BaseEventSO<CommandRequested>
{
    protected override void OnEvent(CommandRequested e)
    {
        EventBus<ConsoleLog>.Raise(new ConsoleLog { Message = "Set time not implemented", Type = LogType.Log });
    }
}