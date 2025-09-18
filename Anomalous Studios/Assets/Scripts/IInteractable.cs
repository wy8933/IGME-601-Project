using UnityEngine;

#nullable enable

/// <summary>
/// Some object that the player can look at and hold F to interact with
/// </summary>
public interface IInteractable
{
    // TODO: check file against style guide

    /// <summary>
    /// The current accessible highest priority interactable to the player
    /// </summary>
    public static IInteractable? Target { get; set; }

    /// <summary>
    /// Decides whether an item can be interacted with
    /// TODO: Should this PREVENT an item from becoming a target? OR signal some error SFX in Interact()?
    /// </summary>
    public bool CanInteract { get; set; }

    /// <summary>
    /// A shader is applied to the interactable to highlight its boundry
    /// </summary>
    public void Highlight();

    /// <summary>
    /// If CanInteract is true, allows the user to perform interaction
    /// </summary>
    public void Interact();
}
