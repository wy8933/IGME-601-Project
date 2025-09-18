using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "GenericEventChannelSO", menuName = "Event Channels/GenericEventChannelSO")]
public class GenericEventChannelSO<T> : ScriptableObject
{
    [Tooltip("The action to perform, listeners subscribe to this UnityAction")]
    public UnityAction<T> OnEventRaised;

    public void RaiseEvent(T parameter) 
    {
        OnEventRaised?.Invoke(parameter);
    }
}
