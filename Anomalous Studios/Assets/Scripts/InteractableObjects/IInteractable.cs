using UnityEngine;

#nullable enable

/// <summary>
/// Some object that the player can look at and hold F to interact with
/// </summary>
public interface IInteractable
{
    /// <summary>
    /// Counts down from the priority object's _holdTime value
    /// LEGACY TODO: remove
    /// </summary>
    //private static float _timer = 0f;

    /// <summary>
    /// Stores the held state of the target 
    /// LEGACY TODO: remove
    /// </summary>
    //public static bool isPressed = false;

    /// <summary>
    /// The current highest priority interactable that is accessible by the player. 
    /// </summary>
    public static IInteractable? Target { get; private set; }

    public static GameObject? Instigator;

    //public AudioClip Clip { get; set; }


    /// <summary>
    /// Decides whether an item can be interacted with
    /// TODO: Should this PREVENT an item from becoming a target? OR signal some error SFX in Interact()?
    /// TODO: if _canInteract swaps while holding down F, it should disengage the holding sequence
    /// </summary>
    public bool CanInteract { get; set; }

    /// <summary>
    /// By default zero; how long it takes the player to hold the interaction until it takes effect
    /// LEGACY TODO: remove, players will just have to hold buttons for a fixed amt of time, or deal with it in an override
    /// </summary>
    //[SerializeField] private float _holdTime = 0f;

    /// <summary>
    /// Children of Interaction class MUST call base.Update() to function as anticpated
    /// </summary>
    //public virtual void Update()
    //{
    //    // Only the priority Target ever updates and only when the interact key is pressed
    //    if (Target == this)
    //    {
    //        if (0 <= 0) 
    //        { 
    //            Interact();
    //        }
    //
    //    }
    //}

    /// <summary>
    /// The public accessor to change the priority Target's interaction values
    /// </summary>
    /// <param name="obj"></param>
    public static void SetPriorityTarget(IInteractable obj)
    {
        // When the player looks at another interaction, update the priority Target values
        if (Target != obj)
        {
            if (Target != null)
                Target.RemoveHighlight();

            if (obj != null)
                obj.Highlight();

            Target = obj;
        }
    }

    /// <summary>
    /// A shader is applied to the interactable to highlight its boundry
    /// TODO: Might not need to be abstract, see if one method will affect all obj equally
    /// </summary>
    public void Highlight();


    public void RemoveHighlight();

    /// <summary>
    /// Performs the unique interaction of this object
    /// </summary>
    public void Interact();
}
