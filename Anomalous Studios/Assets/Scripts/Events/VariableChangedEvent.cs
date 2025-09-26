using UnityEngine;

public struct VariableChangedEvent : IEvent
{
   
    public string Key;
    public string RawValue;

    public bool HasInt;
    public int IntValue;

    public bool HasFloat;
    public float FloatValue;

    [Header("Debug")]
    public string PrevRawValue;
    public string Reason;

    public VariableChangedEvent(
        string key, string rawValue,
        bool hasInt, int intValue,
        bool hasFloat, float floatValue,
        string prevRawValue = null,
        string reason = null)
    {
        Key = key;
        RawValue = rawValue;
        HasInt = hasInt;
        IntValue = intValue;
        HasFloat = hasFloat;
        FloatValue = floatValue;
        PrevRawValue = prevRawValue;
        Reason = reason;
    }
    
}
