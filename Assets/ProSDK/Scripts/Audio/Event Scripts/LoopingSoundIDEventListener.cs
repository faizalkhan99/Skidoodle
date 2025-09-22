using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A MonoBehaviour component that listens to a LoopingSoundIDEvent channel.
/// When the event is raised, it invokes a UnityEvent response, passing the ID value.
/// </summary>
public class LoopingSoundIDEventListener : MonoBehaviour
{
    [Tooltip("The event channel to listen to.")]
    public LoopingSoundIDEvent Event;

    [Tooltip("The response to invoke when the event is raised.")]
    public UnityEvent<LoopingSoundID> Response;

    private void OnEnable()
    {
        if (Event != null) Event.RegisterListener(this);
    }

    private void OnDisable()
    {
        if (Event != null) Event.UnregisterListener(this);
    }

    public void OnEventRaised(LoopingSoundID value)
    {
        Response.Invoke(value);
    }
}
