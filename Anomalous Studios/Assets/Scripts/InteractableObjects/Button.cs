using UnityEngine;

/// <summary>
/// A test-case for the interactable interface
/// </summary>
public class Button : MonoBehaviour, IInteractable
{
    [SerializeField] private Animator _elevator;

    private bool _canInteract = true;

    // TODO: implement a press and hold variable, if == 0 then it is instant, otherwise takes time
    // The press and hold variables should be changeable in the inspector

    // TODO: check file against style guide
    
    public bool CanInteract
    {
        // TODO: determine how to implement CanInteract, prob. just as public variable
        get { return _canInteract; }
        set { _canInteract = value; }
    }

    public void Highlight()
    {
        // TODO: Replace with shader to highlight the item, or UI element to indicate it is interactable
        Debug.Log("Click me!!!!");
    }
    public void Interact()
    {
        Debug.Log("Interaction: Button Pressed");
        // TODO: temporary interact value. Replace with a numpad to call the transition between levels
        if (_elevator != null)
        {
            _elevator.SetBool("is_open", !_elevator.GetBool("is_open"));
        }
    }
}
