using UnityEngine;

public class TestCommandSubscriber : MonoBehaviour
{
    private EventBinding<CommandRequested> _binding;

    void OnEnable()
    {
        _binding = new EventBinding<CommandRequested>(_ => {
            EventBus<ConsoleLog>.Raise(new ConsoleLog { Message = "test command works", Type = LogType.Log });
            Debug.Log("test command works");
        }, pr: 100);

        EventBus<CommandRequested>.Register(_binding, scope: CommandScope.Key("test_command"));
    }

    void OnDisable()
    {
        EventBus<CommandRequested>.DeRegister(_binding, scope: CommandScope.Key("test_command"));
    }
}
