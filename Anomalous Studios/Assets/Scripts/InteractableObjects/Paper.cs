using AudioSystem;
using TMPro;
using UnityEngine;

public class Paper : MonoBehaviour, IInteractable
{
    [SerializeField] private TextMeshProUGUI _description;
    [SerializeField] private Handbook_UI _handbook = null;
    [SerializeField] private float _holdTime = 0.0f;

    private ElevatorController _elevator;
    private string _taskID;
    private Renderer _renderer;
    private bool _isTask;

    // TODO: Change the initialization of the first level to be dynamic, raise an event
    // The level system is going to change pretty soon to accomadate new level box anyway
    [SerializeField] private bool _canInteract = true;
    public float HoldTime { get => _holdTime; }
    public bool CanInteract { get => _canInteract; set => _canInteract = value; }

    private EventBinding<LevelLoaded> _levelLoaded;

    [Header("Reaction SFX")]
    [SerializeField] private SoundDataSO _initialSFX;
    [SerializeField] private SoundDataSO _failedSFX;
    [SerializeField] private SoundDataSO _cancelSFX;
    [SerializeField] private SoundDataSO _successSFX;
    public SoundDataSO InitialSFX { get => _initialSFX; }
    public SoundDataSO FailedSFX { get => _failedSFX; }
    public SoundDataSO CancelSFX { get => _cancelSFX; }
    public SoundDataSO SuccessSFX { get => _successSFX; }

    /// <summary>
    /// Called on first active frame
    /// </summary>
    public void Start()
    {
        _elevator = transform.parent.parent.GetComponent<ElevatorController>();
        _renderer = GetComponent<Renderer>();
    }

    public void Highlight()
    {
        GetComponent<HighlightTarget>().IsHighlighted = true;
    }
    public void RemoveHighlight()
    {
        GetComponent<HighlightTarget>().IsHighlighted = false;
    }

    /// <summary>
    /// Leverages interact from interaction class
    /// </summary>
    public void Interact()
    {
        _elevator.RemoveNote(this);

        this.gameObject.SetActive(false);
        AddToHandbook();
    }

    /// <summary>
    /// Adds the rule or task to its respective page
    /// </summary>
    public void AddToHandbook()
    {
        if (_isTask)
        {
            _handbook.AddTask(_description.text);
        }
        else
        {
            _handbook.AddPolicy(_description.text);
        }
    }

    public void OnDrawGizmos()
    {
        //Gizmos.DrawWireSphere(_center, 1);
    }

    public void InitReferences(ElevatorController elevator, Handbook_UI handbook, bool isTask, string taskID, string desc)
    {
        _elevator = elevator;
        _handbook = handbook;
        _isTask = isTask;
        _taskID = taskID;
        _description.text = desc;
    }
}
