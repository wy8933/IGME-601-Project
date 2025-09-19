using UnityEngine;

public struct TestEventNoArgs : IEvent
{

}

public struct TestEventWithArgs : IEvent 
{
    public int health;
    public float stuff;
}
