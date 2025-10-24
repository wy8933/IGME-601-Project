using System;
using System.Collections.Generic;
using UnityEngine;

public enum ValueType { String, Int, Float }

public static class GameVariables
{
    public static bool Verbose = false;
    public static float GlobalFloatTolerance = 0.0001f;
    public static string DefaultReason = "Gameplay";

    private static int _batchDepth = 0;
    private static readonly List<VariableChangedEvent> _queued = new();

    /// <summary>
    /// Sets a variable by key and emits a VariableChangedEvent if the value changed.
    /// Suppresses events for equal values (or floats within tolerance).
    /// </summary>
    /// <param name="key">Variable key (may end with :int or :float to hint type).</param>
    /// <param name="value">New raw string value.</param>
    /// <param name="reason">Optional change reason; defaults to DefaultReason.</param>
    public static void Set(string key, string value, string reason = null) 
    {

#if UNITY_EDITOR
        if (System.Threading.Thread.CurrentThread.ManagedThreadId != 1)
        {
            Debug.LogError($" Set() called off main thread: {key}");
        }
#endif

        string prevRaw = null;
        bool hadPrev = VariableConditionManager.Instance.TryGet(key, out prevRaw);

        ValueType vt = InferTypeFromSuffix(key);

        bool hasInt = false; int intVal = 0, prevInt = 0;
        bool hasFloat = false; float floatVal = 0f, prevFloat = 0f;

        switch (vt)
        {
            case ValueType.Int:
                hasInt = TryParseInt(value, out intVal);
                if (hadPrev) TryParseInt(prevRaw, out prevInt);
                break;

            case ValueType.Float:
                hasFloat = TryParseFloat(value, out floatVal);
                if (hadPrev) TryParseFloat(prevRaw, out prevFloat);
                break;

            case ValueType.String:
                break;
        }
        bool suppress = IsEffectivelySame(
            vt,
            hadPrev, prevRaw,
            hasInt, intVal, prevInt,
            hasFloat, floatVal, prevFloat,
            value,
            GlobalFloatTolerance
        );

        if (!suppress)
        {
            var payload = new VariableChangedEvent(
                key: key,
                rawValue: value,
                hasInt: hasInt, intValue: intVal,
                hasFloat: hasFloat, floatValue: floatVal,
                prevRawValue: hadPrev ? prevRaw : null,
                reason: string.IsNullOrEmpty(reason) ? DefaultReason : reason
            );

            VariableConditionManager.Instance.Set(key, value);
            RaiseOrQueue(payload);

            if (Verbose)
                Debug.Log($"[GameVariables] emit {key}: '{prevRaw}' to '{value}'"
                    + (hasInt ? $" (int={intVal})" : "")
                    + (hasFloat ? $" (float={floatVal})" : ""));
        }
        else if (Verbose)
        {
            Debug.Log($"[GameVariables] suppress {key}: unchanged (or < tol)");
        }
    }

    /// <summary>
    /// Begins a batch so change events are queued instead of raised immediately.
    /// </summary>
    public static void BeginBatch() 
    {
        _batchDepth++;
    }

    /// <summary>
    /// Ends a batch depth and flushes queued events when depth reaches zero.
    /// </summary>
    public static void CommitBatch() 
    {
        _batchDepth = Mathf.Max(0, _batchDepth - 1);
        if (_batchDepth == 0 && _queued.Count > 0) 
        {
            for (int i = 0; i < _queued.Count; i++) 
            {
                EventBus<VariableChangedEvent>.Raise(_queued[i]);
            }

            _queued.Clear();
        }
    }

    /// <summary>
    /// Infers value type from key suffix (:int or :float). Defaults to string.
    /// </summary>
    /// <param name="key">Variable key.</param>
    /// <returns>Inferred <see cref="ValueType"/>.</returns>
    private static ValueType InferTypeFromSuffix(string key) 
    {
        if (key.EndsWith(":int", StringComparison.Ordinal)) return ValueType.Int;
        else if (key.EndsWith(":float", StringComparison.Ordinal)) return ValueType.Float;

        return ValueType.String;
    }

    /// <summary>
    /// Checks if a new value is effectively the same as the previous one.
    /// Uses exact match for strings/ints and tolerance for floats.
    /// </summary>
    private static bool IsEffectivelySame(
    ValueType vt,
    bool hadPrev, string prevRaw,
    bool hasInt, int intVal, int prevInt,
    bool hasFloat, float floatVal, float prevFloat,
    string curRaw, float tol)
    {
        if (!hadPrev) return false;

        switch (vt)
        {
            case ValueType.Int:
                if (hasInt && TryParseInt(prevRaw, out prevInt)) return intVal == prevInt;
                return string.Equals(prevRaw, curRaw, StringComparison.Ordinal);

            case ValueType.Float:
                if (hasFloat && TryParseFloat(prevRaw, out prevFloat))
                    return Mathf.Abs(floatVal - prevFloat) < tol;
                return string.Equals(prevRaw, curRaw, StringComparison.Ordinal);

            case ValueType.String:
            default:
                return string.Equals(prevRaw, curRaw, StringComparison.Ordinal);
        }
    }

    /// <summary>
    /// Parses an int from a raw string.
    /// </summary>
    private static bool TryParseInt(string raw, out int v) => int.TryParse(raw, out v);

    /// <summary>
    /// Parses a float from a raw string.
    /// </summary>
    private static bool TryParseFloat(string raw, out float v) => float.TryParse(raw, out v);

    /// <summary>
    /// Raises the event immediately or queues it if inside a batch.
    /// </summary>
    /// <param name="payload">Change event payload.</param>
    private static void RaiseOrQueue(VariableChangedEvent payload)
    {
        if (_batchDepth > 0) { _queued.Add(payload); }
        else { EventBus<VariableChangedEvent>.Raise(payload); }
    }

}
