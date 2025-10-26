using AudioSystem;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Invoked when the player is allowed to move to the next floor
/// TODO: Decide whether to move to a rule manager, or keep in ElevatorController
/// </summary>
public struct TaskComplete : IEvent { }

public class ElevatorController : MonoBehaviour
{
    [SerializeField] private GameObject _paperPrefab;
    [SerializeField] private SoundDataSO _floorChime;
    [SerializeField] private SoundDataSO _doorsMoving;

    private Animator _animator;
    private Transform _corkboard;

    // TODO: Temporarily assigned in inspector, should be spawned in my some the SceneLoader based on type of level
    // TODO: replace with an actual array, using id values when creating the notes
    //[SerializeField] private List<Paper> _notes;
    private List<Paper> _notes;

    private Dictionary<Level, ElevatorButton> _buttons;
    private static ElevatorButton _openButton;

    private EventBinding<TaskComplete> _taskComplete;
    private EventBinding<LevelLoaded> _levelLoaded;

    public void Start()
    {
        _notes = new List<Paper>();

        _animator = GetComponent<Animator>();
        _corkboard = transform.Find("Corkboard");

        _openButton = transform.Find("Buttons/Open").GetComponent<ElevatorButton>();
        _buttons = new Dictionary<Level, ElevatorButton>
        {
            { Level.onboarding, transform.Find("Buttons/B1").GetComponent<ElevatorButton>() },
            { Level.firstLevel, transform.Find("Buttons/B2").GetComponent<ElevatorButton>() },
            { Level.mainMenu, transform.Find("Buttons/B3").GetComponent<ElevatorButton>() }
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
        // TODO: We need to go to some final scene rather than the main menu
        _buttons[(Level)(((int)SceneLoader.CurrentLevel+1)%(int)(Level.firstLevel+1))].Enable();
    }

    /// <summary>
    /// Instantiates the physical papers on the corkboard per the level data
    /// </summary>
    /// <param name="PaperData">The paper data from each unique level</param>
    public void SpawnNotes(LevelLoaded e)
    {
        PaperDataSO[] PaperData = e._papers;
        _notes?.Clear();

        float x = -1.0f;
        float y = 0.5f;
        foreach (PaperDataSO data in PaperData) 
        {
            GameObject paper = Instantiate(_paperPrefab, _corkboard, false);

            float z = paper.transform.localPosition.z;

            paper.GetComponent<Paper>().InitReferences(this, data.IsTask, data.TaskID, data.Description);
            paper.transform.localPosition = new Vector3(x, y, z);

            // TODO: Make position data dynamic to corkboard. This is hardcoded for its current size
            x += 0.5f;
            if (x >= 1.0f)
            {
                x = -1.0f;
                y -= 0.5f;
            }

            _notes.Add(paper.GetComponent<Paper>());
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
        _levelLoaded = new EventBinding<LevelLoaded>(SpawnNotes);
        EventBus<LevelLoaded>.Register(_levelLoaded);
    }

    public void OnDisable()
    {
        EventBus<TaskComplete>.DeRegister(_taskComplete);
        EventBus<LevelLoaded>.DeRegister(_levelLoaded);
    }
}
