using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FirstFloorTempManager : MonoBehaviour
{
    [SerializeField]private GameObject _fistLevelDoor;
    private GameObject _player;
    public float updateCooldown;

    public void Start()
    {
        VariableConditionManager.Instance.Set("Trash","0");
        _player = GameObject.FindGameObjectWithTag("Player");

        GameVariables.Verbose = false;
        StartCoroutine(UpdateGame());
    }

    public void AllTaskCompleted() 
    {
        Destroy(_fistLevelDoor);
    }

    private void OnTriggerEnter(Collider other)
    {
        SceneManager.LoadScene("GameOver");
    }

    private IEnumerator UpdateGame() 
    {
        if (VariableConditionManager.Instance.Get("Trash") == "3")
        {
            AllTaskCompleted();
        }

        //Time calculation
        float currentTime = float.Parse(VariableConditionManager.Instance.Get("watchTimer"));

        if (currentTime > 15 && currentTime < 180)
        {
            VariableConditionManager.Instance.Set("WrongTime", "true");
        }

        // Light Calculation
        float lightValue = _player.GetComponent<LightDetection>().lightTotal;
        GameVariables.Set("player-light-value:float", lightValue.ToString());

        yield return new WaitForSeconds(updateCooldown);

        StartCoroutine(UpdateGame());
    }
}
