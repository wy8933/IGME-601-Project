using UnityEngine;

public class EventTester : MonoBehaviour
{
    private EventBinding<TestEventNoArgs> _testEventNoArgsBinding;
    private EventBinding<TestEventWithArgs> _testEventWithArgsBinding;

    private void OnEnable()
    {
        // Register the events
        _testEventNoArgsBinding = new EventBinding<TestEventNoArgs>(HandleTestEventNoArgs);
        EventBus<TestEventNoArgs>.Register(_testEventNoArgsBinding);

        _testEventWithArgsBinding = new EventBinding<TestEventWithArgs>(HandleTestEventWithArgs);
        EventBus<TestEventWithArgs>.Register(_testEventWithArgsBinding);
    }

    private void OnDisable()
    {
        // DeRegister the events
        _testEventNoArgsBinding = new EventBinding<TestEventNoArgs>(HandleTestEventNoArgs);
        EventBus<TestEventNoArgs>.DeRegister(_testEventNoArgsBinding);

        _testEventWithArgsBinding = new EventBinding<TestEventWithArgs>(HandleTestEventWithArgs);
        EventBus<TestEventWithArgs>.DeRegister(_testEventWithArgsBinding);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J)) 
        {
            EventBus<TestEventNoArgs>.Raise(new TestEventNoArgs());
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            EventBus<TestEventWithArgs>.Raise(new TestEventWithArgs { health = 100, stuff = 123f });
        }
    }

    private void HandleTestEventNoArgs() 
    {
        Debug.Log("Test Event received");
    }


    private void HandleTestEventWithArgs(TestEventWithArgs testEventWithArgs)
    {
        Debug.Log($"Test Event received, the variables include Health: {testEventWithArgs.health} and Stuff: {testEventWithArgs.stuff}");
    }
}
