using UnityEngine;

/// <summary>
/// A lightweight controller placed in each scene to define its audio.
/// On start, it raises an event to tell the persistent AudioManager what BGM to play.
/// </summary>
public class SceneAudioController : MonoBehaviour
{
    [Header("Background Music")]
    [SerializeField]
    [Tooltip("The BGM for this scene. Can be null if no BGM is desired.")]
    private AudioClip _sceneBGM;

    [Header("Event Channel")]
    [SerializeField]
    [Tooltip("The event channel to broadcast the BGM change request on.")]
    private AudioClipEvent _onChangeBGMRequest;

    void Start()
{
    if (_onChangeBGMRequest != null && _sceneBGM != null)
    {
        // This log will appear in the console if the script is working correctly.
        Debug.Log($"<color=cyan>[SceneAudioController] Firing OnChangeBGMRequest with clip: {_sceneBGM.name}</color>");
        _onChangeBGMRequest.Raise(_sceneBGM);
    }
    else
    {
        // This error will appear if a reference is missing in the Inspector.
        Debug.LogError("<color=orange>[SceneAudioController] Event or AudioClip is missing! Cannot request BGM.</color>", this.gameObject);
    }
}
}