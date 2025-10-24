using RuleViolationSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

public class RuleQueryAdapter : MonoBehaviour, IRuleQuery
{
    [SerializeField] private string _floorId = "first-floor";
    public string FloorId => _floorId;

    public DateTime UtcNow => DateTime.UtcNow;

    /// <summary>
    /// Checks a single variable condition.
    /// </summary>
    /// <param name="cond">The condition to evaluate.</param>
    /// <returns>True if the condition passes; otherwise false.</returns>
    public bool Check(VariableCondition cond)
    {
        return VariableConditionManager.Instance.Check(cond);
    }

    /// <summary>
    /// Checks that all provided conditions pass.
    /// </summary>
    /// <param name="conds">Conditions to evaluate (null means pass).</param>
    /// <returns>True if every condition passes; otherwise false.</returns>
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

    /// <summary>
    /// Sets the current floor id.
    /// </summary>
    /// <param name="id">New floor identifier.</param>
    public void setFloorID(string id)
    {
        _floorId = id;
    }

    /// <summary>
    /// Tries to read a raw string value for a key.
    /// </summary>
    /// <param name="key">Variable key.</param>
    /// <param name="raw">Output raw string value.</param>
    /// <returns>True if found; otherwise false.</returns>
    public bool TryGet(string key, out string raw)
    {
        return VariableConditionManager.Instance.TryGet(key, out raw);
    }

    /// <summary>
    /// Tries to read a float value for a key.
    /// </summary>
    /// <param name="key">Variable key.</param>
    /// <param name="value">Output float value.</param>
    /// <returns>True if parsed; otherwise false.</returns>
    public bool TryGetFloat(string key, out float value)
    {
        string output;
        VariableConditionManager.Instance.TryGet(key, out output);

        return float.TryParse(output, out value);
    }

    /// <summary>
    /// Tries to read an int value for a key.
    /// </summary>
    /// <param name="key">Variable key.</param>
    /// <param name="value">Output int value.</param>
    /// <returns>True if parsed; otherwise false.</returns>
    public bool TryGetInt(string key, out int value)
    {
        string output;
        VariableConditionManager.Instance.TryGet(key, out output);

        return int.TryParse(output, out value);
    }

    /// <summary>
    /// Checks whether an interaction was seen within a time window.
    /// </summary>
    /// <param name="interactionId">Interaction identifier.</param>
    /// <param name="withinSeconds">Time window in seconds.</param>
    /// <returns>True if seen within the window; otherwise false.</returns>
    public bool WasInteractionSeen(string interactionId, float withinSeconds)
    {
        Debug.Log ("WasInteractionSeen is not implemented yet");
        return false;
    }

}
