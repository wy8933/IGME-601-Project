using AudioSystem;
using UnityEngine;

public enum ButtonType
{
    Level,
    Close,
    Open
}

/// <summary>
/// Elevator buttons to navigate the basement floors
/// </summary>
public class Button : MonoBehaviour, IInteractable
{
    [SerializeField] private ButtonType _buttonType = ButtonType.Level;
    [SerializeField] private LevelTESTING _level;
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

        // TEMP: remove when level loading is done testing with
        if (_buttonType == ButtonType.Level)
            Enable();

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
            case ButtonType.Level:
                EventBus<LoadLevel>.Raise(new LoadLevel { newLevel = _level } );
                // Disable(); TEMP: use disable when we are able to move to the next levels
                break;
            
            case ButtonType.Open:
                _elevator.OpenDoors();
                Disable();
                break;

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
