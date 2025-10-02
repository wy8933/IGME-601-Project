using UnityEngine;

public class LightManager : MonoBehaviour
{
    public static LightManager Instance;

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

    private void ListLights()
    {
        //Relist light objects
        lights = FindObjectsByType<Light>(FindObjectsSortMode.None);
    }
}
