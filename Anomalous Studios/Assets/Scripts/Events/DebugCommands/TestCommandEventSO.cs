using UnityEngine;


[CreateAssetMenu(fileName = "TestCommandEventSO", menuName = "Events/Commands/TestCommandEventSO")]
public sealed class TestCommandEventSO : BaseEventSO<CommandRequested>
{
    protected override void OnEvent(CommandRequested e)
    {
        EventBus<ConsoleLog>.Raise(new ConsoleLog { Message = "test command works", Type = LogType.Log });
        Debug.Log("test command works");
    }
}