using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Level
{
    currentLevel,
    blue,
    green,
    red
}

public struct LevelLoading : IEvent { public Level newLevel; }

public class SceneLoader : MonoBehaviour
{
    [Tooltip("Artificially creates some time in-between levels")]
    [Range(0.0f, 5.0f)]
    [SerializeField] private float _waitTime = 3.0f;

    // TODO: Temporary testing levels to be replaced with the grayboxed scenes
    [SerializeField] private SceneField[] _floorB7;
    [SerializeField] private SceneField[] _floorB6;
    [SerializeField] private SceneField[] _floorB5;

    /// <summary>
    /// Reference to the AI agent chasing the player
    /// </summary>
    //[SerializeField] private EnemyBehavior _ruleKeeper;

    private Dictionary<Level, SceneField[]> _floorLibrary;

    private Level _lastLevel;

    private List<AsyncOperation> _scenesToLoad = new List<AsyncOperation>();

    private Animator _animator;

    private EventBinding<LevelLoading> _levelLoading;

    public void Start()
    {
        // TODO: should these var be init here? Is there a better place to init all the var?
        VariableConditionManager.Instance.Set("TaskComplete", "true");
        VariableConditionManager.Instance.Set("IsLevelLoading", "true");

        _animator = GetComponent<Animator>();

        _floorLibrary = new Dictionary<Level, SceneField[]>
        {
            { Level.currentLevel, null },
            { Level.blue, _floorB7 },
            { Level.green, _floorB6 },
            { Level.red, _floorB5 }
        };

        // TODO: The initial scene load should all be done in one method (variables, items, etc.)
        // TODO: Load in the Rulekeeper every level, don't keep it active between scenes. Can throw errors w/ navmesh
        _lastLevel = Level.red;
        StartCoroutine(LoadScenes(new LevelLoading { newLevel = Level.red }));
    }

    /// <summary>
    /// EventBus event, starts the process of moving to the next level
    /// </summary>
    /// <param name="e"></param>
    private void OnLevelLoaded(LevelLoading e)
    {
        _animator.SetTrigger("moveDoors");

        StartCoroutine(LoadScenes(e));
    }

    /// <summary>
    /// Waits for scenes to unload and load before opening the elevator doors
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    private IEnumerator LoadScenes(LevelLoading e)
    {
        // TODO: Refactor this to use some sort of bitmask. Right now just brute-forcing it
        // This should get rid of the need for _lastLevel as well

        // Wait for the doors to close before unloading any scenes
        while (_animator.GetBool("isOpen")) { yield return null; }

        //_ruleKeeper.resetRuleKeeper(new Vector3(-14, 2.5f, 0.0f));

        // Unload all the scenes from the previous level, and load in all the new level's scenes
        if (_lastLevel != e.newLevel)
        {
            foreach (SceneField scene in _floorLibrary[_lastLevel])
            {
                _scenesToLoad.Add(SceneManager.UnloadSceneAsync(scene));
            }
        }

        foreach (SceneField scene in _floorLibrary[e.newLevel])
        {
            _scenesToLoad.Add(SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive));
        }
        
        _lastLevel = e.newLevel;

        // Wait until the scenes are all loaded before opening the doors
        if (!_scenesToLoad[_scenesToLoad.Count - 1].isDone) { yield return null; }

        //_ruleKeeper.resetRuleKeeper(); 

        // Artificially create some amount of time in the elevator for ambiance and SFX
        if (_waitTime != 0) { yield return new WaitForSeconds(_waitTime); }

        VariableConditionManager.Instance.Set("IsLevelLoading", "false");
        
        _scenesToLoad.Clear();

        _animator.SetTrigger("moveDoors");
    }

    /// <summary>
    /// If the player dies or wants to reload the level, restarts THIS level
    /// </summary>
    public void ResetScene()
    {
        OnLevelLoaded(new LevelLoading { newLevel = _lastLevel } );
    }

    /// <summary>
    /// Waits for the close / open animation to fully finish before changing values
    /// </summary>
    public void IsOpen()
    {
        _animator.SetBool("isOpen", !_animator.GetBool("isOpen"));
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
        _levelLoading = new EventBinding<LevelLoading>(OnLevelLoaded);
        EventBus<LevelLoading>.Register(_levelLoading);
    }

    public void OnDisable()
    {
        EventBus<LevelLoading>.DeRegister(_levelLoading);
    }
}
