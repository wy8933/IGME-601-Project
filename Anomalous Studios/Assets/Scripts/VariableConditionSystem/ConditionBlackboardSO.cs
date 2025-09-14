using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConditionBlackboard", menuName = "Systems/Condition Blackboard SO")]
public class ConditionBlackboardSO : ScriptableObject
{
    [System.Serializable]
    public class StringKeyValue
    {
        public string key;
        public string value;
    }

    [SerializeField] private List<StringKeyValue> _variableList = new List<StringKeyValue>();
    private Dictionary<string, string> _variables = null;

    private void OnEnable() { RebuildDictionary(); }
    private void OnValidate() { RebuildDictionary(); }

    /// <summary>
    /// Reconstruct the runtime dictionary from the serialized list.
    /// </summary>
    private void RebuildDictionary()
    {
        _variables = new Dictionary<string, string>();
        foreach (var keyValuePair in _variableList)
        {
            if (!string.IsNullOrEmpty(keyValuePair.key))
                _variables[keyValuePair.key] = keyValuePair.value;
        }
    }

    /// <summary>
    /// Set or add a variable, updates both the runtime dictionary and the serialized list.
    /// </summary>
    /// <param name="name">Variable key.</param>
    /// <param name="value">Value to store.</param>
    public void SetVariable(string name, string value)
    {
        if (_variables == null) RebuildDictionary();

        if (_variables.ContainsKey(name))
        {
            _variables[name] = value;
            var pair = _variableList.Find(keyValuePair => keyValuePair.key == name);
            if (pair != null) pair.value = value;
        }
        else
        {
            _variables.Add(name, value);
            _variableList.Add(new StringKeyValue { key = name, value = value });
        }
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

    /// <summary>
    /// Get a variable value by key.
    /// </summary>
    /// <param name="name">Variable key.</param>
    /// <returns>The stored value, or null if not found.</returns>
    public string GetVariable(string name)
    {
        if (_variables == null) RebuildDictionary();
        return _variables != null && _variables.TryGetValue(name, out var value) ? value : null;
    }

    /// <summary>
    /// Try get a variable value without allocating or logging.
    /// </summary>
    /// <param name="name">Variable key.</param>
    /// <param name="value">Out parameter receiving the value if found.</param>
    /// <returns>true if found, otherwise false.</returns>
    public bool TryGetVariable(string name, out string value)
    {
        if (_variables == null) RebuildDictionary();
        if (_variables != null && _variables.TryGetValue(name, out value)) return true;
        value = null;
        return false;
    }

    /// <summary>
    /// Remove all variables from both the serialized list and the runtime dictionary.
    /// </summary>
    public void ClearAll()
    {
        _variableList.Clear();
        _variables = new Dictionary<string, string>();
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}
