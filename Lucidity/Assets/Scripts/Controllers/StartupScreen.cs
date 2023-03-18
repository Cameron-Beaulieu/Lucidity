using SimpleFileBrowser;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartupScreen : MonoBehaviour {
    [SerializeField] private Button loadMapButton;
    [SerializeField] private Button newMapButton;
    public static string FilePath;
    public static bool IsTesting = false;

    private void Start() {
        newMapButton.onClick.AddListener(NewMapClickHandler);
        loadMapButton.onClick.AddListener(LoadMapClickHandler);
    }

    /// <summary>
    /// Button handler for <c>loadMapButton</c>, selected through in the Unity editor.
    /// </summary>
    public static void LoadMapClickHandler() {
        if (IsTesting) {
            SceneManager.LoadScene("MapEditor", LoadSceneMode.Single);
            return;
        }

        FileBrowser.SetFilters(true, new FileBrowser.Filter("JSON", ".json"));
        FileBrowser.SetDefaultFilter( ".json" );

        FileBrowser.ShowLoadDialog( (paths) => { ValidatePath(paths[0]); }, null, FileBrowser.PickMode.Files, false, null,
            null, "Select File", "Select" );
    }

    /// <summary>
    /// Checks that the file path passed in is valid and loads the map editor scene if so.
    /// </summary>
    /// <param name="path">The path to the file to load.</param>
    private static void ValidatePath(string path) {
        // cancelled selecting a path
        if (path.Equals("")) { return; }

        // Guarantee the file is JSON
        if (!path.Substring(Math.Max(0, path.Length - 5)).Equals(".json")) {
            GameObject.Find("ErrorMessage").GetComponent<TMP_Text>().text = "You can only load a map as a JSON file.";
            return;
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
