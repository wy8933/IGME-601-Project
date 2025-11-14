using RuleViolationSystem;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class RuleManager : MonoBehaviour
{
    public static RuleManager Instance;

    public bool verbose = false;

    public RuleSetSO ruleSet;

    private RuleQueryAdapter _query;

    private readonly Dictionary<string, List<RuleAssetSO>> _depMap = new();

    private readonly Dictionary<RuleAssetSO, RuleRuntimeState> _state = new();

    private EventBinding<VariableChangedEvent> _varBinding;

    private void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        BuildDependencyMap();
        InitRuleState();

        BindEvents();
    }
    private void Start()
    {
        _query = gameObject.AddComponent<RuleQueryAdapter>();
        _query.setFloorID("first_floor");

        EvaluateAll("Init");
    }

    private void OnDisable()
    {
        UnbindEvents();
        _depMap.Clear();
        _state.Clear();
    }

    /// <summary>
    /// Registers the variable-changed listener.
    /// </summary>
    private void BindEvents()
    {
        _varBinding = new EventBinding<VariableChangedEvent>(OnVariableChanged);
        EventBus<VariableChangedEvent>.Register(_varBinding);
    }

    /// <summary>
    /// Unregisters the variable-changed listener.
    /// </summary>
    private void UnbindEvents()
    {
        if (_varBinding != null)
            EventBus<VariableChangedEvent>.DeRegister(_varBinding);
    }

    /// <summary>
    /// Reacts to a variable change by evaluating impacted rules or all as fallback.
    /// </summary>
    /// <param name="e">Change event with key and value.</param>
    private void OnVariableChanged(VariableChangedEvent e)
    {
        if (string.IsNullOrEmpty(e.Key)) return;

        if (_depMap.TryGetValue(e.Key, out var rules) && rules.Count > 0)
        {
            for (int i = 0; i < rules.Count; i++)
                EvaluateRule(rules[i], reason: $"Var:{e.Key}");
        }
        else
        {
            if (verbose) Debug.LogWarning($" No dep-map entry for key '{e.Key}'. Falling back to EvaluateAll.");
            EvaluateAll($"Var:{e.Key} (fallback)"); 
        }

        if (verbose) Debug.Log($"VarChanged key='{e.Key}' value='{e.RawValue}'");
    }

    /// <summary>
    /// Initializes runtime state for all rules in the set.
    /// </summary>
    private void InitRuleState()
    {
        _state.Clear();
        if (ruleSet?.rules == null) return;

        foreach (var r in ruleSet.rules)
        {
            if (r == null) continue;
            _state[r] = new RuleRuntimeState { LastViolated = false, Latched = false, LastFiredUtc = DateTime.MinValue };
        }
    }

    /// <summary>
    /// Evaluates a single rule and fires violation/resolve actions as needed.
    /// </summary>
    /// <param name="rule">Rule to evaluate.</param>
    /// <param name="reason">Reason tag for logging.</param>
    private void EvaluateRule(RuleAssetSO rule, string reason)
    {
        var s = _state[rule];
        bool violated = EvaluateConditions(rule);

        if (violated && !s.LastViolated)
        {
            LogRule($"{rule.ruleId}: OK to violated (rule breaked)");
            FireViolation(rule, s, reason);
            s.LastViolated = true;
            return;
        }

        if (violated && s.LastViolated)
        {
            if (rule.fireOnceUntilResolved && s.Latched)
            {
                LogRule($"{rule.ruleId}: violated to violated");
                return;
            }

            var rem = CooldownRemaining(s.LastFiredUtc, rule.cooldownSeconds);
            if (rem > 0.0f)
            {
                LogRule($"{rule.ruleId}: violated to violated");
                return;
            }

            LogRule($"{rule.ruleId}: violated to violated");
            //FireViolation(rule, s, reason);
            return;
        }

        if (!violated && s.LastViolated)
        {
            LogRule($"{rule.ruleId}: violated to OK (resolve)");
            s.LastViolated = false;
            s.Latched = false;
            FireResolve(rule);
            return;
        }

        LogRule($"{rule.ruleId}: OK to OK (no action)");
    }

    /// <summary>
    /// Evaluates the rule's conditions using the active query.
    /// </summary>
    /// <param name="rule">Rule to check.</param>
    /// <returns>True if the rule is violated; otherwise false.</returns>
    private bool EvaluateConditions(RuleAssetSO rule)
    {
        if (_query == null) { LogRule("Query is null, skipping evaluation"); return false; }
        if (rule.ruleConditions == null || rule.ruleConditions.Length == 0) return false;

        bool any = false, all = true;
        for (int i = 0; i < rule.ruleConditions.Length; i++)
        {
            var c = rule.ruleConditions[i];
            if (c == null) continue;
            bool v = c.IsViolated(_query);
            any |= v;
            all &= v;
        }
        return rule.conditionLogic == ConditionLogic.All ? all : any;
    }

    /// <summary>
    /// Evaluates all rules in the set.
    /// </summary>
    /// <param name="reason">Reason tag for logging.</param>
    private void EvaluateAll(string reason)
    {
        if (ruleSet == null || ruleSet.rules == null || ruleSet.rules.Length == 0) return;

        foreach (var r in ruleSet.rules)
            if (r != null) EvaluateRule(r, reason);
    }

    /// <summary>
    /// Executes violation actions and updates runtime state timestamps amd latches.
    /// </summary>
    /// <param name="rule">Rule that violated.</param>
    /// <param name="s">Runtime state to update.</param>
    /// <param name="reason">Reason tag for logging.</param>
    private void FireViolation(RuleAssetSO rule, RuleRuntimeState s, string reason)
    {
        s.LastFiredUtc = DateTime.UtcNow;
        if (rule.fireOnceUntilResolved) s.Latched = true;

        var acts = rule.violationActions;
        if (acts != null)
            for (int i = 0; i < acts.Length; i++)
                acts[i]?.Execute(_query, rule);

    }

    /// <summary>
    /// Executes resolve actions.
    /// </summary>
    /// <param name="rule">Rule transitioning to OK.</param>
    private void FireResolve(RuleAssetSO rule)
    {
        var acts = rule.resolveActions;
        if (acts != null)
            for (int i = 0; i < acts.Length; i++)
                acts[i]?.Execute(_query, rule);
    }

    /// <summary>
    /// Builds a map from variable keys to the rules that reference them.
    /// </summary>
    private void BuildDependencyMap()
    {
        _depMap.Clear();
        if (ruleSet == null || ruleSet.rules == null) return;

        foreach (var rule in ruleSet.rules)
        {
            if (rule == null || rule.ruleConditions == null) continue;

            if (!_state.ContainsKey(rule))
                _state[rule] = new RuleRuntimeState { LastFiredUtc = DateTime.MinValue };

            var seen = new HashSet<string>(StringComparer.Ordinal);
            foreach (var cond in rule.ruleConditions)
            {
                if (cond == null) continue;
                foreach (var key in cond.ReferencedVariableKeys())
                {
                    if (string.IsNullOrEmpty(key) || !seen.Add(key)) continue;

                    if (!_depMap.TryGetValue(key, out var list))
                    {
                        list = new List<RuleAssetSO>(4);
                        _depMap[key] = list;
                    }
                    if (!list.Contains(rule)) list.Add(rule);
                }
            }
        }
    }

    /// <summary>
    /// Logs a rules message when verbose is enabled.
    /// </summary>
    /// <param name="msg">Message to log.</param>
    private void LogRule(string msg)
    {
        if (verbose) Debug.Log($"Rules: {msg}");
    }

    /// <summary>
    /// Returns remaining cooldown time in seconds.
    /// </summary>
    /// <param name="lastFiredUtc">Last time the rule fired .</param>
    /// <param name="cooldownSeconds">Rule cooldown length in seconds.</param>
    /// <returns>Seconds remaining (0 if ready).</returns>
    private static double CooldownRemaining(DateTime lastFiredUtc, float cooldownSeconds)
    {
        var elapsed = (DateTime.UtcNow - lastFiredUtc).TotalSeconds;
        return Math.Max(0.0, cooldownSeconds - elapsed);
    }


    private sealed class RuleRuntimeState
    {
        public bool LastViolated;
        public DateTime LastFiredUtc;
        public bool Latched;
    }
}

