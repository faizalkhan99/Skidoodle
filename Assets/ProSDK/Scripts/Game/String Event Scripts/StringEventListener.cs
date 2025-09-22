using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A MonoBehaviour component that listens to a specific StringEvent channel.
/// When the event is raised, it invokes a UnityEvent response, passing the string value.
/// </summary>
public class StringEventListener : MonoBehaviour
{
    [Tooltip("The event channel to listen to.")]
    public StringEvent Event;

    [Tooltip("The response to invoke when the event is raised.")]
    public UnityEvent<string> Response;

    private void OnEnable()
    {
        // Subscribe to the event when this component is enabled.
        if (Event != null)
        {
            Event.RegisterListener(this);
        }
    }

    private void OnDisable()
    {
        // Unsubscribe from the event when this component is disabled.
        if (Event != null)
        {
            Event.UnregisterListener(this);
        }
    }

    /// <summary>
    /// This method is called by the StringEvent when it is raised.
    /// </summary>
    public void OnEventRaised(string value)
    {
        Debug.Log($"[StringEventListener]: {value}");
        Response.Invoke(value);
    }
}

