using UnityEngine;

/// <summary>
/// A utility script that prevents a GameObject from being destroyed on scene loads.
/// This creates a persistent object that will exist across all scenes.
/// It also handles destroying duplicate instances if the scene containing
/// this object is accidentally loaded again.
/// </summary>
public class PersistentObject : MonoBehaviour
{
    private void Awake()
    {
        // This line is the core of the script. It tells Unity to move this
        // GameObject to a special, separate scene that doesn't get unloaded.
        DontDestroyOnLoad(gameObject);
    }
}