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
    Task _currentTask;
    int _currentTaskId;
    Policy _currentPolicy;
    int _currentPolicyId;
    public GameObject[] pages;
    [SerializeField] private Transform _taskPage;
    [SerializeField] private Transform _policyPage;
    [SerializeField] private GameObject _taskPrefabLeft;
    [SerializeField] private GameObject _taskPrefabRight;
    [SerializeField] private GameObject _policyPrefabLeft;
    [SerializeField] private GameObject _policyPrefabRight;
    [SerializeField] private GameObject _popupPrefabPolicy;
    [SerializeField] private GameObject _popupPrefabTask;
    //[SerializeField] private PlayerController _playerController;
    PlayerController _playerController;

    private void Start()
    {
        _playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    }
    /// <summary>
    /// Global Add rule method that can be called when player gets a new task. 
    /// Will add the gameObject specified into the journal under the tasks page
    /// </summary>
    /// <param name="task"></param>
    public void AddTask(string description)
    {
        GameObject taskObj;
        Task task;
        if ((taskList.Count + 1 ) % 2 == 0)
        {
            taskObj = Instantiate(_taskPrefabLeft, _taskPage);
            task = taskObj.GetComponent<Task>();
            task.IsRightPage = false;
        }
        else
        {
            taskObj = Instantiate(_taskPrefabRight, _taskPage);
            task = taskObj.GetComponent<Task>();
            task.IsRightPage = true;
        }

        task.Description = $"{taskList.Count + 1}. {description}";

        // Lets user know that they picked up a new task
        GameObject popupText = Instantiate(_popupPrefabTask);
        popupText.transform.SetParent(transform.parent, false);

        // Adds task to list and sets it as current task
        taskList.Add(task);
        _currentTaskId = taskList.Count - 1;
        // Update current shown task to display this page
        UpdateTask(0);
    }

    /// <summary>
    /// Global Add rule method that can be called when player gets a new policy. 
    /// Will add the gameObject specified into the journal under the policies page
    /// </summary>
    /// <param name="policy"></param>
    public void AddPolicy(string description)
    {
        GameObject policyObj;
        if ((policiesList.Count + 1) % 2 == 0)
        {
            policyObj = Instantiate(_policyPrefabLeft, _policyPage);
        }
        else
        {
            policyObj = Instantiate(_policyPrefabRight, _policyPage);
        }
        Policy policy = policyObj.GetComponent<Policy>();
        policy.Description = $"{policiesList.Count+1}. {description}";

        // Lets user know that they picked up a new task
        GameObject popupText = Instantiate(_popupPrefabTask);
        popupText.transform.SetParent(transform.parent, false);

        // Adds policy to list and sets it as current policy
        policiesList.Add(policy);
        _currentPolicyId = policiesList.Count - 1;
        // Update current shown policy to display this page
        UpdatePolicy(0);
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
    /// Hides all other pages and updates their arrows as well as updating the current task
    /// </summary>
    /// <param name="value"></param>
    public void UpdateTask(int value)
    {
        foreach (Task task in taskList)
        {
            task.gameObject.SetActive(false);
        }
        
        // Adds 1, 0, or -2 to change task to next, added, or previous in list.
        _currentTaskId += value;

        // Catches edge cases of changing pages
        if (_currentTaskId >= taskList.Count)
        {
            _currentTaskId = taskList.Count - 1;
        }
        if(_currentTaskId < 0)
        {
            _currentTaskId = 0;
        }
        _currentTask = taskList[_currentTaskId];
        
        // Turns on the game object
        _currentTask.gameObject.SetActive(true);

        // Right page turns on left page unless it is first page
        if (_currentTask.IsRightPage && _currentTaskId > 1)
        {
            taskList[_currentTaskId - 1].gameObject.SetActive(true);
            if(_currentTaskId + 1 < taskList.Count)
            {
                _currentTask.Arrow.SetActive(true);
            }
        }

        // Left page turns on right page if it exists
        else if (_currentTaskId >= 1 && _currentTaskId+ 1 < taskList.Count) 
        {
            taskList[_currentTaskId + 1].gameObject.SetActive(true);
            _currentTaskId += 1;
            if (_currentTaskId + 1 < taskList.Count)
            {
                taskList[_currentTaskId].Arrow.SetActive(true);
            }
        }

        // First page gets the arrow if there is another task in the list
        else if (taskList.Count >= 2) 
        { 
            _currentTask.Arrow.SetActive(true);
        }
    }

    /// <summary>
    /// Hides all other pages and updates their arrows as well as updating the current policy
    /// </summary>
    /// <param name="value"></param>
    public void UpdatePolicy(int value)
    {
        foreach (Policy policy in policiesList)
        {
            policy.gameObject.SetActive(false);
        }

        // Adds 1, 0, or -2 to change task to next, added, or previous in list.
        _currentPolicyId += value;

        // Catches edge cases of changing pages
        if (_currentPolicyId >= policiesList.Count)
        {
            _currentPolicyId = policiesList.Count - 1;
        }
        if (_currentPolicyId < 0)
        {
            _currentPolicyId = 0;
        }
        _currentPolicy = policiesList[_currentPolicyId];

        // Turns on the game object
        _currentPolicy.gameObject.SetActive(true);

        // Right page turns on left page unless it is first page
        if (_currentPolicy.IsRightPage && _currentPolicyId > 1)
        {
            policiesList[_currentPolicyId - 1].gameObject.SetActive(true);
            if (_currentPolicyId + 1 < policiesList.Count)
            {
                _currentPolicy.Arrow.SetActive(true);
            }
        }

        // Left page turns on right page if it exists
        else if (_currentPolicyId >= 1 && _currentPolicyId + 1 < policiesList.Count)
        {
            policiesList[_currentPolicyId + 1].gameObject.SetActive(true);
            _currentPolicyId += 1;
            if (_currentPolicyId + 1 < policiesList.Count)
            {
                policiesList[_currentPolicyId].Arrow.SetActive(true);
            }
        }

        // First page gets the arrow if there is another task in the list
        else if (policiesList.Count >= 2)
        {
            _currentPolicy.Arrow.SetActive(true);
        }
    }
    /// <summary>
    /// Hides the journal and resumes play
    /// </summary>
    public void Resume()
    {
        _playerController.GetPlayerJournal().ToggleHandbook();
        // Resume Logic
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
