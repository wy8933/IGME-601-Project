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
    [Header("Sound Effects")]
    [SerializeField] private SoundDataSO _floorChime;
    [SerializeField] private SoundDataSO _doorsMoving;

    private Animator _animator;
    private Transform _corkboard;
    private Handbook_UI _handbook;

    private List<Paper> _notes;

    [Header("Paper Data")]
    [SerializeField] private GameObject _paperPrefab;
    [SerializeField] private PaperDataSO[] _papersB1;
    [SerializeField] private PaperDataSO[] _papersB2;

    private static ElevatorButton _openButton;
    /// <summary>
    /// Stores all the buttons on the elevator, 
    /// </summary>
    private Dictionary<Level, ElevatorButton> _buttons;
    private Dictionary<Level, PaperDataSO[]> _paperData;

    private EventBinding<TaskComplete> _taskComplete;
    private EventBinding<LevelLoaded> _levelLoaded;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _corkboard = transform.Find("Corkboard");
        _handbook = GameObject.Find("MainUI").transform.Find("Handbook").GetComponent<Handbook_UI>();

        _openButton = transform.Find("Buttons/Open").GetComponent<ElevatorButton>();

        // Keeps references to each of the Elevator floor buttons to enable / disable them
        _buttons = new Dictionary<Level, ElevatorButton>
        {
            { Level.B1, transform.Find("Buttons/B1").GetComponent<ElevatorButton>() },
            { Level.B2, transform.Find("Buttons/B2").GetComponent<ElevatorButton>() },
            { Level.endGame, transform.Find("Buttons/B3").GetComponent<ElevatorButton>() }
        };

        _paperData = new Dictionary<Level, PaperDataSO[]>
        {
            { Level.B1, _papersB1 },
            { Level.B2, _papersB2 }
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
    /// Instantiates the papers on the corkboard per each level
    /// </summary>
    public void SpawnNotes(LevelLoaded e)
    {
        // The player will continue to hold onto handbook info, no need to spawn it again
        if (e.prevLevel == SceneLoader.CurrentLevel) { return; }

        // TODO: Paper positions are hardcoded, make this dynamic to corkboard dimensions, random spawn pattern

        PaperDataSO[] PaperData = _paperData[(Level)SceneLoader.CurrentLevel];

        // Creates a new list of notes if empty, and clears it if not
        _notes?.Clear();
        _notes ??= new List<Paper>();

        float x = -1.25f;
        float y = 0.6f;

        // Spawns in each of the notes on the corkboard
        foreach (PaperDataSO data in PaperData) 
        {
            GameObject paper = Instantiate(_paperPrefab, _corkboard, false);
            float z = paper.transform.localPosition.z;

            paper.GetComponent<Paper>().InitReferences(this, _handbook, data.IsTask, data.TaskID, data.Description);
            paper.transform.localPosition = new Vector3(x, y, z);
            _notes.Add(paper.GetComponent<Paper>());

            x += 0.75f;
            if (x >= 1.25f)
            {
                x = -1.5f;
                y = -0.6f;
            }
        }

        if (_notes.Count <= 0) { _openButton.Enable(); }
    }

    /// <summary>
    /// Called when the task of the level is complete, enabling the next floor's button
    /// </summary>
    public void EnableElevatorButtons()
    {
        _buttons[(Level)SceneLoader.CurrentLevel+1].Enable();
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
