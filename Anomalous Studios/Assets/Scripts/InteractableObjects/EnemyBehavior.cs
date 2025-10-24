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
public struct RuleBroken : IEvent 
{ 
    public bool isBroken;
    public Vector3 target;
}

public struct MakeNoise : IEvent { public Vector3 target; }


/// <summary>
/// A controller for the Rulekeeper's unique rules-dependent behaviors
/// </summary>
public class EnemyBehavior : MonoBehaviour, IInteractable
{
    private EventBinding<RuleBroken> _ruleBroken;
    private EventBinding<LevelLoaded> _levelLoaded;
    private EventBinding<LoadLevel> _loadLevel;

    private BehaviorGraphAgent _behaviorAgent;
    private NavMeshAgent _navAgent;

    private float _walkSpeed = 5.0f;

    private bool _canInteract = true;

    public float WalkSpeed { get => _walkSpeed; }

    public float HoldTime { get => 0.0f; }
    public bool CanInteract { get => _canInteract; set => _canInteract = value; }

    public float Speed
    {
        get { _behaviorAgent.GetVariable("Speed", out BlackboardVariable<float> speed); return speed; }

        set => _behaviorAgent.SetVariableValue("Speed", value);
    }

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

    private Vector3[] _sightCone; 

    public void Start()
    {
        _behaviorAgent = GetComponent<BehaviorGraphAgent>();
        _navAgent = GetComponent<NavMeshAgent>();
        _behaviorAgent.SetVariableValue("Player", 
            GameObject.FindGameObjectWithTag("Player"));
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

    public void CheckLineOfSight(Transform target)
    {
        // create a direction arrow towards the player
        
        // If that arrow is within the arc of enemy vision

        // TODO: make a bunch of rays or direction vectors before hand, raycast all of them

        // If target is not already seen, I-C-U SFX
        if (Physics.Raycast(transform.position, target.position - transform.position, 
            out RaycastHit hit) && hit.collider.CompareTag("Player"))
        {
            _behaviorAgent.SetVariableValue("playerSeen", true);
        }
        else
        {
            _behaviorAgent.SetVariableValue("playerSeen", false);

        }
    }

    /// <summary>
    /// Affects the Rulekeeper's decision making process. The Rulekeeper will eventually have different reactions to different rules outside of just "attack"
    /// </summary>
    private void OnRuleBroken(RuleBroken e)
    {
        _rulesLibrary["lights"] = e.isBroken;

        _behaviorAgent.GetVariable("ruleBroken", out BlackboardVariable<bool> ruleBroken);

        if (!ruleBroken.Value)
        {
            _behaviorAgent.SetVariableValue("TargetLocation", e.target);
        }

        // One giant OR statement of dictionary values
        _behaviorAgent.SetVariableValue("ruleBroken", _rulesLibrary.Values.Any(value => value));
    }

    /// <summary>
    /// Brings the Rulekeepr back to spawn, resets their behaviors in between levels
    /// </summary>
    private void DisableRulekeeper(LoadLevel e)
    {
        _behaviorAgent.enabled = false;
        _navAgent.enabled = false;
        _behaviorAgent.SetVariableValue("ruleBroken", false);
        _behaviorAgent.Restart();

        // TODO: set up the list of rules with a new dataset
    }

    private void EnableRuleKeeper(LevelLoaded e)
    {
        _behaviorAgent.enabled = true;
        _navAgent.enabled = true;
    }

    public void OnEnable()
    {
        _ruleBroken = new EventBinding<RuleBroken>(OnRuleBroken);
        EventBus<RuleBroken>.Register(_ruleBroken);
        _loadLevel = new EventBinding<LoadLevel>(DisableRulekeeper);
        EventBus<LoadLevel>.Register(_loadLevel);
        _levelLoaded = new EventBinding<LevelLoaded>(EnableRuleKeeper);
        EventBus<LevelLoaded>.Register(_levelLoaded);
    }

    public void OnDisable()
    {
        EventBus<RuleBroken>.DeRegister(_ruleBroken);
        EventBus<LoadLevel>.DeRegister(_loadLevel);
        EventBus<LevelLoaded>.DeRegister(_levelLoaded);
    }
}
