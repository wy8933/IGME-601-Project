using Unity.Behavior;
using UnityEngine;

/// <summary>
/// Test class to break the Rulekeeper's rules
/// </summary>
public class RageTotem : MonoBehaviour, IInteractable
{
    [SerializeField] private BehaviorGraphAgent _rulekeeper;
    [SerializeField] private float _holdTime = 0.0f;
    private bool _canInteract = true;

    public float HoldTime { get => _holdTime; }
    public bool CanInteract { get => _canInteract; set => _canInteract = value; }

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
