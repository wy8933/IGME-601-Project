using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Task : MonoBehaviour
{
    private string _description;
    private string _title;
    private bool _isComplete = false;
    private bool _isFirstTask = false;
    private bool _isLastTask = false;
    private Handbook_UI _handbook;

    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI completionText;
    [SerializeField] private GameObject leftArrow;
    [SerializeField] private GameObject rightArrow;

    /// <summary>
    /// Public property to get and set task description
    /// </summary>
    public string Description
    {
        get { return _description; }
        set { _description = value; }
    }

    /// <summary>
    /// Public property to get and set task title
    /// </summary>
    public string Title
    {
        get { return _title; }
        set { _title = value; }
    }

    /// <summary>
    /// Public property to get and set task completion status
    /// </summary>
    public bool IsComplete
    {
        get { return _isComplete; }
        set { _isComplete = value; }
    }

    /// <summary>
    /// Public property to get and set the arrow visibility
    /// True hides left arrow
    /// </summary>
    public bool IsFirstTask
    {
        get { return _isFirstTask; }
        set { _isFirstTask = value; }
    }

    /// <summary>
    /// Public property to get and set the arrow visibility
    /// True hides right arrow 
    /// </summary>
    public bool IsLastTask
    {
        get { return _isLastTask; }
        set { _isLastTask = value; }
    }

    /// <summary>
    /// Sets text to description
    /// </summary>
    private void Awake()
    {
        descriptionText.text = _description;
        _handbook = FindFirstObjectByType<Handbook_UI>();
    }

    private void OnEnable()
    {
        if (_handbook != null)
        {
            UpdatePage(_handbook.taskList);
        }
    }
    public void UpdatePage(List<Task> taskList)
    {
        if (_isFirstTask && taskList.Count == 1)
        {
            leftArrow.SetActive(false);
            rightArrow.SetActive(false);
        }
        if (_isFirstTask && taskList.Count > 1)
        {
            rightArrow.SetActive(true);
        }
        if (_isLastTask)
        {
            rightArrow.SetActive(false);
            leftArrow.SetActive(true);
        }
        if (!_isFirstTask && !_isLastTask)
        {
            leftArrow.SetActive(true);
            rightArrow.SetActive(true);
        }
    }
    public void CompleteTask()
    {
        completionText.text = "<color=green>Task Completed</color>";
    }

    /// <summary>
    /// Right arrow button
    /// </summary>
    public void RightArrow()
    {
        _handbook.NextTask();
    }

    /// <summary>
    /// Left arrow button method
    /// </summary>
    public void LeftArrow()
    {
        print("Left Arrow Pressed");

        _handbook.PreviousTask();
    }
}
