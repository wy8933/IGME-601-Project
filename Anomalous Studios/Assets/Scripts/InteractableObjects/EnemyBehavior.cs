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

// Some notes about unexpected behaviors:
    // To update the RuleKeeper's speed, use the value found in the RulekeeperBehaviors graph. In code, 
        // The speed value under Steering in the navmesh agent component has no effect, and should not be modified.
    // Make sure to update the prefab with any changes before doing testing, doing so has fixed unexpected behavior in the past.


/// <summary>
/// A controller for the Rulekeeper's unique rules-dependent behaviors
/// </summary>
public class EnemyBehavior : MonoBehaviour, IInteractable
{
    private EventBinding<RuleBroken> _ruleBroken;
    private EventBinding<MakeNoise> _makeNoise;

    private BehaviorGraphAgent _behaviorAgent;
    private NavMeshAgent _navAgent;

    private int _ignoreLayers;

    private bool _canInteract = true;

    /// <summary>
    /// Used 
    /// </summary>
    public float OriginalSpeed { get; private set; }

    public float HoldTime { get => 0.0f; }
    public bool CanInteract { get => _canInteract; set => _canInteract = value; }

    public float CurrentSpeed
    {
        get { _behaviorAgent.GetVariable("Speed", out BlackboardVariable<float> speed); return speed; }

        set => _behaviorAgent.SetVariableValue("Speed", value);
    }

    [Header("Reaction SFX")]
    [SerializeField] private SoundDataSO _failedSFX;
    [SerializeField] private SoundDataSO _successSFX;
    [SerializeField] private SoundDataSO _ambianceSFX;
    [SerializeField] private SoundDataSO _screamSFX;
    [SerializeField] private SoundDataSO _staticSFX;
    public SoundDataSO InitialSFX => null;
    public SoundDataSO FailedSFX { get => _failedSFX; }
    public SoundDataSO CancelSFX => null;
    public SoundDataSO SuccessSFX { get => _successSFX; }

    // TODO: change to an enum value instead of string names for rules
    private Dictionary<string, bool> _rulesLibrary = new Dictionary<string, bool>
    {
        { "lights", false },
        { "camera", false }, // Temp rule
        { "action!", false } // Temp rule
    };

    private Vector3[] _sightCone;

    public void Start()
    {
        _behaviorAgent = GetComponent<BehaviorGraphAgent>();
        _navAgent = GetComponent<NavMeshAgent>();
        _behaviorAgent.SetVariableValue("Player",
            GameObject.FindGameObjectWithTag("Player"));

        _ignoreLayers = ~LayerMask.GetMask("RuleKeeper", "Ignore Raycast");

        // Only use behaviorgraphagent's speed value, see explanation above
        _behaviorAgent.GetVariable("Speed", out BlackboardVariable speed); 
        OriginalSpeed = (float)speed.ObjectValue;

        //SoundEffectTrigger.Instance.PlayAmbience(transform);
        AudioManager.Instance.Play(_ambianceSFX, gameObject, transform.position);
    }

    public void Highlight()
    {
        // Highlight the target!
    }
    public void RemoveHighlight()
    {
        // Remove the highlight
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
            out RaycastHit hit, 1000, _ignoreLayers) && hit.collider.CompareTag("Player"))
        {
            _behaviorAgent.SetVariableValue("playerSeen", true);
            UpdateTargetLocation(new MakeNoise { target = hit.transform.position });
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

        //SoundEffectTrigger.Instance.PlayScream(transform);
        if (e.isBroken)
        {
            AudioManager.Instance.Play(_screamSFX, gameObject, transform.position);
            AudioManager.Instance.Stop(gameObject, _ambianceSFX);
            AudioManager.Instance.Play(_staticSFX, gameObject, transform.position);
        }
        else
        {
            AudioManager.Instance.Stop(gameObject, _staticSFX);
            AudioManager.Instance.Play(_ambianceSFX, gameObject, transform.position);
        }
    }

    private void UpdateTargetLocation(MakeNoise e)
    {
        // TODO: I forsee some issues with the target location having INSTANT priority
        // Might need to introduce a priority queue of target positions, nodes to "check out"
        _behaviorAgent.SetVariableValue("TargetLocation", e.target);
        //SoundEffectTrigger.Instance.StopAmbience();
    }


    public void OnEnable()
    {
        _ruleBroken = new EventBinding<RuleBroken>(OnRuleBroken);
        EventBus<RuleBroken>.Register(_ruleBroken);
        _makeNoise = new EventBinding<MakeNoise>(UpdateTargetLocation);
        EventBus<MakeNoise>.Register(_makeNoise);
    }

    public void OnDisable()
    {
        EventBus<RuleBroken>.DeRegister(_ruleBroken);
        EventBus<MakeNoise>.DeRegister(_makeNoise);
    }

    public void OnDestroy()
    {
        AudioManager.Instance.Stop(gameObject, _staticSFX);
        AudioManager.Instance.Stop(gameObject, _ambianceSFX);
    }
}
