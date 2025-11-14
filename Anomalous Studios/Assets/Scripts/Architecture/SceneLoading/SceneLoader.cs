using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

/// <summary>
/// Raised when a level has been fully loaded, shoud alert managers to start referencing objects in the scene.
/// </summary>
public struct LevelLoaded : IEvent { public Level? prevLevel; }

/// <summary>
/// Raised when a script wants to load a new level. Called by the elevator and menus.
/// </summary>
public struct LoadLevel : IEvent { public Level newLevel; }

public enum Level
{
    mainMenu,
    B1,
    B2,
    endGame
}

public class SceneLoader : MonoBehaviour
{
    // TODO: Fade in and fade out a black screen BEFORE the loading process and AFTER the loading is fully done
    public static Level? CurrentLevel { get; private set; }

    [Header("Level Listings")]
    [SerializeField] private SceneField _elevator;
    [SerializeField] private SceneField _mainMenu;
    [SerializeField] private SceneField _floorB1;
    [SerializeField] private SceneField _floorB2;

    private Dictionary<Level, SceneField> _floorLibrary;

    private List<AsyncOperation> _scenesToLoad = new List<AsyncOperation>();

    private EventBinding<LoadLevel> _levelLoading;

    private GameObject _blackScreenTEMP;
    private NavMeshSurface _navMeshSurface;

    public void Start()
    {
        _floorLibrary = new Dictionary<Level, SceneField>
        {
            { Level.mainMenu, _mainMenu },
            { Level.B1, _floorB1 },
            { Level.B2, _floorB2 }
        };

        _blackScreenTEMP = GameObject.Find("MainUI").transform.Find("LoadingScreen").gameObject;
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
        _scenesToLoad.Clear();

        // If we are in the main menu going to a level, load the elevator

        if (CurrentLevel == Level.mainMenu && e.newLevel != Level.mainMenu)
        {
            _scenesToLoad.Add(SceneManager.LoadSceneAsync(_elevator, LoadSceneMode.Additive));
        }

        // Unload the old scenes, load in the new
        _scenesToLoad.Add(SceneManager.LoadSceneAsync(_floorLibrary[e.newLevel], LoadSceneMode.Additive));

        if (CurrentLevel != null)
        {
            _scenesToLoad.Add(SceneManager.UnloadSceneAsync(_floorLibrary[(Level)CurrentLevel]));
        }

        // If we are not in the main menu going to the main menu, unload the elevator
        if (e.newLevel == Level.mainMenu && CurrentLevel != Level.mainMenu && CurrentLevel != null)
        {
            _scenesToLoad.Add(SceneManager.UnloadSceneAsync(_elevator));
        }

        Level? prevLevel = CurrentLevel;
        CurrentLevel = e.newLevel;

        while (!_scenesToLoad[_scenesToLoad.Count - 1].isDone) { yield return null; }

        // Add an artifical amount of loading time to smooth everything over
        yield return new WaitForSeconds(0.5f);

        EventBus<LevelLoaded>.Raise(new LevelLoaded { prevLevel = prevLevel });

        _blackScreenTEMP.SetActive(false);
    }

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
