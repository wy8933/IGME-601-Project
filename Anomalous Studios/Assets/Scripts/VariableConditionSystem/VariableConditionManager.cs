using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public enum VariableCompareType { Equals, NotEquals, GreaterThan, LessThan }

public class VariableConditionManager : MonoBehaviour
{
    public static VariableConditionManager Instance { get; private set; }
    public ConditionBlackboardSO conditionBlackboard;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Clears the singleton reference if this instance is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    /// <summary>
    /// Set or add a variable on the blackboard.
    /// </summary>
    /// <param name="variableName">The key of the variable.</param>
    /// <param name="variableValue">The value to store.</param>
    public void Set(string variableName, string variableValue)
    {
        if (!CheckBlackboardAssigned()) return;
        conditionBlackboard.SetVariable(variableName, variableValue);
    }

    /// <summary>
    /// Get a variable value from the blackboard.
    /// </summary>
    /// <param name="variableName">The key to look up.</param>
    /// <returns>The stored value, or null if not found or if the blackboard is missing.</returns>
    public string Get(string variableName)
    {
        if (!CheckBlackboardAssigned()) return null;
        return conditionBlackboard.GetVariable(variableName);
    }

    /// <summary>
    /// Try to get a variable value from the blackboard.
    /// </summary>
    /// <param name="variableName">The key to look up.</param>
    /// <param name="variableValue">Out parameter receiving the value if found.</param>
    /// <returns>true if the key exists, otherwise false.</returns>
    public bool TryGet(string variableName, out string variableValue)
    {
        variableValue = null;
        if (!CheckBlackboardAssigned()) return false;
        return conditionBlackboard.TryGetVariable(variableName, out variableValue);
    }

    /// <summary>
    /// Remove all variables from the blackboard.
    /// </summary>
    public void ClearAll()
    {
        if (!CheckBlackboardAssigned()) return;
        conditionBlackboard.ClearAll();
    }

    /// <summary>
    /// Evaluate a single VariableCondition against current blackboard data.
    /// </summary>
    /// <param name="condition">The condition to evaluate.</param>
    /// <returns>
    /// true if the condition passes, and false if it fails, the key is missing,
    /// the values cannot be parsed (for numeric comparisons), or the blackboard is missing.
    /// </returns>
    public bool Check(VariableCondition condition)
    {
        if (!CheckBlackboardAssigned() || condition == null) return false;

        if (!conditionBlackboard.TryGetVariable(condition.variableName, out var currentVariableValue) ||
            currentVariableValue == null) return false;

        switch (condition.compareType)
        {
            case VariableCompareType.Equals:
                return currentVariableValue == condition.compareValue;

            case VariableCompareType.NotEquals:
                return currentVariableValue != condition.compareValue;

            case VariableCompareType.GreaterThan:
                return TryParseFloatPair(currentVariableValue, condition.compareValue,
                                         out var leftParsedValue, out var rightParsedValue)
                       && leftParsedValue > rightParsedValue;

            case VariableCompareType.LessThan:
                return TryParseFloatPair(currentVariableValue, condition.compareValue,
                                         out var leftParsedValue2, out var rightParsedValue2)
                       && leftParsedValue2 < rightParsedValue2;

            default:
                return false;
        }
    }

    /// <summary>
    /// Evaluate a list of conditions, all conditions must pass.
    /// </summary>
    /// <param name="conditionsToEvaluate">List of conditions, if null/empty, returns true.</param>
    /// <returns>true if all pass, otherwise false.</returns>
    public bool CheckAll(List<VariableCondition> conditionsToEvaluate)
    {
        if (!CheckBlackboardAssigned()) return false;
        if (conditionsToEvaluate == null || conditionsToEvaluate.Count == 0) return true;

        for (int i = 0; i < conditionsToEvaluate.Count; i++)
        {
            if (!Check(conditionsToEvaluate[i])) return false;
        }
        return true;
    }

    /// <summary>
    /// Check if the blackboard reference is assigned, logs a warning if not.
    /// </summary>
    /// <returns>true if assigned, otherwise false.</returns>
    private bool CheckBlackboardAssigned()
    {
        if (conditionBlackboard != null) return true;
        Debug.LogWarning("VariableConditionManager's conditionBlackboard is not assigned.");
        return false;
    }

    /// <summary>
    /// Try to parse two strings as floats.
    /// </summary>
    /// <param name="leftVariableText">Left variable as string.</param>
    /// <param name="rightVariableText">Right variable as string.</param>
    /// <param name="leftVariableValue">Parsed left float.</param>
    /// <param name="rightVariableValue">Parsed right float.</param>
    /// <returns>true if both parsed successfully, otherwise false.</returns>
    private static bool TryParseFloatPair(string leftVariableText, string rightVariableText,
                                          out float leftVariableValue, out float rightVariableValue)
    {
        bool isLeftParsed = float.TryParse(leftVariableText, out leftVariableValue);
        bool isRightParsed = float.TryParse(rightVariableText, out rightVariableValue);
        return isLeftParsed && isRightParsed;
    }
}

/// <summary>
/// Serializable description of a single variable comparison to perform against the blackboard.
/// </summary>
[System.Serializable]
public class VariableCondition
{
    public string variableName;
    public VariableCompareType compareType = VariableCompareType.Equals;
    public string compareValue;

    public VariableCondition() : this("NewVariable", VariableCompareType.Equals, "") { }

    public VariableCondition(string variableName, VariableCompareType compareType, string compareValue)
    {
        this.variableName = variableName;
        this.compareType = compareType;
        this.compareValue = compareValue;
    }
}
