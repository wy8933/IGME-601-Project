using UnityEngine;

#nullable enable

/// <summary>
/// Some object that the player can look at and hold F to interact with
/// </summary>
public abstract class Interaction : MonoBehaviour
{
    /// <summary>
    /// Counts down from the priority object's _holdTime value
    /// </summary>
    private static float _timer = 0f;

    /// <summary>
    /// Stores the held state of the target 
    /// </summary>
    public static bool isPressed = false;

    /// <summary>
    /// The current highest priority interactable that is accessible by the player. 
    /// </summary>
    public static Interaction? Target { get; private set; }

    public static GameObject? Instigator;

    /// <summary>
    /// Decides whether an item can be interacted with
    /// TODO: Should this PREVENT an item from becoming a target? OR signal some error SFX in Interact()?
    /// TODO: if _canInteract swaps while holding down F, it should disengage the holding sequence
    /// </summary>
    public bool canInteract = true;

    /// <summary>
    /// By default zero; how long it takes the player to hold the interaction until it takes effect
    /// </summary>
    [SerializeField] private float _holdTime = 0f;

    /// <summary>
    /// Children of Interaction class MUST call base.Update() to function as anticpated
    /// </summary>
    public virtual void Update()
    {
        // Only the priority Target ever updates and only when the interact key is pressed
        if (Target == this && isPressed)
        {
            if (_timer <= 0) 
            { 
                isPressed = false;
                Interact();
            }

            else { _timer -= Time.deltaTime; }
        }
    }

    /// <summary>
    /// The public accessor to change the priority Target's interaction values
    /// </summary>
    /// <param name="obj"></param>
    public static void SetPriorityTarget(Interaction obj)
    {
        // When the player looks at another interaction, update the priority Target values
        if (Target != obj)
        {
            isPressed = false;
            Target = obj;
            _timer = obj != null ? obj._holdTime : 0f;
        }

        // If the player lets go of the interaction but continues to look at it, reset the timer
        else if (!isPressed)
        {
            _timer = obj != null ? obj._holdTime : 0f;
        }
    }

    /// <summary>
    /// A shader is applied to the interactable to highlight its boundry
    /// TODO: Might not need to be abstract, see if one method will affect all obj equally
    /// </summary>
    public abstract void Highlight();

    /// <summary>
    /// Performs the unique interaction of this object
    /// </summary>
    protected abstract void Interact();
}
