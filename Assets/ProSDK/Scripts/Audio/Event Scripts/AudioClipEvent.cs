using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A ScriptableObject event channel that carries an AudioClip payload.
/// </summary>
[CreateAssetMenu(menuName = "SDK/Events/AudioClip Event")]
public class AudioClipEvent : ScriptableObject
{
    [System.NonSerialized]
    private readonly List<AudioClipEventListener> _listeners = new List<AudioClipEventListener>();

    public void Raise(AudioClip value)
    {
        for (int i = _listeners.Count - 1; i >= 0; i--)
        {
            _listeners[i].OnEventRaised(value);
        }
    }

    public void RegisterListener(AudioClipEventListener listener)
    {
        if (!_listeners.Contains(listener)) _listeners.Add(listener);
    }

    public void UnregisterListener(AudioClipEventListener listener)
    {
        if (_listeners.Contains(listener)) _listeners.Remove(listener);
    }
}
