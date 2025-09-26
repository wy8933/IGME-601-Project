using RuleViolationSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

public class RuleQueryAdapter : MonoBehaviour, IRuleQuery
{
    public string FloorId { get; }
    private readonly VariableConditionManager _vars;

    public DateTime UtcNow => DateTime.UtcNow;


    public RuleQueryAdapter(string floorId, VariableConditionManager variableManager)
    {
        FloorId = floorId;
        _vars = variableManager ?? throw new ArgumentNullException(nameof(variableManager));
    }

    public bool Check(VariableCondition cond)
    {
        return _vars.Check(cond);
    }

    public bool CheckAll(IEnumerable<VariableCondition> conds)
    {
        if (conds == null) return true;
        foreach (var c in conds)
        {
            if (!_vars.Check(c))
                return false;
        }
        return true;
    }

    public bool TryGet(string key, out string raw)
    {
        return _vars.TryGet(key, out raw);
    }

    public bool TryGetFloat(string key, out float value)
    {
        string output;
        _vars.TryGet(key, out output);

        return float.TryParse(output, out value);
    }

    public bool TryGetInt(string key, out int value)
    {
        string output;
        _vars.TryGet(key, out output);

        return int.TryParse(output, out value);
    }

    public bool WasInteractionSeen(string interactionId, float withinSeconds)
    {
        Debug.Log ("WasInteractionSeen is not implemented yet");
        return false;
    }

}
