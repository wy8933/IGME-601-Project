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

    public static void BeginBatch() 
    {
        _batchDepth++;
    }
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


    private static ValueType InferTypeFromSuffix(string key) 
    {
        if (key.EndsWith(":int", StringComparison.Ordinal)) return ValueType.Int;
        else if (key.EndsWith(":float", StringComparison.Ordinal)) return ValueType.Float;

        return ValueType.String;
    }

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


    private static bool TryParseInt(string raw, out int v) => int.TryParse(raw, out v);
    private static bool TryParseFloat(string raw, out float v) => float.TryParse(raw, out v);

    private static void RaiseOrQueue(VariableChangedEvent payload)
    {
        if (_batchDepth > 0) { _queued.Add(payload); }
        else { EventBus<VariableChangedEvent>.Raise(payload); }
    }

}
