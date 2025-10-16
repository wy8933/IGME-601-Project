using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
public struct LoadLevel : IEvent { public LevelTESTING newLevel; }

public enum LevelTESTING
{
    currentLevel,
    mainMenu,
    B1,
    B2,
    B3
}

public class SceneLoadingManager : MonoBehaviour
{
    // TODO: Fade in and fade out a black screen BEFORE the loading process and AFTER the loading is fully done
    public static LevelTESTING CurrentLevel { get; private set; } = LevelTESTING.mainMenu;

    [Header("Level Listings")]
    [SerializeField] private SceneField[] _mainMenu;
    [SerializeField] private SceneField[] _floorB1;
    [SerializeField] private SceneField[] _floorB2;
    [SerializeField] private SceneField[] _floorB3;

    private Dictionary<LevelTESTING, SceneField[]> _floorLibrary;

    private List<AsyncOperation> _scenesToLoad = new List<AsyncOperation>();

    private EventBinding<LoadLevel> _levelLoading;

    private LevelLoaded _levelLoaded;

    private GameObject _blackScreenTEMP;

    void Start()
    {
        VariableConditionManager.Instance.Set("TaskComplete", "true");
        VariableConditionManager.Instance.Set("IsLevelLoading", "true");

        _floorLibrary = new Dictionary<LevelTESTING, SceneField[]>
        {
            { LevelTESTING.currentLevel, _mainMenu },
            { LevelTESTING.mainMenu, _mainMenu },
            { LevelTESTING.B1, _floorB1 },
            { LevelTESTING.B2, _floorB2 },
            { LevelTESTING.B3, _floorB3 }
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
    }

    private void LoadLevel(LoadLevel e)
    {
        StartCoroutine(WaitForScenes(e));
    }

    public IEnumerator WaitForScenes(LoadLevel e)
    {
        // TODO: fade to black loading screen
        _blackScreenTEMP.SetActive(true);

        if (_floorLibrary[LevelTESTING.currentLevel] != _floorLibrary[e.newLevel])
        {

            foreach (SceneField scene in _floorLibrary[e.newLevel])
            {
                _scenesToLoad.Add(SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive));
            }

            if (_floorLibrary[LevelTESTING.currentLevel] != null)
            {
                foreach (SceneField scene in _floorLibrary[LevelTESTING.currentLevel])
                {
                    _scenesToLoad.Add(SceneManager.UnloadSceneAsync(scene));
                }
            }

            // Wait until the scenes are all loaded
            if (!_scenesToLoad[_scenesToLoad.Count - 1].isDone) { yield return null; }
        }

        _floorLibrary[LevelTESTING.currentLevel] = _floorLibrary[e.newLevel];

        // Comment out when testing. Cheaper to artifically extend load screen time to cover any last moments of lag
        yield return new WaitForSeconds(0.5f);

        EventBus<LevelLoaded>.Raise(_levelLoaded);

        // TODO: fade from black loading screen
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
