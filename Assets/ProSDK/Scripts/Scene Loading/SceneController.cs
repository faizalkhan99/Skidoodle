using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// A persistent, global manager responsible for loading scenes.
/// It automatically loads an initial scene on startup and then listens for events.
/// </summary>
public class SceneController : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField]
    [Tooltip("The name of the scene to load as soon as the game starts.")]
    private string _initialSceneToLoad = "MainMenu"; // Make sure this matches your scene name exactly!

    [Header("Loading Screen")]
    [SerializeField] private GameObject _loadingScreenPanel;

    private string _currentSceneName;

    private void Start()
    {
        // Hide the loading screen at the start.
        if (_loadingScreenPanel != null)
        {
            _loadingScreenPanel.SetActive(false);
        }

        // --- THE FIX ---
        // Instead of trying to get a scene that doesn't exist, we immediately
        // start the process of loading our very first scene.
        StartCoroutine(LoadSceneRoutine(_initialSceneToLoad));
    }

    /// <summary>
    /// This public method is the entry point for other systems to request a scene load.
    /// It's called by the StringEventListener.
    /// </summary>
    public void LoadNewScene(string sceneName)
    {
        // Prevent loading the same scene again
        if (sceneName == _currentSceneName)
        {
            Debug.LogWarning($"[SceneController] Tried to load '{sceneName}', but it's already the current scene. Aborting.");
            return;
        }
        Debug.Log($"[SceneController] Received request. Starting LoadSceneRoutine for '{sceneName}'.");
        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        if (_loadingScreenPanel != null)
        {
            _loadingScreenPanel.SetActive(true);
        }

        // Unload the old scene ONLY if one exists.
        if (!string.IsNullOrEmpty(_currentSceneName))
        {
            yield return SceneManager.UnloadSceneAsync(_currentSceneName);
        }

        // Add a small delay for a smoother visual transition, especially on startup.
        yield return new WaitForSeconds(0.5f);

        // Load the new scene additively.
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        while (!operation.isDone)
        {
            yield return null;
        }

        _currentSceneName = sceneName;
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));

        if (_loadingScreenPanel != null)
        {
            _loadingScreenPanel.SetActive(false);
        }
    }
}

