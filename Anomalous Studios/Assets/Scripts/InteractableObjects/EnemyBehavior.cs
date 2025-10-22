using Unity.Behavior;
using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using AudioSystem;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// If a rule is broken or resolved, passes the type of affected rule to the Rulekeeper
/// </summary>
public struct RuleBroken : IEvent { public bool isBroken; }

/// <summary>
/// A controller for the Rulekeeper's unique rules-dependent behaviors
/// </summary>
public class EnemyBehavior : MonoBehaviour, IInteractable
{
    private EventBinding<RuleBroken> _ruleBroken;
    // TODO: refactor reset of scene
    private EventBinding<LevelLoaded> _levelLoading;

    private BehaviorGraphAgent self;

    // TODO: refactor reset of scene
    private Vector3 _spawnPoint = Vector3.zero;

    private bool _canInteract = true;

    public float HoldTime { get => 0.0f; }
    public bool CanInteract { get => _canInteract; set => _canInteract = value; }

    [Header("Reaction SFX")]
    [SerializeField] private SoundDataSO _failedSFX;
    [SerializeField] private SoundDataSO _successSFX;
    public SoundDataSO InitialSFX => null;
    public SoundDataSO FailedSFX { get => _failedSFX; }
    public SoundDataSO CancelSFX => null;
    public SoundDataSO SuccessSFX { get => _successSFX; }

    // TODO: change to an enum value instead of string names for rules
    private Dictionary<string, bool> _rulesLibrary = new Dictionary<string, bool>
        {
            { "lights", false },
            { "camera", false },
            { "action!", false }
        };

    public void Start()
    {
        self = GetComponent<BehaviorGraphAgent>();
        self.SetVariableValue("Player", 
            GameObject.FindGameObjectWithTag("Player"));

        // TODO: refactor reset of scene
        _spawnPoint = new Vector3(-14, 2.5f, 0.0f);

        EventBus<RuleBroken>.Raise(new RuleBroken { isBroken = true });
    }

    public void Highlight()
    {
        GetComponent<HighlightTarget>().IsHighlighted = true;
    }
    public void RemoveHighlight()
    {
        GetComponent<HighlightTarget>().IsHighlighted = false;
    }

    public void Interact()
    {
        // TODO: replace with textbox interaction, should be able to simply say 'hello,' or sign a paper
    }

    /// <summary>
    /// Affects the Rulekeeper's decision making process. The Rulekeeper will eventually have different reactions to different rules outside of just "attack"
    /// </summary>
    private void OnRuleBroken(RuleBroken e)
    {
        _rulesLibrary["lights"] = e.isBroken;

        // One giant OR statement of dictionary values
        self.SetVariableValue("ruleBroken", _rulesLibrary.Values.Any(value => value));
    }


    /// <summary>
    /// Brings the Rulekeepr back to spawn, resets their behaviors in between levels
    /// </summary>
    private void OnLevelLoaded(LevelLoaded e)
    {
        self.enabled = false;
        GetComponent<NavMeshAgent>().enabled = false;
        self.SetVariableValue("ruleBroken", false);
        self.Restart();
        transform.position = _spawnPoint;

        StartCoroutine(EnableRuleKeeper(e));

        // TODO: set up the list of rules with a new dataset
    }

    private IEnumerator EnableRuleKeeper(LevelLoaded e)
    {
        while (VariableConditionManager.Instance.Get("IsLevelLoading") == "false") { yield return null; }

        self.enabled = true;
        GetComponent<NavMeshAgent>().enabled = true;

    }

    public void OnEnable()
    {
        _ruleBroken = new EventBinding<RuleBroken>(OnRuleBroken);
        EventBus<RuleBroken>.Register(_ruleBroken);
        _levelLoading = new EventBinding<LevelLoaded>(OnLevelLoaded);
        EventBus<LevelLoaded>.Register(_levelLoading);
    }

    public void OnDisable()
    {
        EventBus<RuleBroken>.DeRegister(_ruleBroken);
        EventBus<LevelLoaded>.DeRegister(_levelLoading);
    }


}
