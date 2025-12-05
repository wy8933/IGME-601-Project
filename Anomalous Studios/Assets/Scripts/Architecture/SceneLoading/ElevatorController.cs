using AudioSystem;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Invoked when the player is allowed to move to the next floor
/// </summary>
public struct TasksComplete : IEvent { }

public class ElevatorController : MonoBehaviour
{
    /// <summary>
    /// EXTREMELY Temporary field, used to pass the player's position to the rulebroken event. 
    /// </summary>
    static public GameObject Player;

    [Header("Sound Effects")]
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
    private static ElevatorButton _closeButton;

    /// <summary>
    /// Stores all the buttons on the elevator, 
    /// </summary>
    private Dictionary<Level, TMP_Text> _indicators;
    private Dictionary<Level, PaperDataSO[]> _paperData;

    private EventBinding<TasksComplete> _tasksComplete;
    private EventBinding<LevelLoaded> _levelLoaded;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _corkboard = transform.Find("Corkboard");
        _handbook = GameObject.Find("MainUI").transform.Find("Handbook").GetComponent<Handbook_UI>();

        _openButton = transform.Find("Buttons/Open").GetComponent<ElevatorButton>();
        _closeButton = transform.Find("Buttons/Close").GetComponent<ElevatorButton>();

        // Keeps references to each of the Elevator floor buttons to enable / disable them
        _indicators = new Dictionary<Level, TMP_Text>
        {
            { Level.B1, transform.Find("LevelIndicators/18").GetComponentInChildren<TMP_Text>() },
            { Level.B2, transform.Find("LevelIndicators/10").GetComponentInChildren<TMP_Text>() },
            { Level.endGame, transform.Find("LevelIndicators/6").GetComponentInChildren<TMP_Text>() }
        };

        _paperData = new Dictionary<Level, PaperDataSO[]>
        {
            { Level.B1, _papersB1 },
            { Level.B2, _papersB2 }
        };

        Player = GameObject.Find("Player");
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
    private void SpawnNotes(LevelLoaded e)
    {
        // The player will continue to hold onto handbook info, no need to spawn it again
        if (e.prevLevel == SceneLoader.CurrentLevel) { return; }

        if (e.prevLevel != Level.mainMenu && e.prevLevel != null) _indicators[(Level)e.prevLevel].color = Color.white;
        _indicators[(Level)SceneLoader.CurrentLevel].color = Color.yellow;

        // TODO: Paper positions are hardcoded, make this dynamic to corkboard dimensions, random spawn pattern

        PaperDataSO[] PaperData = _paperData[(Level)SceneLoader.CurrentLevel];

        // Creates a new list of notes if empty, and clears it if not
        _notes?.Clear();
        _notes ??= new List<Paper>();

        float x = -1.6f;
        float y = 0.0f;

        // Spawns in each of the notes on the corkboard
        foreach (PaperDataSO data in PaperData) 
        {
            GameObject paper = Instantiate(_paperPrefab, _corkboard, false);
            float z = paper.transform.localPosition.z;

            paper.GetComponent<Paper>().InitReferences(this, _handbook, data.IsTask, data.TaskID, data.Description);
            paper.transform.localPosition = new Vector3(x, y, z);
            _notes.Add(paper.GetComponent<Paper>());

            x += 0.6f;
        }

        if (_notes.Count <= 0) { _openButton.Enable(); }
    }

    /// <summary>
    /// Called when the task of the level is complete, enabling progression to the next floor
    /// </summary>
    private void EnableNextLevel()
    {
        Level lvl = (Level)SceneLoader.CurrentLevel+1;

        _closeButton.Enable(lvl);
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
        _tasksComplete = new EventBinding<TasksComplete>(EnableNextLevel);
        EventBus<TasksComplete>.Register(_tasksComplete);
        _levelLoaded = new EventBinding<LevelLoaded>(SpawnNotes);
        EventBus<LevelLoaded>.Register(_levelLoaded);
    }

    public void OnDisable()
    {
        EventBus<TasksComplete>.DeRegister(_tasksComplete);
        EventBus<LevelLoaded>.DeRegister(_levelLoaded);
    }
}
