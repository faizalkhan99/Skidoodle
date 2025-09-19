using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// A modular, event-driven UI Manager.
/// It has no singleton and holds no references to other managers.
/// Its public methods are designed to be connected to Event Listeners in the Inspector.
/// </summary>
public class UIManager : MonoBehaviour
{
    // Enum to define the context of the UI in the Inspector.
    public enum UIType
    {
        MainMenu,
        GameUI
        // You can add more types here like GameOver, StoreUI, etc.
    }

    [Tooltip("Select the purpose of this UI instance.")]
    public UIType currentUIType;

    // --- Main Menu Fields ---
    [Header("Main Menu Panels")]
    public GameObject mainMenuPanel;
    public GameObject gameModeSelectPanel;
    public GameObject creditsPanel;

    // --- Game UI Fields ---
    [Header("Game UI Panels")]
    public GameObject pauseMenuPanel;
    public GameObject pauseButtonPanel;
    public GameObject gameOverPanel;

    [Header("Game HUD Elements")]
    public TextMeshProUGUI scoreText;

    // --- Event Broadcasting ---
    [Header("Events to Broadcast")]
    public StringEvent onSceneLoadRequest;
    public GameEvent onGamePausedRequest;
    public GameEvent onGameResumedRequest;
    public GameEvent onPlayButtonClickSoundRequest;
    public GameEvent onQuitRequest;


    // You can keep all your public methods here. They will be available
    // regardless of the UI type selected, but you will only hook up the
    // ones you need for that specific UI prefab.

    public void RequestSceneLoad(string sceneName)
    {
        
        Debug.Log($"[UIManager] Requesting to load scene: {sceneName}");
        
        if (onSceneLoadRequest != null)
        {
            onSceneLoadRequest.Raise(sceneName);
        }
    }

    public void RequestPause()
    {
        onGamePausedRequest?.Raise();
        if (pauseMenuPanel) pauseMenuPanel.SetActive(true);
        if (pauseButtonPanel) pauseButtonPanel.SetActive(false);
        PlayButtonClickSound();
    }

    public void RequestResume()
    {
        onGameResumedRequest?.Raise();
        if (pauseMenuPanel) pauseMenuPanel.SetActive(false);
        if (pauseButtonPanel) pauseButtonPanel.SetActive(true);
        PlayButtonClickSound();
    }
    
    public void RequestQuitGame()
    {
        onQuitRequest?.Raise();
        PlayButtonClickSound();
    }

    public void UpdateScore(int newScore)
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + newScore.ToString();
        }
    }
    
    public void OnGameOver(string reason)
    {
        if (gameOverPanel) gameOverPanel.SetActive(true);
        if (pauseButtonPanel) pauseButtonPanel.SetActive(false);
        if (pauseMenuPanel) pauseMenuPanel.SetActive(false);
    }
    
    private void PlayButtonClickSound()
    {
        onPlayButtonClickSoundRequest?.Raise();
    }
}
