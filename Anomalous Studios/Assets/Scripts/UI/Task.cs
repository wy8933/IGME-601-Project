using TMPro;
using UnityEngine;

public class Task : MonoBehaviour
{
    private string _description;
    private string _title;
    private bool _isComplete;

    [SerializeField] private TextMeshProUGUI descriptionText;

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
    /// Sets text to description
    /// </summary>
    private void Start()
    {
        descriptionText.text = _description;
    }

    public void CompleteTask()
    {
        
        descriptionText.fontStyle = FontStyles.Strikethrough;
        
    }
}
