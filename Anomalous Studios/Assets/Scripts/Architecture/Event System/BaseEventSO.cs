using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEventSO<TEvent> : ScriptableObject where TEvent : IEvent
{
    public System.Type EventType => typeof(TEvent);

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

    #region Publish Methods
    public void Publish(TEvent e)
    {
        EventBus<TEvent>.Raise(e);
    }

    public void PublishSticky(TEvent e)
    {
        EventBus<TEvent>.RaiseSticky(e);
    }

    public void PublishToConfiguredScope(TEvent e)
    {
        var scope = ScopeRegistry.Get(scopeKey);
        if (scope == null) EventBus<TEvent>.Raise(e);
        else EventBus<TEvent>.RaiseScoped(scope, e);
    }

    public void PublishStickyToConfiguredScope(TEvent e)
    {
        var scope = ScopeRegistry.Get(scopeKey);
        if (scope == null) EventBus<TEvent>.RaiseSticky(e);
        else EventBus<TEvent>.RaiseStickyScoped(scope, e);
    }

    public void PublishToScope(object scope, TEvent e)
    {
        if (scope == null) { Publish(e); return; }
        EventBus<TEvent>.RaiseScoped(scope, e);
    }

    public void PublishStickyToScope(object scope, TEvent e)
    {
        if (scope == null) { PublishSticky(e); return; }
        EventBus<TEvent>.RaiseStickyScoped(scope, e);
    }

    public void PublishToScopeKey(string key, TEvent e)
    {
        PublishToScope(ScopeRegistry.Get(key), e);
    }

    public void PublishStickyToScopeKey(string key, TEvent e)
    {
        PublishStickyToScope(ScopeRegistry.Get(key), e);
    }
    #endregion
}