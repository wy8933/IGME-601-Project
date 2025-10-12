using UnityEngine;

[CreateAssetMenu(fileName = "MessageCommandEventSO", menuName = "Events/Commands/MessageCommandEventSO")]
public sealed class MessageCommandEventSO : BaseEventSO<CommandRequested>
{
    protected override void OnEvent(CommandRequested e)
    {
        if (e.Args == null || e.Args.Length == 0)
        {
            EventBus<ConsoleLog>.Raise(new ConsoleLog
            {
                Message = "Usage: message <text>",
                Type = LogType.Warning
            });
            return;
        }

        var text = string.Join(" ", e.Args);

        EventBus<ConsoleLog>.Raise(new ConsoleLog { Message = $"[message] {text}", Type = LogType.Log });
    }
}
