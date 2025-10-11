using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEventSO<TEvent> : ScriptableObject where TEvent : IEvent
{
    [Header("Subscription")]
    [Tooltip("Higher number runs earlier among listeners.")]
    [SerializeField] private int priority = 0;
    [Tooltip("If true, RegisterSticky is used so the last value is replayed immediately on enable.")]
    [SerializeField] private bool sticky = false;
    [Tooltip("Leave empty for global subscription.")]
    [SerializeField] private string scopeKey = "";

    private EventBinding<TEvent> _binding;
    private bool _registered;

    private static class ScopeRegistry
    {
        private static readonly Dictionary<string, object> _map = new Dictionary<string, object>();
        public static object Get(string key)
        {
            if (string.IsNullOrEmpty(key)) return null;
            if (!_map.TryGetValue(key, out var obj))
            {
                obj = new object();
                _map[key] = obj;
            }
            return obj;
        }
    }

    protected virtual void OnEnable()
    {
        if (_registered) return;

        _binding = new EventBinding<TEvent>(OnEvent, priority);
        
        _binding.Add(OnEventNoArgs);

        var scope = ScopeRegistry.Get(scopeKey);
        if (sticky)
        {
            
            EventBus<TEvent>.RegisterSticky(_binding, global: scope == null, scope: scope);
        }
        else
        {
            if (scope == null)
                EventBus<TEvent>.Register(_binding);
            else
                EventBus<TEvent>.Register(_binding, scope: scope);
        }

        _registered = true;
    }

    protected virtual void OnDisable()
    {
        if (!_registered) return;

        var scope = ScopeRegistry.Get(scopeKey);
        if (scope == null)
            EventBus<TEvent>.DeRegister(_binding);
        else
            EventBus<TEvent>.DeRegister(_binding, scope: scope);

        _registered = false;
    }

    protected abstract void OnEvent(TEvent e);

    protected virtual void OnEventNoArgs() { }
}