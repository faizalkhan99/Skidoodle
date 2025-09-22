using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A MonoBehaviour component that listens for a specific GameEvent.
/// When the GameEvent is raised, it invokes a UnityEvent response.
/// </summary>
public class GameEventListener : MonoBehaviour
{
    [Tooltip("The GameEvent channel to listen to.")]
    public GameEvent Event;

    [Tooltip("The response to invoke when the event is raised.")]
    public UnityEvent Response;

    private void OnEnable()
    {
        // When this component is enabled, register it with the GameEvent.
        if (Event != null)
        {
            Event.RegisterListener(this);
        }
    }

    private void OnDisable()
    {
        // When this component is disabled, unregister it.
        if (Event != null)
        {
            Event.UnregisterListener(this);
        }
    }

    /// <summary>
    /// This method is called by the GameEvent itself when it is raised.
    /// </summary>
    public void OnEventRaised()
    {
        Response.Invoke();
    }
}

