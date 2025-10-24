using UnityEngine;

public class LightManager : MonoBehaviour
{
    public static LightManager Instance;

    private EventBinding<LevelLoaded> _levelLoaded;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else 
        {
            Destroy(this);
        }
    }
    private Light[] lights; //List of active lights
    public Light[] Lights => lights;

    void Start()
    {
        ListLights();
    }

    /// <summary>
    /// Finds and stores all Light components in the scene.
    /// </summary>
    private void ListLights()
    {
        //Relist light objects
        lights = FindObjectsByType<Light>(FindObjectsSortMode.None);
    }


    /// <summary>
    /// Subscribes to the LevelLoaded event to refresh the light list.
    /// </summary>
    public void OnEnable()
    {
        _levelLoaded = new EventBinding<LevelLoaded>(ListLights);
        EventBus<LevelLoaded>.Register(_levelLoaded);
    }

    /// <summary>
    /// Unsubscribes from the LevelLoaded event.
    /// </summary>
    public void OnDisable()
    {
        EventBus<LevelLoaded>.DeRegister(_levelLoaded);
    }
}
