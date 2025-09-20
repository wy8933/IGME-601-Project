using UnityEngine;

#nullable enable

/// <summary>
/// Some object that the player can look at and hold F to interact with
/// </summary>
public abstract class Interaction : MonoBehaviour
{
    private static float timer = 0f;

    /// <summary>
    /// The current highest priority interactable that is accessible by the player. 
    /// </summary>
    public static Interaction? Target { get; private set; }

    /// <summary>
    /// The _holdTime value of the Target object
    /// </summary>
    public static float HoldTime { get; private set; }

    /// <summary>
    /// Decides whether an item can be interacted with
    /// TODO: Should this PREVENT an item from becoming a target? OR signal some error SFX in Interact()?
    /// TODO: if _canInteract swaps while holding down F, it should disengage the holding sequence
    /// </summary>
    public bool _canInteract = true;

    /// <summary>
    /// By default zero; how long it takes the player to hold the interaction until it takes effect
    /// </summary>
    [SerializeField] private float _holdTime = 0f;

    /// <summary>
    /// The public accessor to change the target priority values
    /// </summary>
    /// <param name="obj"></param>
    public static void SetPriorityTarget(Interaction obj)
    {
        Target = obj;
        HoldTime = obj != null ? obj._holdTime : 0f;
    }

    /// <summary>
    /// A shader is applied to the interactable to highlight its boundry
    /// TODO: Might not need to be abstract, see if one method will affect all obj equally
    /// </summary>
    public abstract void Highlight();

    /// <summary>
    /// If CanInteract is true, allows the user to perform interaction
    /// </summary>
    public abstract void Interact();
}
