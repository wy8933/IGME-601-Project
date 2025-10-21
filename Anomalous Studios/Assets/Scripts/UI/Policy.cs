using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Policy : MonoBehaviour
{
    private string _description;
    private string _title;
    private bool _isBroken;
    private bool _isFirstPolicy = false;
    private bool _isLastPolicy = false;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private GameObject _leftArrow;
    [SerializeField] private GameObject _rightArrow;
    private Handbook_UI _handbook = null;

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
    /// Public property to get and set the arrow visibility
    /// True hides left arrow
    /// </summary>
    public bool IsFirstPolicy
    {
        get { return _isFirstPolicy; }
        set { _isFirstPolicy = value; }
    }

    /// <summary>
    /// Public property to get and set the arrow visibility
    /// True hides right arrow 
    /// </summary>
    public bool IsLastPolicy
    {
        get { return _isLastPolicy; }
        set { _isLastPolicy = value; }
    }
    /// <summary>
    /// Sets text to description
    /// </summary>
    private void Awake()
    {
   
        descriptionText.text = _description;
        _handbook = FindFirstObjectByType<Handbook_UI>();
    }

    /// <summary>
    /// Updates the arrows on the page based on its position and size of the list
    /// </summary>
    /// <param name="policiesList"></param>
    public void UpdatePage(List<Policy> policiesList)
    {
        if (_isFirstPolicy && policiesList.Count == 1)
        {
            _leftArrow.SetActive(false);
            _rightArrow.SetActive(false);
        }
        if (_isFirstPolicy && policiesList.Count > 1)
        {
            _rightArrow.SetActive(true);
        }
        if (_isLastPolicy)
        {
            _rightArrow.SetActive(false);
            _leftArrow.SetActive(true);
        }
        if (!_isFirstPolicy && !_isLastPolicy)
        {
            _leftArrow.SetActive(true);
            _rightArrow.SetActive(true);
        }
    }

    /// <summary>
    /// Right arrow button
    /// </summary>
    public void RightArrow()
    {
        _handbook.UpdatePolicy(1);
    }

    /// <summary>
    /// Left arrow button method
    /// </summary>
    public void LeftArrow()
    {
        _handbook.UpdatePolicy(-1);
    }
}
