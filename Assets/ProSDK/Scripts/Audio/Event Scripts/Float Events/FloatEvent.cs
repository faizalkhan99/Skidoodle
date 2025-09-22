using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A ScriptableObject event channel that carries a float payload.
/// This is perfect for sending continuous data like health, progress, or RPM.
/// </summary>
[CreateAssetMenu(menuName = "SDK/Events/Float Event")]
public class FloatEvent : ScriptableObject
{
    [System.NonSerialized]
    private readonly List<FloatEventListener> _listeners = new List<FloatEventListener>();

    public void Raise(float value)
    {
        for (int i = _listeners.Count - 1; i >= 0; i--)
        {
            _listeners[i].OnEventRaised(value);
        }
    }

    public void RegisterListener(FloatEventListener listener)
    {
        if (!_listeners.Contains(listener)) _listeners.Add(listener);
    }

    public void UnregisterListener(FloatEventListener listener)
    {
        if (_listeners.Contains(listener)) _listeners.Remove(listener);
    }
}
