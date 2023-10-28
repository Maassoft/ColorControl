using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static ColorControl.Services.EventDispatcher.EventExtensions;

namespace ColorControl.Services.EventDispatcher;

public class EventDispatcher<T> where T : EventArgs
{
    private Dictionary<string, EventHandler<T>> eventHandlers = new Dictionary<string, EventHandler<T>>();
    private Dictionary<string, AsyncEventHandler<T>> asyncEventHandlers = new Dictionary<string, AsyncEventHandler<T>>();

    public void RegisterEventHandler(string eventName, EventHandler<T> eventHandler)
    {
        if (!eventHandlers.ContainsKey(eventName))
        {
            eventHandlers.Add(eventName, eventHandler);
        }
        else
        {
            eventHandlers[eventName] += eventHandler;
        }
    }

    public void RegisterAsyncEventHandler(string eventName, AsyncEventHandler<T> eventHandler)
    {
        if (!asyncEventHandlers.ContainsKey(eventName))
        {
            asyncEventHandlers.Add(eventName, eventHandler);
        }
        else
        {
            asyncEventHandlers[eventName] += eventHandler;
        }
    }

    public void UnregisterEventHandler(string eventName)
    {
        eventHandlers.Remove(eventName);
    }

    public void DispatchEvent(string eventName, T eventArgs)
    {
        if (eventHandlers.ContainsKey(eventName))
        {
            eventHandlers[eventName]?.Invoke(this, eventArgs);
        }
    }

    public async Task DispatchEventAsync(string eventName, T eventArgs)
    {
        if (asyncEventHandlers.ContainsKey(eventName))
        {
            await asyncEventHandlers[eventName]?.InvokeAsync(this, eventArgs);
        }
    }

    protected bool HasHandlers(string eventName)
    {
        return eventHandlers.ContainsKey(eventName) || asyncEventHandlers.ContainsKey(eventName);
    }
}
