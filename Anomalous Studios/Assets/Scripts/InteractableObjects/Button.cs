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
    /// By default false for elevator buttons, must meet some precondition each time anyway to unlock.
    /// </summary>
    private bool _canInteract = true;

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
            case ButtonType.Level:
                Debug.Log("Go to level");
                _elevator.OpenDoors();
                //_canInteract = false;
                break;
            
            case ButtonType.Open:
                Debug.Log("Close doors");
                if (VariableConditionManager.Instance.Get("TaskComplete") === "true")
                {
                    _elevator.OpenDoors();
                    //_canInteract = false;
                }
                break;

            case ButtonType.Close:
                Debug.Log("Lmao");
                break;
        }

    }
}
