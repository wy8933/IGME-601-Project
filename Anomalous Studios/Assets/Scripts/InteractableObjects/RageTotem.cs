using Unity.Behavior;
using UnityEngine;

/// <summary>
/// Test class to break the Rulekeeper's rules
/// </summary>
public class RageTotem : Interaction
{
    [SerializeField] private BehaviorGraphAgent _rulekeeper;

    // Update is called once per frame
    public override void Update()
    {
        base.Update();  
    }
    public override void Highlight()
    {
        // Highlight!
    }

    protected override void Interact()
    {
        _rulekeeper.SetVariableValue("ruleBroken", true);
    }
}
