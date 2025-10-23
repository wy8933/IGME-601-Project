using AudioSystem;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Raised when 
/// </summary>
public struct TaskComplete : IEvent { }

public class ElevatorController : MonoBehaviour
{
    [SerializeField] private SoundDataSO _floorChime;
    [SerializeField] private SoundDataSO _doorsMoving;

    private Animator _animator;
    private GameObject _corkboard;

    // TODO: Temporarily assigned in inspector, should be spawned in my some the SceneLoader based on type of level
    // TODO: replace with an actual array, using id values when creating the notes
    [SerializeField] private List<Paper> _notes;

    private Dictionary<Level, ElevatorButton> _buttons;
    private static ElevatorButton _openButton;

    private EventBinding<TaskComplete> _taskComplete;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _corkboard = transform.Find("Corkboard").gameObject;

        _openButton = transform.Find("Buttons/Open").GetComponent<ElevatorButton>();
        _buttons = new Dictionary<Level, ElevatorButton>
        {
            { Level.B1, transform.Find("Buttons/B1").GetComponent<ElevatorButton>() },
            { Level.B2, transform.Find("Buttons/B2").GetComponent<ElevatorButton>() },
            { Level.B3, transform.Find("Buttons/B3").GetComponent<ElevatorButton>() }
        };
    }

    /// <summary>
    /// Waits for the close / open animation to fully finish before changing values
    /// </summary>
    public void IsOpen()
    {
        _animator.SetBool("isOpen", !_animator.GetBool("isOpen"));
    }

    public void OpenDoors()
    {
        AudioManager.Instance.Play(_doorsMoving, transform.position);

        AudioManager.Instance.Play(_floorChime, transform.position);

        _animator.SetTrigger("moveDoors");
    }

    /// <summary>
    /// Keeps track of whether all the notes have been collected by the player
    /// </summary>
    /// <param name="note"></param>
    public void RemoveNote(Paper note)
    {
        _notes.Remove(note);

        if (_notes.Count <= 0 ) { _openButton.Enable(); }
    }

    /// <summary>
    /// Called when the task of the level is complete, enabling the next floor's button
    /// </summary>
    public void EnableElevatorButtons()
    {
        _buttons[(Level)SceneLoader.CurrentLevel+1].Enable();
    }

    /// <summary>
    /// TODO: Initializes the notes on the corkboard when the scene loads
    /// </summary>
    /// <param name="Notes">The Papers to spawn on the corkboard</param>
    public void SpawnNotes(List<Paper> Notes)
    {
        _notes = Notes;

        foreach (Paper paper in Notes) 
        {
            // TODO: spawn all of the notes into the scene on the _corkboard
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        VariableConditionManager.Instance.Set("InElevator", "true");
    }

    public void OnTriggerExit(Collider other)
    {
        VariableConditionManager.Instance.Set("InElevator", "false");
    }

    public void OnEnable()
    {
        _taskComplete = new EventBinding<TaskComplete>(EnableElevatorButtons);
        EventBus<TaskComplete>.Register(_taskComplete);
    }

    public void OnDisable()
    {
        EventBus<TaskComplete>.DeRegister(_taskComplete);
    }
}
