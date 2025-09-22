using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A MonoBehaviour component that listens to a specific AudioClipEvent channel.
/// </summary>
public class AudioClipEventListener : MonoBehaviour
{
    [Tooltip("The event channel to listen to.")]
    public AudioClipEvent Event;

    [Tooltip("The response to invoke when the event is raised.")]
    public UnityEvent<AudioClip> Response;

    private void OnEnable()
    {
        if (Event != null) Event.RegisterListener(this);
    }

    private void OnDisable()
    {
        if (Event != null) Event.UnregisterListener(this);
    }

    public void OnEventRaised(AudioClip value)
    {
        Response.Invoke(value);
    }
}
