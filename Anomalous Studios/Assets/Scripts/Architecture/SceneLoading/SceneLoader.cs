using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

/// <summary>
/// Raised when a level has been fully loaded, shoud alert managers to start referencing objects in the scene.
/// Passes references of managers 
/// </summary>
public struct LevelLoaded : IEvent 
{
    public Handbook_UI _handbook;
}

/// <summary>
/// Raised when a script wants to load a new level. Called by the elevator and menus.
/// </summary>
public struct LoadLevel : IEvent { public Level newLevel; }

public enum Level
{
    mainMenu,
    B1,
    B2,
    B3
}

public class SceneLoader : MonoBehaviour
{
    // TODO: Fade in and fade out a black screen BEFORE the loading process and AFTER the loading is fully done
    public static Level ?CurrentLevel { get; private set; }

    [Header("Level Listings")]
    [SerializeField] private SceneField _elevator;
    [SerializeField] private SceneField[] _mainMenu;
    [SerializeField] private SceneField[] _floorB1;
    [SerializeField] private SceneField[] _floorB2;
    [SerializeField] private SceneField[] _floorB3;

    private Dictionary<Level, SceneField[]> _floorLibrary;

    private List<AsyncOperation> _scenesToLoad = new List<AsyncOperation>();

    private EventBinding<LoadLevel> _levelLoading;

    private LevelLoaded _levelLoaded;

    private GameObject _blackScreenTEMP;
    private NavMeshSurface _navMeshSurface;

    public void Start()
    {
        //VariableConditionManager.Instance.Set("TaskComplete", "false"); // TODO: Remove, legacy
        //VariableConditionManager.Instance.Set("IsLevelLoading", "true"); // TODO: Remove, legacy

        _floorLibrary = new Dictionary<Level, SceneField[]>
        {
            { Level.mainMenu, _mainMenu },
            { Level.B1, _floorB1 },
            { Level.B2, _floorB2 },
            { Level.B3, _floorB3 }
        };

        GameObject _mainUI = GameObject.Find("MainUI");

        // Holds a reference to any managers, passes them along when the level is loaded to help initialize new scenes
        // transform.Find() is able to search for inactive objects, unlike GameObject.Find()
        // QUESTION: Should the managers all be singletons since they are persistent, rather than passing their references?
        _levelLoaded = new LevelLoaded
        {
            _handbook = _mainUI.transform.Find("Handbook").GetComponent<Handbook_UI>()
        };

        _blackScreenTEMP = _mainUI.transform.Find("LoadingScreen").gameObject;
        _navMeshSurface = GetComponent<NavMeshSurface>();


        // TODO: Need to move between cameras better, from the main menu to the player controller. Only one audio listener
        LoadLevel(new LoadLevel { newLevel = Level.mainMenu });
    }

    private void LoadLevel(LoadLevel e)
    {
        _blackScreenTEMP.SetActive(true);

        StartCoroutine(WaitForScenes(e));
    }

    /// <summary>
    /// Displays the loading screen, and loads all the new scenes additively. The navmesh is rebaked after all scenes are loaded safely
    /// </summary>
    /// <param name="e"></param>
    private IEnumerator WaitForScenes(LoadLevel e)
    {
        // If we are in the main menu going to a level, load the elevator

        if (CurrentLevel == Level.mainMenu && e.newLevel != Level.mainMenu)
        {
            _scenesToLoad.Add(SceneManager.LoadSceneAsync(_elevator, LoadSceneMode.Additive));
        }

        // Load all of the new level scenes
        foreach (SceneField scene in _floorLibrary[e.newLevel])
        {
            _scenesToLoad.Add(SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive));
        }

        // unload all of the old level scenes
        if (CurrentLevel != null)
        {
            foreach (SceneField scene in _floorLibrary[(Level)CurrentLevel])
            {
                _scenesToLoad.Add(SceneManager.UnloadSceneAsync(scene));
            }
        }

        // If we are not in the main menu going to the main menu, unload the elevator
        if (e.newLevel == Level.mainMenu && CurrentLevel != Level.mainMenu && CurrentLevel != null)
        {
            _scenesToLoad.Add(SceneManager.UnloadSceneAsync(_elevator));
        }

        CurrentLevel = e.newLevel;

        if (!_scenesToLoad[_scenesToLoad.Count - 1].isDone) { yield return null; }

        // An artifical amount of loading time to prevent the NavMesh from trying to rebuild too quickly
        yield return new WaitForSeconds(0.5f);

        _scenesToLoad.Add(_navMeshSurface.UpdateNavMesh(_navMeshSurface.navMeshData));

        if (!_scenesToLoad[_scenesToLoad.Count - 1].isDone) { yield return null; }

        EventBus<LevelLoaded>.Raise(_levelLoaded);

        _blackScreenTEMP.SetActive(false);
    }

    // TODO: add a method that only resets the current scene, doesn't worry about resetting the whole thing


    public void OnEnable()
    {
        _levelLoading = new EventBinding<LoadLevel>(LoadLevel);
        EventBus<LoadLevel>.Register(_levelLoading);
    }

    public void OnDisable()
    {
        EventBus<LoadLevel>.DeRegister(_levelLoading);
    }
}
