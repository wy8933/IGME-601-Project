using System.Collections;
using System.Collections.Generic;
using AudioSystem;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Handbook_UI : MonoBehaviour
{
    public List <Policy> policiesList;
    public List <Task> taskList;
    public GameObject[] pages;
    [SerializeField] private Transform _taskContainer;
    [SerializeField] private Transform _ruleContainer;
    [SerializeField] private GameObject _taskPrefab;
    [SerializeField] private GameObject _policyPrefab;
    [SerializeField] private GameObject _popupPrefabPolicy;
    [SerializeField] private GameObject _popupPrefabTask;
    [SerializeField] private PlayerController _playerController;

    /// <summary>
    /// Global Add rule method that can be called when player gets a new task. 
    /// Will add the gameObject specified into the journal under the tasks page
    /// </summary>
    /// <param name="task"></param>
    public void AddTask(string description, string title)
    {
        GameObject taskObj = Instantiate(_taskPrefab, _taskContainer);
        Task task = taskObj.GetComponent<Task>();
        
        task.Description = $"{taskList.Count + 1}. {description}";
        task.Title = title;

        GameObject popupText = Instantiate(_popupPrefabTask);
        popupText.transform.SetParent(transform.parent, false);
        taskList.Add(task);
    }

    /// <summary>
    /// Global Add rule method that can be called when player gets a new task. 
    /// Will add the gameObject specified into the journal under the tasks page
    /// </summary>
    /// <param name="task"></param>
    public void AddPolicy(string description, string title)
    {
        GameObject policyObj = Instantiate(_policyPrefab, _ruleContainer);
        Policy policy = policyObj.GetComponent<Policy>();

        policy.Description = $"{policiesList.Count+1}. {description}";
        policy.Title = title;

        policiesList.Add(policy);

        GameObject popupText = Instantiate(_popupPrefabPolicy);
        popupText.transform.SetParent(transform.parent, false);
    }

    #region Button Methods
    /// <summary>
    /// Used for tabs to open the passed in page and closes all other pages effectively swapping the contents of the journal. 
    /// </summary>
    /// <param name="page"></param>
    public void OpenPage(GameObject page)
    {
        for (int i = 0; i < pages.Length; i++)
        {
            if (pages[i] == page)
            {
                pages[i].SetActive(true);
            }
            else
            {
                pages[i].SetActive(false);
            }
        }
    }

    /// <summary>
    /// Arrow method to increase slider by 20% of sliders max value
    /// </summary>
    /// <param name="slider"></param>
    public void SliderArrowUp(Slider slider)
    {
        float amount = slider.maxValue/20;
        slider.value += amount;  
    }

    /// <summary>
    /// Arrow method to decrease slider by 20% of sliders max value
    /// </summary>
    /// <param name="slider"></param>
    public void SliderArrowDown(Slider slider)
    {
        float amount = slider.maxValue / 20;
        slider.value -= amount;
    }

    /// <summary>
    /// Sets the volume slider to 0
    /// </summary>
    /// <param name="volumeSlider"></param>
    public void Mute(Slider volumeSlider)
    {
        volumeSlider.value = 0;
    }
    
    /// <summary>
    /// Hides the journal and resumes play
    /// </summary>
    public void Resume()
    {
        _playerController.ToggleHandbook();
        // Resume Logic
    }

    /// <summary>
    /// Loads the Main Menu scene
    /// </summary>
    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// Checks if the game is playing in editor or build and then closes the game.
    /// </summary>
    public void QuitGame()
    {
    #if UNITY_EDITOR
        EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }
    #endregion

    

    #region Test
    // Testing
    //public void TestTask()
    //{
    //    AddTask("KILL EVERYONE", "kill");
    //}

    //public void CompleteTest()
    //{
    //    Debug.Log(taskList.Count + " tasks" + taskList[0].Description);
    //    taskList[0].CompleteTask();
    //    taskList.RemoveAt(0);
    //}

    //public void TestClue()
    //{
    //    AddClue("KILL EVERYONE", "kill");
    //}

    //public void CompleteClue()
    //{
    //    Debug.Log(cluesList.Count + " clues" + cluesList[0].Description);
    //    cluesList[0].gameObject.SetActive(false);
    //    cluesList.RemoveAt(0);
    //}
    #endregion
}
