using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A MonoBehaviour component that listens to a specific FloatEvent channel.
/// When the event is raised, it invokes a UnityEvent response, passing the float value.
/// </summary>
public class FloatEventListener : MonoBehaviour
{
    [Tooltip("The event channel to listen to.")]
    public FloatEvent Event;

    [Tooltip("The response to invoke when the event is raised.")]
    public UnityEvent<float> Response;

    private void OnEnable()
    {
        if (Event != null) Event.RegisterListener(this);
    }

    private void OnDisable()
    {
        if (Event != null) Event.UnregisterListener(this);
    }

    public void OnEventRaised(float value)
    {
        Response.Invoke(value);
    }
}
