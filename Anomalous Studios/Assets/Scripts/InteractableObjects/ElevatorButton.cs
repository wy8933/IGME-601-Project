using AudioSystem;
using UnityEngine;

/// <summary>
/// The purpose of this button. There should only ever be one close and one open button, the rest move the player between Levels
/// </summary>
public enum ButtonType
{
    Level,
    Close,
    Open
}

/// <summary>
/// Elevator buttons to navigate the basement floors
/// </summary>
public class ElevatorButton : MonoBehaviour, IInteractable
{
    [Tooltip("The purpose of this button.")]
    [SerializeField] private ButtonType _buttonType = ButtonType.Level;

    [Tooltip("This button will send the player to this level")]
    [SerializeField] private Level _level;
    [SerializeField] private float _holdTime = 0.0f;

    [Header("Reaction SFX")]
    [SerializeField] private SoundDataSO _failedSFX;
    [SerializeField] private SoundDataSO _successSFX;
    public SoundDataSO InitialSFX => null;
    public SoundDataSO FailedSFX { get => _failedSFX; }
    public SoundDataSO CancelSFX => null;
    public SoundDataSO SuccessSFX { get => _successSFX; }

    private Renderer _renderer;
    private ElevatorController _elevator;

    /// <summary>
    /// FIX: By default false for elevator buttons, must meet some precondition each time anyway to unlock.
    /// </summary>
    private bool _canInteract = false;

    public bool CanInteract { get => _canInteract; set => _canInteract = value; }
    
    public float HoldTime { get => _holdTime; }

    public void Start()
    {
        _renderer = GetComponent<Renderer>();
        _elevator = transform.parent.parent.GetComponent<ElevatorController>();
    }

    public void Highlight()
    {
        GetComponent<HighlightTarget>().IsHighlighted = true;
    }

    public void RemoveHighlight()
    {
        GetComponent<HighlightTarget>().IsHighlighted = false;
    }

    public void Interact()
    {
        switch (_buttonType)
        {
            // When a task has been completed, used to move to the very next level
            case ButtonType.Level:
                _elevator.OpenDoors();
                EventBus<LoadLevel>.Raise(new LoadLevel { newLevel = _level } );
                Disable();
                break;
            
            // After all the papers have been collected, used to open the elevator doors
            case ButtonType.Open:
                _elevator.OpenDoors();
                Disable();
                break;

            // The close button in the elevator is a joke, might make an electrical sounds when pressed, or fall off the wall
            case ButtonType.Close:
                Debug.Log("Lmao");
                break;
        }

    }

    /// <summary>
    /// Highlights this button to be pressed by the player
    /// </summary>
    public void Enable()
    {
        // A temporary "glow" effect to prompt the player to press this button, should probably pulse yellow eventually
        _renderer.material.color = Color.yellow;
        _canInteract = true;
    }

    /// <summary>
    /// Prevents the usage of this button
    /// </summary>
    public void Disable()
    {
        _renderer.material.color = Color.black;
        _canInteract = false;
    }
}
