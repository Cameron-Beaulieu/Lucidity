using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartupScreen : MonoBehaviour {
    [SerializeField] private Button loadMapButton;
    [SerializeField] private Button newMapButton;
    public static string FilePath;

    private void Start() {
        newMapButton.onClick.AddListener(NewMapClickHandler);
        loadMapButton.onClick.AddListener(LoadMapClickHandler);
    }

    /// <summary>
    /// Button handler for <c>loadMapButton</c>, selected through in the Unity editor.
    /// </summary>
    public static void LoadMapClickHandler() {
        string path = EditorUtility.OpenFilePanel("Select File", "", "json");
        // cancelled selecting a path
        if (path.Equals("")) { return; }

        // Guarantee the file is JSON
        while (!path.Substring(Math.Max(0, path.Length - 5)).Equals(".json")) {
            bool tryAgain = EditorUtility.DisplayDialog(
                "Invalid file selection", "You can only load a map as a JSON file.", "Try again",
                "Cancel");
            if (!tryAgain) {return;}

            path = EditorUtility.OpenFilePanel("Select File", "", "json");
        }
        
        FilePath = path;

        SceneManager.LoadScene("MapEditor", LoadSceneMode.Single);
    }

    /// <summary>
    /// Button handler for <c>newMapButton</c>, selected through in the Unity editor.
    /// </summary>
    public void NewMapClickHandler() {
        SceneManager.LoadScene("MapCreation", LoadSceneMode.Single);
    }
}
