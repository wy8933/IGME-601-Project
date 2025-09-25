using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Level
{
    blue,
    red
}

public struct LevelLoading : IEvent { public Level newLevel; }

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private SceneField[] _blueLevel;
    [SerializeField] private SceneField[] _redLevel;

    private Level _lastLevel;

    private Dictionary<Level, SceneField[]> dict;

    private List<AsyncOperation> _scenesToLoad = new List<AsyncOperation>();

    private EventBinding<LevelLoading> _levelLoading;

    private Animator _animator;

    public void Start()
    {
        dict = new Dictionary<Level, SceneField[]>
        {
            { Level.blue, _blueLevel },
            { Level.red, _redLevel }
        };

        // Initializes the starting level
        _lastLevel = Level.blue;

        foreach (SceneField scene in dict[Level.blue])
        {
            SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
        }


        // Assumes the player spawn in the Elevator to start
        VariableConditionManager.Instance.Set("TaskComplete", "true");

        _animator = GetComponent<Animator>();
        _animator.SetTrigger("moveDoors");
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

    private IEnumerator LoadScenes(LevelLoading e)
    {
        // TODO: Refactor this to use some sort of bitmask or something. Right now just brute-forcing it

        // Start unloading scenes when elevator door is fully closed
        while (_animator.GetBool("isOpen")) { yield return null; }

        // Unload the current floor, and load in the new floor
        foreach (SceneField scene in dict[_lastLevel])
        {
            _scenesToLoad.Add(SceneManager.UnloadSceneAsync(scene));
        }

        foreach (SceneField scene in dict[e.newLevel])
        {
            _scenesToLoad.Add(SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive));
        }

        _lastLevel = e.newLevel;

        // Wait for the last operation before opening the doors
        while (!_scenesToLoad[_scenesToLoad.Count - 1].isDone) { yield return null; }

        _scenesToLoad.Clear();

        _animator.SetTrigger("moveDoors");
    }

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
