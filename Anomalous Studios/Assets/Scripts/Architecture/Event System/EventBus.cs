using System;
using System.Collections.Generic;
using UnityEngine;

public interface IEvent { }
public interface IPrioritized { int Priority { get; } }
public static class EventBus<T> where T : IEvent
{
    static readonly List<IEventBinding<T>> _global = new List<IEventBinding<T>>();
    static readonly Dictionary<object, List<IEventBinding<T>>> _scoped = new Dictionary<object, List<IEventBinding<T>>>();

    static bool _hasStickyGlobal;
    static T _stickyGlobal;
    static readonly Dictionary<object, (bool has, T val)> _stickyScoped = new Dictionary<object, (bool has, T val)>();

    internal static void Register(IEventBinding<T> binding) => InsertByPriority(_global, binding);

    internal static void Register(IEventBinding<T> binding, object scope)
    {
        if (!_scoped.TryGetValue(scope, out var list))
            _scoped[scope] = list = new List<IEventBinding<T>>();
        InsertByPriority(list, binding);
    }

    internal static void DeRegister(IEventBinding<T> binding)
    {
        _global.Remove(binding);

        var emptyKeys = new List<object>();
        foreach (var kv in _scoped)
        {
            kv.Value.Remove(binding);
            if (kv.Value.Count == 0)
                emptyKeys.Add(kv.Key);
        }
        for (int i = 0; i < emptyKeys.Count; i++)
            _scoped.Remove(emptyKeys[i]);
    }

    internal static void DeRegister(IEventBinding<T> binding, object scope)
    {
        if (scope == null) { DeRegister(binding); return; }

        if (_scoped.TryGetValue(scope, out var list))
        {
            list.Remove(binding);
            if (list.Count == 0)
                _scoped.Remove(scope);
        }
    }

    static void InsertByPriority(List<IEventBinding<T>> list, IEventBinding<T> binding)
    {
        int p = (binding is IPrioritized pr) ? pr.Priority : 0;
        int idx = list.FindIndex(b => ((b as IPrioritized)?.Priority ?? 0) < p);
        if (idx >= 0) list.Insert(idx, binding); else list.Add(binding);
    }

    /// <summary>
    /// Accept an event and active both of the action on the binding
    /// </summary>
    /// <param name="event"></param>
    public static void Raise(T @event)
    {
        var snapshot = _global.ToArray();
        for (int i = 0; i < snapshot.Length; i++)
        {
            var b = snapshot[i];
            b.OnEvent.Invoke(@event);
            b.OnEventNoArgs.Invoke();
        }
    }

    public static void RaiseScoped(object scope, T @event)
    {
        if (_scoped.TryGetValue(scope, out var list))
        {
            var snapshot = list.ToArray();
            for (int i = 0; i < snapshot.Length; i++)
            {
                var b = snapshot[i];
                b.OnEvent.Invoke(@event);
                b.OnEventNoArgs.Invoke();
            }
        }
    }

    public static void RaiseSticky(T @event)
    {
        _stickyGlobal = @event;
        _hasStickyGlobal = true;
        Raise(@event);
    }

    public static void RaiseStickyScoped(object scope, T @event)
    {
        if (scope == null) { RaiseSticky(@event); return; }
        _stickyScoped[scope] = (true, @event);
        RaiseScoped(scope, @event);
    }

    internal static void RegisterSticky(IEventBinding<T> binding, bool global = true, object scope = null)
    {
        if (global || scope == null) Register(binding);
        else Register(binding, scope);

        if (global)
        {
            if (_hasStickyGlobal)
            {
                binding.OnEvent.Invoke(_stickyGlobal);
                binding.OnEventNoArgs.Invoke();
            }
        }
        else if (_stickyScoped.TryGetValue(scope, out var s) && s.has)
        {
            binding.OnEvent.Invoke(s.val);
            binding.OnEventNoArgs.Invoke();
        }
    }

    static void Clear()
    {
        _global.Clear();
        _scoped.Clear();
        _stickyScoped.Clear();
        _hasStickyGlobal = false;
        _stickyGlobal = default;
    }

    public static int ListenerCountGlobal => _global.Count;
    public static int ListenerCountScoped(object scope) =>
        _scoped.TryGetValue(scope, out var l) ? l.Count : 0;
}