using System;
using System.Collections.Generic;
using UnityEngine;

public struct CommandRequested : IEvent
{
    public string Id;
    public string[] Args;
}

public struct ConsoleLog : IEvent
{
    public string Message;
    public LogType Type;
}

public static class CommandScope
{
    static readonly Dictionary<string, object> _keys = new();
    public static object Key(string id)
    {
        if (!_keys.TryGetValue(id, out var key))
        { key = new object(); _keys[id] = key; }
        return key;
    }
}
