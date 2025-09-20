using UnityEngine;

/// <summary>
/// A test-case for the interactable interface
/// </summary>
public class Button : Interaction
{
    [SerializeField] private string _name;

    [SerializeField] private Animator _elevator;

    // TODO: check file against style guide
    
    public override void Highlight()
    {
        // TODO: Replace with shader to highlight the item, or UI element to indicate it is interactable
        Debug.Log(_name + " Button");
    }

    public override void Interact()
    {
        Debug.Log("Interaction: " + _name + " Button Pressed");

        // TODO: temporary interact value. A different numpad object should call the transition between levels
        if (_elevator != null)
        {
            _elevator.SetBool("is_open", !_elevator.GetBool("is_open"));
        }
    }
}
