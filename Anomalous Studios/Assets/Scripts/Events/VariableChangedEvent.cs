using UnityEngine;

public class VariableChangedEvent : IEvent
{
    public string key;
    public string rawValue;

    [Header("Debuging")]
    public string prevRawValue;
}
