using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Task : MonoBehaviour
{
    private string _description;
    private string _title;
    private bool _isComplete = false;
    private bool _isRightPage = false;
    private Handbook_UI _handbook;

    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private TextMeshProUGUI _completionText;
    [SerializeField] private GameObject _arrow;

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

    public bool IsRightPage
    {
        get { return _isRightPage; }
        set { _isRightPage = value; }
    }
    public GameObject Arrow
    {
        get { return _arrow; }
        set { _arrow = value; }
    }
    /// <summary>
    /// Sets text to description
    /// </summary>
    private void Awake()
    {
        _descriptionText.text = _description;
        _handbook = FindFirstObjectByType<Handbook_UI>();
    }

    ///// <summary>
    ///// Updates the arrows on each page when the list is altered 
    ///// </summary>
    ///// <param name="taskList"></param>
    //public void UpdatePage(List<Task> taskList)
    //{
    //    if (_isFirstTask && taskList.Count == 1)
    //    {
    //        _leftArrow.SetActive(false);
    //        _rightArrow.SetActive(false);
    //    }
    //    if (_isFirstTask && taskList.Count > 1)
    //    {
    //        _rightArrow.SetActive(true);
    //    }
    //    if (_isLastTask)
    //    {
    //        _rightArrow.SetActive(false);
    //        _leftArrow.SetActive(true);
    //    }
    //    if (!_isFirstTask && !_isLastTask)
    //    {
    //        _leftArrow.SetActive(true);
    //        _rightArrow.SetActive(true);
    //    }
    //}
    public void CompleteTask()
    {
        _completionText.text = "<color=green>Task Completed</color>";
    }

    /// <summary>
    /// Right arrow button
    /// </summary>
    public void RightArrowClicked()
    {
        _handbook.UpdateTask(2);
    }

    /// <summary>
    /// Left arrow button method
    /// </summary>
    public void LeftArrowClicked()
    {
        _handbook.UpdateTask(-2);

    }
}
