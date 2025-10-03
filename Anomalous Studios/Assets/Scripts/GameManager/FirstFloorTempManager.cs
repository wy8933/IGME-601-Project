using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FirstFloorTempManager : MonoBehaviour
{
    [SerializeField]private GameObject _fistLevelDoor;

    public void Start()
    {
        VariableConditionManager.Instance.Set("Trash","0");
    }

    public void Update()
    {
        if (VariableConditionManager.Instance.Get("Trash") == "3") 
        {
            AllTaskCompleted();
        }
    }

    public void AllTaskCompleted() 
    {
        Destroy(_fistLevelDoor);
    }

    private void OnTriggerEnter(Collider other)
    {
        SceneManager.LoadScene("GameOver");
    }
}
