using UnityEngine;

/// <summary>
/// A test-case for the interactable interface
/// </summary>
public class Button : Interaction
{
    [SerializeField] private Level _level;

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
    }

    protected override void Interact()
    {
        // TODO: Check to see if which levels are available to the player as yet, some sort of static condition for all elevator buttons
        if (_currentLevel != _level &&
            VariableConditionManager.Instance.Get("InElevator").Equals("true") &&
            VariableConditionManager.Instance.Get("TaskComplete").Equals("true") &&
            VariableConditionManager.Instance.Get("IsLevelLoading").Equals("false"))
        {            
            EventBus<LevelLoading>.Raise(new LevelLoading { newLevel = _level });
            VariableConditionManager.Instance.Set("IsLevelLoading", "true");
            _currentLevel = _level;
        }

    }
}
