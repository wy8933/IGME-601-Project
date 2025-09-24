using UnityEngine;

/// <summary>
/// A test-case for the interactable interface
/// </summary>
public class Button : Interaction
{
    [SerializeField] private string _name;

    [SerializeField] private Animator _elevator;

    [SerializeField] private Level _level;

    public override void Update()
    {
        base.Update();
    }

    public override void Highlight()
    {
        // TODO: Replace with shader to highlight the item, or UI element to indicate it is interactable
    }

    protected override void Interact()
    {
        // TODO: temporary interact value. A different numpad object should call the transition between levels

        // If the player is in the elevator
        if (VariableConditionManager.Instance.Get("InElevator").Equals("true") &&
            VariableConditionManager.Instance.Get("TaskComplete").Equals("false"))
        {
            _elevator.SetBool("is_open", false);
            
            EventBus<LevelLoading>.Raise(new LevelLoading { name = _level });

            _elevator.SetBool("is_open", !_elevator.GetBool("is_open"));
        }

    }
}
