using TMPro;
using UnityEngine;

public class Policy : MonoBehaviour
{
    private string _description;
    private string _title;
    private bool _isBroken;

    [SerializeField] private TextMeshProUGUI descriptionText;

    /// <summary>
    /// Public property to get and set policy description
    /// </summary>
    public string Description
    {
        get { return _description; }
        set { _description = value; }
    }
    /// <summary>
    /// Public property to get and set policy title
    /// </summary>
    public string Title
    {
        get { return _title; }
        set { _title = value; }
    }

    /// <summary>
    /// Public propert to get and set if policy is broken
    /// </summary>
    public bool IsBroken
    {
        get { return _isBroken; }
        set { _isBroken = value; }
    }
    /// <summary>
    /// Sets text to description
    /// </summary>
    private void Start()
    {
        descriptionText.text = _description;
    }
}
