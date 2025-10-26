using System;

internal interface IEventBinding<T>
{
    public Action<T> OnEvent { get; set; }
    public Action OnEventNoArgs { get; set; }


}

public sealed class EventBinding<T> : IEventBinding<T>, IPrioritized, IDisposable where T : IEvent
{
    private Action<T> _onEvent = _ => { };
    private Action _onEventNoArgs = () => { };
    private bool _disposed;

    public int Priority { get; set; } = 0;

    Action<T> IEventBinding<T>.OnEvent { get => _onEvent; set => _onEvent = value ?? (_ => { }); }
    Action IEventBinding<T>.OnEventNoArgs { get => _onEventNoArgs; set => _onEventNoArgs = value ?? (() => { }); }

    /// <summary>
    /// Create a binding that listens to events with a payload.
    /// </summary>
    /// <param name="onEvent">Callback invoked with the event payload.</param>
    public EventBinding(Action<T> onEvent) => _onEvent = onEvent ?? (_ => { });

    /// <summary>
    /// Create a binding that listens to events without a payload.
    /// </summary>
    /// <param name="onEventNoArgs">Callback invoked with no arguments.</param>
    public EventBinding(Action onEventNoArgs) => _onEventNoArgs = onEventNoArgs ?? (() => { });

    /// <summary>
    /// Create a binding with a payload callback and a priority.
    /// Higher numbers run earlier if supported by the bus.
    /// </summary>
    /// <param name="onEvent">Callback invoked with the event payload.</param>
    /// <param name="pr">Priority value.</param>
    public EventBinding(Action<T> onEvent, int pr) { _onEvent = onEvent ?? (_ => { }); Priority = pr; }

    /// <summary>
    /// Add another payload callback to this binding.
    /// </summary>
    /// <param name="onEvent">Callback to add.</param>
    public void Add(Action<T> onEvent) => _onEvent += onEvent;

    /// <summary>
    /// Add another no-arg callback to this binding.
    /// </summary>
    /// <param name="onEvent">Callback to add.</param>
    public void Add(Action onEvent) => _onEventNoArgs += onEvent;

    /// <summary>
    /// Remove a payload callback from this binding.
    /// </summary>
    /// <param name="f">Callback to remove.</param>
    public void Remove(Action<T> f) => _onEvent -= f;

    /// <summary>
    /// Remove a no-arg callback from this binding.
    /// </summary>
    /// <param name="f">Callback to remove.</param>
    public void Remove(Action f) => _onEventNoArgs -= f;

    /// <summary>
    /// Unregister this binding from the event bus.
    /// Safe to call multiple times.
    /// </summary>
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        EventBus<T>.DeRegister(this);
    }

}