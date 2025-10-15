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
    //public Light[] Lights => lights;
    public Light[] Lights
    {
        get 
        {
            if (lights == null) Debug.LogWarning("LightManager: Called too early, wait for lights to references");
                
            return lights;
        }
    }

    void Start()
    {
        //ListLights();
    }

    private void ListLights()
    {
        //Relist light objects
        lights = FindObjectsByType<Light>(FindObjectsSortMode.None);
    }

    public void OnEnable()
    {
        _levelLoaded = new EventBinding<LevelLoaded>(ListLights);
        EventBus<LevelLoaded>.Register(_levelLoaded);
    }

    public void OnDisable()
    {
        EventBus<LevelLoaded>.DeRegister(_levelLoaded);
    }
}
