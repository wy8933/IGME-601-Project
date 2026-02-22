using AudioSystem;
using UnityEngine;

/// <summary>
/// Elevator buttons to navigate the basement floors
/// </summary>
public class ElevatorButton : MonoBehaviour, IInteractable
{
    private GameObject _playerGO;
    private PlayerController _playerController;

    [Tooltip("Whether this button opens or closes the doors")]
    [SerializeField] private bool _isOpenButton = true;

    [SerializeField] private float _holdTime = 0.0f;

    [Header("Reaction SFX")]
    [SerializeField] private SoundDataSO _failedSFX;
    [SerializeField] private SoundDataSO _successSFX;
    [SerializeField] private SoundDataSO _floorChime;

    public SoundDataSO InitialSFX => null;
    public SoundDataSO FailedSFX { get => _failedSFX; }
    public SoundDataSO CancelSFX => null;
    public SoundDataSO SuccessSFX { get => _successSFX; }

    private ElevatorController _elevator;
    private Level? _nextLevel;

    /// <summary>
    /// FIX: By default false for elevator buttons, must meet some precondition each time anyway to unlock.
    /// </summary>
    private bool _canInteract = false;

    public bool CanInteract { get => _canInteract; set => _canInteract = value; }
    
    public float HoldTime { get => _holdTime; }

    public void Start()
    {
        _elevator = transform.parent.parent.GetComponent<ElevatorController>();

        // Get references to player and player controller
        _playerGO = GameObject.FindGameObjectWithTag("Player");
        _playerController = _playerGO.GetComponent<PlayerController>();
    }

    public void Highlight()
    {
        if (_canInteract) 
        { 
            GetComponent<AutoOutline>().IsHighlighted = true;
            // Displays Helper UI Text
            IInteractable.DisplayHelperText(_playerController);
        }
    }

    public void RemoveHighlight()
    {
        if (_canInteract) 
        { 
            GetComponent<AutoOutline>().IsHighlighted = false;
            // Hides Helper UI Text
            IInteractable.HideHelperText(_playerController);
        }
    }

    public void Interact()
    {
        _elevator.OpenDoors();

        if (!_isOpenButton) { EventBus<LoadLevel>.Raise(new LoadLevel { newLevel = (Level)_nextLevel }); }

        Disable();
    }

    /// <summary>
    /// Enables the usage of this button
    /// </summary>
    public void Enable(Level? nextLevel = null)
    {
        AudioManager.Instance.Play(_floorChime, transform.position);

        _nextLevel = nextLevel;

        _canInteract = true;
    }

    /// <summary>
    /// Prevents the usage of this button
    /// </summary>
    public void Disable()
    {
        RemoveHighlight();

        _canInteract = false;
    }
}
