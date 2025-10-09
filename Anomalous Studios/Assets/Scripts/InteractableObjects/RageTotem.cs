using Unity.Behavior;
using UnityEngine;

/// <summary>
/// Test class to break the Rulekeeper's rules
/// </summary>
public class RageTotem : MonoBehaviour, IInteractable
{
    [SerializeField] private BehaviorGraphAgent _rulekeeper;

    public bool CanInteract { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public void Highlight()
    {
        // Highlight!
    }

    public void Interact()
    {
        _rulekeeper.SetVariableValue("ruleBroken", true);
    }

    public void RemoveHighlight()
    {
        throw new System.NotImplementedException();
    }
}
