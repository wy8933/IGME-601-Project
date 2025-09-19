using System;
using System.Collections.Generic;
using UnityEngine;

public interface IEvent { }

public static class EventBus<T> where T : IEvent
{
    static readonly HashSet<IEventBinding<T>> bindings = new HashSet<IEventBinding<T>>();

    public static void Register(EventBinding<T> binding) => bindings.Add(binding);
    public static void DeRegister(EventBinding<T> binding) => bindings.Remove(binding);

    /// <summary>
    /// Accept an event and active both of the action on the binding
    /// </summary>
    /// <param name="event"></param>
    public static void Raise(T @event) 
    {
        foreach (var binding in bindings) 
        {
            // Normally there will only be one, but both are init as empty delegate, so it's safe to call both
            binding.OnEvent.Invoke(@event);
            binding.OnEventNoArgs.Invoke();
        }
    }
}
