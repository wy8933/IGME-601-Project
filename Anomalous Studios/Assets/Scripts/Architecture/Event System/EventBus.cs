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

    /// <summary>
    /// Registers a binding to receive all global events of type T.
    /// Preserves priority order if the binding implements IPrioritized
    /// </summary>
    /// <param name="binding">The binding to register.</param>
    internal static void Register(IEventBinding<T> binding) => InsertByPriority(_global, binding);

    /// <summary>
    /// Registers a binding to receive events within a specific scope.
    /// Preserves priority order if the binding implements IPrioritized.
    /// </summary>
    /// <param name="binding">The binding to register.</param>
    /// <param name="scope">Scope key that groups listeners and raised events.</param>
    internal static void Register(IEventBinding<T> binding, object scope)
    {
        if (!_scoped.TryGetValue(scope, out var list))
            _scoped[scope] = list = new List<IEventBinding<T>>();
        InsertByPriority(list, binding);
    }

    /// <summary>
    /// Deregisters a binding from both global and any scoped lists where it appears.
    /// Removes empty scope lists.
    /// </summary>
    /// <param name="binding">The binding to remove.</param>
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

    /// <summary>
    /// Deregisters a binding from a specific scope only.
    /// If scope is null, behaves like the global .
    /// </summary>
    /// <param name="binding">The binding to remove.</param>
    /// <param name="scope">Scope list to remove from.</param>
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

    /// <summary>
    /// Inserts a binding into a list based on descending IPrioritized.Priority.
    /// Non-prioritized bindings are treated as priority 0.
    /// </summary>
    /// <param name="list">Target list to insert into.</param>
    /// <param name="binding">Binding to insert.</param>
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

    /// <summary>
    /// Raises a global event to all registered global bindings.
    /// Invokes both the payload and no-arg callbacks on each binding.
    /// </summary>
    /// <param name="event">The event payload.</param>
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

    /// <summary>
    /// Raises an event to bindings registered under the given scope only.
    /// Invokes both the payload and no-arg callbacks on each scoped binding.
    /// </summary>
    /// <param name="scope">Scope key used during registration.</param>
    /// <param name="event">The event payload.</param>
    public static void RaiseSticky(T @event)
    {
        _stickyGlobal = @event;
        _hasStickyGlobal = true;
        Raise(@event);
    }

    /// <summary>
    /// Raises a global event and stores it as the current sticky value.
    /// New listeners that register via RegisterSticky will immediately receive it.
    /// </summary>
    /// <param name="event">The event payload to broadcast and store.</param>
    public static void RaiseStickyScoped(object scope, T @event)
    {
        if (scope == null) { RaiseSticky(@event); return; }
        _stickyScoped[scope] = (true, @event);
        RaiseScoped(scope, @event);
    }

    /// <summary>
    /// Raises a scoped event and stores it as the sticky value for that scope
    /// If scope is null, behaves like RaiseSticky(T)
    /// </summary>
    /// <param name="scope">Scope key to associate the sticky value with.</param>
    /// <param name="event">The event payload to broadcast and store.</param>
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

    /// <summary>
    /// Clears all listeners, all scopes, and all sticky values.
    /// </summary>
    static void Clear()
    {
        _global.Clear();
        _scoped.Clear();
        _stickyScoped.Clear();
        _hasStickyGlobal = false;
        _stickyGlobal = default;
    }

    public static int ListenerCountGlobal => _global.Count;

    /// <summary>
    /// Returns the number of listeners registered under the specified scope
    /// </summary>
    /// <param name="scope">Scope key to query.</param>
    /// <returns>Listener count within the scope; 0 if the scope is not present.</returns>
    public static int ListenerCountScoped(object scope) =>
        _scoped.TryGetValue(scope, out var l) ? l.Count : 0;
}