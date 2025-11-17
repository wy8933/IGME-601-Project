using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnboardingManager : MonoBehaviour
{
    private GameObject _player;
    private bool _taskCompleted = false;
    public float updateCooldown;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");

        VariableConditionManager.Instance.Set("Trash", "0");
        GameVariables.Set("floor-cleaned", "false");
        GameVariables.Set("rule_broken_count:int", "1");
        GameVariables.Set("task_completed:int", "0");
        _player = GameObject.FindGameObjectWithTag("Player");

        GameVariables.Verbose = false;
        StartCoroutine(UpdateGame());

    }

    public void AllTaskCompleted()
    {
        // TODO: enable next button
        //SceneManager.LoadScene("GameOver");
        if (!_taskCompleted) 
        {
            VariableConditionManager.Instance.Set("task_completed:int", (int.Parse(VariableConditionManager.Instance.Get("task_completed:int")) + 1).ToString());
            EventBus<TasksComplete>.Raise(new TasksComplete { });
            _taskCompleted = true;
        }
        
    }

    private IEnumerator UpdateGame()
    {
        if (VariableConditionManager.Instance.Get("Trash") == "3")
        {
            AllTaskCompleted();
        }

        yield return new WaitForSeconds(updateCooldown);

        StartCoroutine(UpdateGame());
    }
}
