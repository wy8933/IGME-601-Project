using Unity.Behavior;
using UnityEngine;
using static RuleViolationSystem.DebugLogActionSO;

/// <summary>
/// TODO: Temporary broken rule event, should be refactored with rule event system
/// </summary>
public struct RuleBroken : IEvent { public bool isBroken; }

/// <summary>
/// A controller for the Rulekeeper's unique rules-dependent behaviors
/// </summary>
public class EnemyBehavior : Interaction
{
    private EventBinding<RuleBroken> _ruleBroken;

    private BehaviorGraphAgent self;

    void Start()
    {
        self = GetComponent<BehaviorGraphAgent>();
    }

    public override void Update()
    {
        base.Update();
    }
    public override void Highlight()
    {
        
    }

    protected override void Interact()
    {
        EventBus<RuleBroken>.Raise(new RuleBroken { isBroken = true });
    }

    /// <summary>
    /// Affects the Rulekeeper's decision making process. The Rulekeeper will eventually have different reactions to different rules outside of just "attack"
    /// </summary>
    private void OnRuleBroken(RuleBroken e)
    {
        self.SetVariableValue("ruleBroken", e.isBroken);
    }

    public void OnEnable()
    {
        _ruleBroken = new EventBinding<RuleBroken>(OnRuleBroken);
        EventBus<RuleBroken>.Register(_ruleBroken);
    }

    public void OnDisable()
    {
        EventBus<RuleBroken>.DeRegister(_ruleBroken);
    }


}
