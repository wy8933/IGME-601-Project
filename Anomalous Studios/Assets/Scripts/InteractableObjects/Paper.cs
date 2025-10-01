using TMPro;
using UnityEngine;

public class Paper : Interaction
{
    [SerializeField] private Level _level;
    [SerializeField] private TextMeshProUGUI _description;
    [SerializeField] private Handbook_UI _handbook = null;
    private Renderer _renderer;

    // TODO: Change the initialization of the first level to be dynamic, raise an event
    // The level system is going to change pretty soon to accomadate new level box anyway
    private static Level _currentLevel = Level.blue;

    public void Start()
    {
        _renderer = GetComponent<Renderer>();
    }

    public override void Update()
    {
        base.Update();
        if (_currentLevel == _level)
        {
            _renderer.material.color = Color.white;
        }

        else
        {
            _renderer.material.color = Color.black;
        }

    }

    public override void Highlight()
    {
        // TODO: Replace with shader to highlight the item, or UI element to indicate it is interactable
        print("Highlighting Paper");
    }

    protected override void Interact()
    {
        print("interacted");
        this.gameObject.SetActive(false);
        AddToHandbook();
    }

    public void AddToHandbook()
    {
        _handbook.AddClue(_description.text, "Rule1");
    }
}
