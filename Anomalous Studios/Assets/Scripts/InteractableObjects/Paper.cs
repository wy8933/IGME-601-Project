    using TMPro;
using UnityEngine;

public class Paper : MonoBehaviour, IInteractable
{
    [SerializeField] private Level _level;
    [SerializeField] private TextMeshProUGUI _description;
    [SerializeField] private Handbook_UI _handbook = null;
    [SerializeField] private float _holdTime = 0.0f;

    private Renderer _renderer;
    public bool isTask;

    // TODO: Change the initialization of the first level to be dynamic, raise an event
    // The level system is going to change pretty soon to accomadate new level box anyway
    private static Level _currentLevel = Level.blue;
    private bool _canInteract = true;
    public float HoldTime { get => _holdTime; }
    public bool CanInteract { get => _canInteract; set => _canInteract = value; }

    /// <summary>
    /// Called on first active frame
    /// </summary>
    public void Start()
    {
        _renderer = GetComponent<Renderer>();
    }

    /// <summary>
    /// TODO: Will be implemented once shader is create
    /// </summary>
    public void Highlight()
    {
        // TODO: Replace with shader to highlight the item, or UI element to indicate it is interactable
        //print("Highlighting Paper");
        //_center = gameObject.transform.position;
    }

    /// <summary>
    /// Leverages interact from interaction class
    /// </summary>
    public void Interact()
    {
        this.gameObject.SetActive(false);
        AddToHandbook();
    }

    /// <summary>
    /// Adds the rule or task to its respective page
    /// </summary>
    public void AddToHandbook()
    {
        if (isTask)
        {
            _handbook.AddTask(_description.text, "Task1");
        }
        else
        {
            _handbook.AddPolicy(_description.text, "Rule1");
        }
    }

    public void OnDrawGizmos()
    {
        //Gizmos.DrawWireSphere(_center, 1);
    }

    public void RemoveHighlight()
    {
      
    }
}
