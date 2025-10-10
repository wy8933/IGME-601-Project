using UnityEngine;

#nullable enable

/// <summary>
/// Some object that the player can look at and hold F to interact with
/// </summary>
public interface IInteractable
{
    /// <summary>
    /// The current highest priority interactable that is accessible by the player. 
    /// </summary>
    public static IInteractable? Target { get; private set; }

    public static GameObject? Instigator;

    //public AudioClip Clip { get; set; }


    /// <summary>
    /// Decides whether an item can be interacted with
    /// TODO: call in UserInteraction to trigger SFX
    /// </summary>
    public bool CanInteract { get; set; }

    /// <summary>
    /// By default zero; how long it takes the player to hold the interaction until it takes effect
    /// </summary>
    public float HoldTime { get; }

    /// <summary>
    /// The public accessor to change the priority Target's interaction values. Called when obj is different than the Target.
    /// </summary>
    /// <param name="obj">The new IInteractable object under the player's crosshair</param>
    public static void SetPriorityTarget(IInteractable obj)
    {
        Target?.RemoveHighlight();

        obj?.Highlight();

        Target = obj;
    }

    /// <summary>
    /// A shader is applied to the interactable to highlight its boundry
    /// TODO: Might not need to be inherited, see if one method will affect all obj equally
    /// </summary>
    public void Highlight();

    /// <summary>
    /// Removes the shader highlight when move crosshair away
    /// </summary>
    public void RemoveHighlight();

    /// <summary>
    /// Performs the unique interaction of this object
    /// </summary>
    public void Interact();
}
