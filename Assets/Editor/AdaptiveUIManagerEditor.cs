// IMPORTANT: This script MUST be placed in a folder named "Editor".
using UnityEditor;
using UnityEngine;

// This attribute tells Unity that this script is a custom editor for the AdaptiveUIManager.
[CustomEditor(typeof(UIManager))]
public class AdaptiveUIManagerEditor : Editor
{
    // This is the method that draws the custom inspector.
    public override void OnInspectorGUI()
    {
        // Update the serialized object's representation.
        serializedObject.Update();

        // Get a reference to the script we are inspecting.
        var uiManager = (UIManager)target;

        // Draw the enum dropdown for UIType.
        EditorGUILayout.PropertyField(serializedObject.FindProperty("currentUIType"));

        // Based on the selected enum value, show different properties.
        switch (uiManager.currentUIType)
        {
            case UIManager.UIType.MainMenu:
                // Show only the Main Menu related fields.
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Main Menu Properties", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("mainMenuPanel"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("gameModeSelectPanel"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("creditsPanel"));
                break;

            case UIManager.UIType.GameUI:
                // Show only the Game UI related fields.
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Game UI Properties", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("pauseMenuPanel"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("pauseButtonPanel"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("gameOverPanel"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("scoreText"));
                break;
        }

        // --- Always show the Event fields ---
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Event Broadcasting", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("onSceneLoadRequest"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("onGamePausedRequest"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("onGameResumedRequest"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("onPlayButtonClickSoundRequest"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("onQuitRequest"));

        // Apply any changes the user has made in the inspector.
        serializedObject.ApplyModifiedProperties();
    }
}
