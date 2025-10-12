using RuleViolationSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

public class RuleQueryAdapter : MonoBehaviour, IRuleQuery
{
    [SerializeField] private string _floorId = "first-floor";
    public string FloorId => _floorId;

    public DateTime UtcNow => DateTime.UtcNow;

    public bool Check(VariableCondition cond)
    {
        return VariableConditionManager.Instance.Check(cond);
    }

    public bool CheckAll(IEnumerable<VariableCondition> conds)
    {
        if (conds == null) return true;
        foreach (var c in conds)
        {
            if (!VariableConditionManager.Instance.Check(c))
                return false;
        }
        return true;
    }

    public void setFloorID(string id)
    {
        _floorId = id;
    }

    public bool TryGet(string key, out string raw)
    {
        return VariableConditionManager.Instance.TryGet(key, out raw);
    }

    public bool TryGetFloat(string key, out float value)
    {
        string output;
        VariableConditionManager.Instance.TryGet(key, out output);

        return float.TryParse(output, out value);
    }

    public bool TryGetInt(string key, out int value)
    {
        string output;
        VariableConditionManager.Instance.TryGet(key, out output);

        return int.TryParse(output, out value);
    }

    public bool WasInteractionSeen(string interactionId, float withinSeconds)
    {
        Debug.Log ("WasInteractionSeen is not implemented yet");
        return false;
    }

}
