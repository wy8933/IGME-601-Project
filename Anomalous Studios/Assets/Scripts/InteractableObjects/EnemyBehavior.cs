using Unity.Behavior;
using UnityEngine;
using static RuleViolationSystem.DebugLogActionSO;
using System.Collections;
using UnityEngine.AI;

/// <summary>
/// TODO: Temporary broken rule event, should be refactored with rule event system
/// </summary>
public struct RuleBroken : IEvent { public bool isBroken; }

/// <summary>
/// A controller for the Rulekeeper's unique rules-dependent behaviors
/// </summary>
public class EnemyBehavior : MonoBehaviour, IInteractable
{
    private EventBinding<RuleBroken> _ruleBroken;
    private EventBinding<LevelLoading> _levelLoading;

    private BehaviorGraphAgent self;

    private Vector3 _spawnPoint = Vector3.zero;

    private bool _canInteract = true;
    public bool CanInteract { get => _canInteract; set => _canInteract = value; }

    public void Start()
    {
        self = GetComponent<BehaviorGraphAgent>();
        self.SetVariableValue("Player", 
            GameObject.FindGameObjectWithTag("Player"));
        _spawnPoint = new Vector3(-14, 2.5f, 0.0f); // TODO: Change to this position, just need to test other things 1st
    }

    public void Highlight()
    {

    }

    public void Interact()
    {
        // TODO: replace with textbox interaction, should be able to simply say 'hello,' or sign a paper
        GetComponent<Renderer>().material.color = Color.red;
    }

    /// <summary>
    /// Affects the Rulekeeper's decision making process. The Rulekeeper will eventually have different reactions to different rules outside of just "attack"
    /// </summary>
    private void OnRuleBroken(RuleBroken e)
    {
        self.SetVariableValue("ruleBroken", e.isBroken);
    }


    /// <summary>
    /// Brings the Rulekeepr back to spawn, resets their behaviors in between levels
    /// TODO: have this register as an event on level loading, then a coroutine 
    /// </summary>
    /// <param name="spawnPoint">Optionally update the Rulekeeper's spawn position</param>
    private void OnLevelLoaded(LevelLoading e)
    {
        self.enabled = false;
        GetComponent<NavMeshAgent>().enabled = false;
        self.SetVariableValue("ruleBroken", false);
        self.Restart();
        transform.position = _spawnPoint;

        StartCoroutine(EnableRuleKeeper(e));
    }

    private IEnumerator EnableRuleKeeper(LevelLoading e)
    {
        while (VariableConditionManager.Instance.Get("IsLevelLoading") == "false") { yield return null; }

        self.enabled = true;
        GetComponent<NavMeshAgent>().enabled = true;

    }

    public void OnEnable()
    {
        _ruleBroken = new EventBinding<RuleBroken>(OnRuleBroken);
        EventBus<RuleBroken>.Register(_ruleBroken);
        _levelLoading = new EventBinding<LevelLoading>(OnLevelLoaded);
        EventBus<LevelLoading>.Register(_levelLoading);
    }

    public void OnDisable()
    {
        EventBus<RuleBroken>.DeRegister(_ruleBroken);
        EventBus<LevelLoading>.DeRegister(_levelLoading);
    }

    public void RemoveHighlight()
    {
        throw new System.NotImplementedException();
    }

}
