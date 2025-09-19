using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A reusable ScriptableObject that represents a parameter-less event.
/// Game components can reference this to either raise the event or listen to it.
/// </summary>
[CreateAssetMenu(menuName = "SDK/Events/Game Event")]
public class GameEvent : ScriptableObject
{
    // A list of all components that are listening to this event.
    private readonly List<GameEventListener> _listeners = new List<GameEventListener>();

    /// <summary>
    /// Call this method to broadcast the event to all listeners.
    /// </summary>
    public void Raise()
    {
        // Loop backwards in case a listener removes itself from the list.
        for (int i = _listeners.Count - 1; i >= 0; i--)
        {
            _listeners[i].OnEventRaised();
        }
    }

    public void RegisterListener(GameEventListener listener)
    {
        if (!_listeners.Contains(listener))
        {
            _listeners.Add(listener);
        }
    }

    public void UnregisterListener(GameEventListener listener)
    {
        if (_listeners.Contains(listener))
        {
            _listeners.Remove(listener);
        }
    }
}

