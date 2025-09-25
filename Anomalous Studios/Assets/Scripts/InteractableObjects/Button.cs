using UnityEngine;

/// <summary>
/// A test-case for the interactable interface
/// </summary>
public class Button : Interaction
{
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
        // TODO: set value of elevator in Start()

        // If the player is in the elevator
        if (VariableConditionManager.Instance.Get("InElevator").Equals("true") &&
            VariableConditionManager.Instance.Get("TaskComplete").Equals("false"))
        {            
            EventBus<LevelLoading>.Raise(new LevelLoading { newLevel = _level });

            //_elevator.SetBool("is_open", !_elevator.GetBool("is_open"));
        }

    }

    // Press the button
    // Start the animation to close the door
    // When the animation is finished, unload and load the levels
    // When the levels are finished loading, open the door

    // Press the to raise an event to open the doors
    
    // the event starts a trigger in the animator
        // 



}
