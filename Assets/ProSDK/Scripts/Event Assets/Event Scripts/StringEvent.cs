using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A ScriptableObject event channel that carries a string payload.
/// </summary>
[CreateAssetMenu(menuName = "SDK/Events/String Event")]
public class StringEvent : ScriptableObject
{
    // By marking this as NonSerialized, we ensure that the list of listeners
    // is treated as runtime-only data. It will be reset automatically when
    // you enter Play Mode in the editor, preventing errors from stale or null
    // references that can persist from a previous session.
    [System.NonSerialized]
    private List<StringEventListener> _listeners = new List<StringEventListener>();

    public void Raise(string value)
    {
        Debug.Log($"[StringEvent] : {value}");
        // Looping backwards is a good practice to avoid issues if a listener
        // unregisters itself during the event call.
        for (int i = _listeners.Count - 1; i >= 0; i--)
        {
            _listeners[i].OnEventRaised(value);
        }
    }

    public void RegisterListener(StringEventListener listener)
    {
        if (!_listeners.Contains(listener))
        {
            _listeners.Add(listener);
        }
    }

    public void UnregisterListener(StringEventListener listener)
    {
        if (_listeners.Contains(listener))
        {
            _listeners.Remove(listener);
        }
    }
}

