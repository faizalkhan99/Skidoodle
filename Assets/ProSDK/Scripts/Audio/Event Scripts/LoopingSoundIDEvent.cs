using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A ScriptableObject event channel that carries a LoopingSoundID payload.
/// </summary>
[CreateAssetMenu(menuName = "SDK/Events/Looping Sound ID Event")]
public class LoopingSoundIDEvent : ScriptableObject
{
    [System.NonSerialized]
    private List<LoopingSoundIDEventListener> _listeners = new List<LoopingSoundIDEventListener>();

    public void Raise(LoopingSoundID value)
    {
        for (int i = _listeners.Count - 1; i >= 0; i--)
        {
            _listeners[i].OnEventRaised(value);
        }
    }

    public void RegisterListener(LoopingSoundIDEventListener listener)
    {
        if (!_listeners.Contains(listener)) _listeners.Add(listener);
    }

    public void UnregisterListener(LoopingSoundIDEventListener listener)
    {
        if (_listeners.Contains(listener)) _listeners.Remove(listener);
    }
}
