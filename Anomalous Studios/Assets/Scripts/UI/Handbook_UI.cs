using System.Collections.Generic;
using UnityEngine;

public class Handbook_UI : MonoBehaviour
{
    public List <Policy> policiesList;
    public List <Task> taskList;
    Task _currentTask;
    int _currentTaskId;
    Policy _currentPolicy;
    int _currentPolicyId;
    public GameObject[] pages;
    [SerializeField] private Transform _taskContainer;
    [SerializeField] private Transform _ruleContainer;
    [SerializeField] private GameObject _taskPrefab;
    [SerializeField] private GameObject _policyPrefab;
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
    public void AddTask(string description, string title)
    {
        GameObject taskObj = Instantiate(_taskPrefab, _taskContainer);
        Task task = taskObj.GetComponent<Task>();
        
        task.Description = $"{taskList.Count + 1}. {description}";
        task.Title = title;

        // Checks position in list
        if (taskList.Count == 0)
        {
            task.IsFirstTask = true;
        }
        else
        {
            task.IsLastTask = true;
            // Turns previously last task to false.
            if(taskList.Count > 1)
            {
                taskList[taskList.Count - 1].IsLastTask = false;
            }
        }
        
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
    /// Global Add rule method that can be called when player gets a new task. 
    /// Will add the gameObject specified into the journal under the tasks page
    /// </summary>
    /// <param name="policy"></param>
    public void AddPolicy(string description, string title)
    {
        GameObject policyObj = Instantiate(_policyPrefab, _ruleContainer);
        Policy policy = policyObj.GetComponent<Policy>();

        policy.Description = $"{policiesList.Count+1}. {description}";
        policy.Title = title;

        // Checks position in list
        if (policiesList.Count == 0)
        {
            policy.IsFirstPolicy = true;
        }
        else
        {
            policy.IsLastPolicy = true;
            // Turns previously last task to false.
            if (policiesList.Count > 1)
            {
                policiesList[policiesList.Count - 1].IsLastPolicy = false;
            }
        }

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
            task.UpdatePage(taskList);
        }

        // Adds 1, 0, or -1 to change task to next, added, or previous in list.
        _currentTaskId += value;
        _currentTask = taskList[_currentTaskId];
        _currentTask.gameObject.SetActive(true);
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
            policy.UpdatePage(policiesList);
        }

        // Adds 1, 0, or -1 to change task to next, added, or previous in list.
        _currentPolicyId += value;
        _currentPolicy = policiesList[_currentPolicyId];
        _currentPolicy.gameObject.SetActive(true);
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
