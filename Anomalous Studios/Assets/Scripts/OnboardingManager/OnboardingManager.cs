using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnboardingManager : MonoBehaviour
{
    private GameObject _player;

    public float updateCooldown;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");

        VariableConditionManager.Instance.Set("Trash", "0");
        GameVariables.Set("floor-cleaned", "false");
        _player = GameObject.FindGameObjectWithTag("Player");

        GameVariables.Verbose = false;
        StartCoroutine(UpdateGame());

    }

    public void AllTaskCompleted()
    {
        // TODO: enable next button
        SceneManager.LoadScene("GameOver");
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
