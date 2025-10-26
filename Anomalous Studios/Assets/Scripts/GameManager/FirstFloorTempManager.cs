using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FirstFloorTempManager : MonoBehaviour
{
    private GameObject _player;
    public float updateCooldown;

    [SerializeField] private GameObject[] _bedRoomLights;
    [SerializeField] private GameObject[] _allLights;

    public float powerOutTime;
    public float powerResetTime;
    public float powerOutCheckTime;
    public bool isPowerOut;
    public void Start()
    {
        VariableConditionManager.Instance.Set("Trash","0");
        _player = GameObject.FindGameObjectWithTag("Player");

        GameVariables.Verbose = false;
        StartCoroutine(UpdateGame());
    }

    public void AllTaskCompleted() 
    {

    }

    private IEnumerator UpdateGame() 
    {
        // TODO: add the check for the cleaning
        if (VariableConditionManager.Instance.Get("Trash") == "3")
        {
            AllTaskCompleted();
        }

        //Time calculation
        float currentTime = float.Parse(VariableConditionManager.Instance.Get("watchTimer"));

        // Set the bedroom lights
        if (currentTime % 60 <= 1) 
        {
            if (currentTime / 60 % 2 == 0)
            {
                foreach (GameObject obj in _bedRoomLights)
                {
                    obj.SetActive(true);
                }
            }
            else 
            {
                foreach (GameObject obj in _bedRoomLights)
                {
                    obj.SetActive(false);
                }
            }
        }

        if (!isPowerOut)
        {
            if (currentTime % powerOutCheckTime <= 1)
            {
                int num = Random.Range(0, 10);

                if (num < 5) 
                {
                    isPowerOut = true;
                    powerOutTime = currentTime;
                    foreach (GameObject lights in _allLights)
                    {
                        lights.SetActive(false);
                    }
                }
            }
        }
        else 
        {
            if (currentTime - powerOutTime >= powerResetTime) 
            {
                isPowerOut = false;

                foreach (GameObject lights in _allLights)
                {
                    lights.SetActive(true);
                }
            }
        }


        // Light Calculation
        float lightValue = _player.GetComponent<LightDetection>().lightTotal;
        GameVariables.Set("player-light-value:float", lightValue.ToString());

        yield return new WaitForSeconds(updateCooldown);

        StartCoroutine(UpdateGame());
    }
}
