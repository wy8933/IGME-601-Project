using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using AudioSystem;

public class Handbook_UI : MonoBehaviour
{
    public List <Clue> cluesList;
    public List <Task> taskList;
    public GameObject[] pages;
    [SerializeField] private Transform taskContainer;
    [SerializeField] private Transform ruleContainer;
    [SerializeField] private GameObject taskPrefab;
    [SerializeField] private GameObject cluePrefab;
    [SerializeField] private PlayerController playerController;

    /// <summary>
    /// Global Add rule method that can be called when player gets a new task. 
    /// Will add the gameObject specified into the journal under the tasks page
    /// </summary>
    /// <param name="task"></param>
    public void AddTask(string description, string title)
    {
        GameObject taskObj = Instantiate(taskPrefab, taskContainer);
        Task task = taskObj.GetComponent<Task>();

        task.Description = description;
        task.Title = title;
        
        taskList.Add(task);
    }
    /// <summary>
    /// Global Add rule method that can be called when player gets a new task. 
    /// Will add the gameObject specified into the journal under the tasks page
    /// </summary>
    /// <param name="task"></param>
    public void AddClue(string description, string title)
    {
        GameObject clueObj = Instantiate(cluePrefab, ruleContainer);
        Clue clue = clueObj.GetComponent<Clue>();

        clue.Description = description;
        clue.Title = title;

        cluesList.Add(clue);
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

    public void Mute(Slider volumeSlider)
    {
        volumeSlider.value = 0;
    }
    
    /// <summary>
    /// Hides the journal and resumes play
    /// </summary>
    public void Resume()
    {
        playerController.ToggleHandbook();
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
    public void TestTask()
    {
        AddTask("KILL EVERYONE", "kill");
    }

    public void CompleteTest()
    {
        Debug.Log(taskList.Count + " tasks" + taskList[0].Description);
        taskList[0].CompleteTask();
        taskList.RemoveAt(0);
    }

    public void TestClue()
    {
        AddClue("KILL EVERYONE", "kill");
    }

    public void CompleteClue()
    {
        Debug.Log(cluesList.Count + " clues" + cluesList[0].Description);
        cluesList[0].gameObject.SetActive(false);
        cluesList.RemoveAt(0);
    }
    #endregion
}
