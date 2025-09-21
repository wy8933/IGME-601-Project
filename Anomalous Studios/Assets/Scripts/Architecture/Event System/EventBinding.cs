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

    public EventBinding(Action<T> onEvent) => _onEvent = onEvent ?? (_ => { });
    public EventBinding(Action onEventNoArgs) => _onEventNoArgs = onEventNoArgs ?? (() => { });
    public EventBinding(Action<T> onEvent, int pr) { _onEvent = onEvent ?? (_ => { }); Priority = pr; }

    public void Add(Action<T> onEvent) => _onEvent += onEvent;
    public void Add(Action onEvent) => _onEventNoArgs += onEvent;
    public void Remove(Action<T> f) => _onEvent -= f;
    public void Remove(Action f) => _onEventNoArgs -= f;

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        EventBus<T>.DeRegister(this);
    }

}