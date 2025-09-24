using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Level
{
    blue,
    green,
    red
}

public struct LevelLoading : IEvent { public Level name; }

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private SceneField[] _blueLevel;
    [SerializeField] private SceneField[] _redLevel;

    private Dictionary<Level, SceneField[]> dict;

    private List<AsyncOperation> _scenesToLoad = new List<AsyncOperation>();

    private EventBinding<LevelLoading> _levelLoading;

    void Start()
    {
        dict = new Dictionary<Level, SceneField[]>
        {
            { Level.blue, _blueLevel },
            { Level.red, _redLevel }
        };

        // Initializes the starting level
        OnLevelLoaded(new LevelLoading { name = Level.blue });

        // Assumes the player spawn in the Elevator to start
        VariableConditionManager.Instance.Set("TaskComplete", "true");
    }


    private void OnLevelLoaded(LevelLoading e)
    {
        // Check for currently loaded scenes that can stay loaded
            // If its already loaded, return
            // else add to scenes to load

        // Unload the previous unnecessary scenes
            // current scenes - stay_loaded scenes

        // Load the necessary scenes
            // total scenes - stay_loaded scenes

        // TODO: Refactor this to use some sort of bitmask or something. Right now just brute-forcing it

        // Add all the scenes from a single level to be loaded
        foreach (SceneField scene in dict[e.name])
        {
            _scenesToLoad.Add(SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive));
        }

        
        // Add these same scenes to be unloaded in the next call
        foreach (SceneField scene in dict[e.name])
        {
            _scenesToLoad.Add(SceneManager.UnloadSceneAsync(scene));
        }
    }

    void OnTriggerEnter(Collider other)
    {
        VariableConditionManager.Instance.Set("InElevator", "true");
    }

    void OnTriggerExit(Collider other)
    {
        VariableConditionManager.Instance.Set("InElevator", "false");
    }

    void OnEnable()
    {
        _levelLoading = new EventBinding<LevelLoading>(OnLevelLoaded);
        EventBus<LevelLoading>.Register(_levelLoading);
    }

    void OnDisable()
    {
        EventBus<LevelLoading>.DeRegister(_levelLoading);
    }
}
