using SimpleFileBrowser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NavController : MonoBehaviour {
    [SerializeField] private TMP_Text _newKeybind;
    [SerializeField] private TMP_Text _openKeybind;
    [SerializeField] private TMP_Text _saveKeybind;
    [SerializeField] private TMP_Text _saveAsKeybind;
    [SerializeField] private TMP_Text _exportKeybind;
    private static TMP_Text _savingText;
    private static float _hideTextTimer = 0f;

    private void Start() {
        if (Application.platform == RuntimePlatform.OSXPlayer || 
            Application.platform == RuntimePlatform.OSXEditor) {
            _newKeybind.text = "Cmd + Opt + N";
            _openKeybind.text = "Cmd + Opt + O";
            _saveKeybind.text = "Cmd + Opt + S";
            _saveAsKeybind.text = "Cmd + Shift + Opt + S";
            _exportKeybind.text = "Cmd + Opt + E";
        }
        _savingText = GameObject.Find("Saving Text").GetComponent<TMP_Text>();
    }

    private void Update() {
        if (_savingText.text == "Saved!" && Time.time > _hideTextTimer) {
            _savingText.text = "";
        }
    }

    /// <summary>
    /// Click handler for the New button in the file menu.
    /// This method can be triggered by the keyboard shortcut CTRL/CMD + ALT + N
    /// </summary>
    public static void NewButtonClickHandler() {
        Debug.Log("New button clicked");
        // TODO: Navigate to the MapCreation scene
    }

    /// <summary>
    /// Click handler for the Open button in the file menu.
    /// This method can be triggered by the keyboard shortcut CTRL/CMD + ALT + O
    /// </summary>
    public static void OpenButtonClickHandler() {
        // Yes = 0, Cancel = 1, No = 2
        // int savePrev = EditorUtility.DisplayDialogComplex(
        //     "Save current map?", 
        //     "Would you like to save your current map before opening a new one?", "Yes", 
        //     "Cancel", "No");

        // switch (savePrev) {
        //     case 0:
        //         SaveButtonClickHandler();
        //         StartupScreen.LoadMapClickHandler();
        //         break;
        //     case 1:
        //         return;
        //     case 2:
        //         StartupScreen.LoadMapClickHandler();
        //         break;
        // }
        StartupScreen.LoadMapClickHandler();
    }

    /// <summary>
    /// Click handler for the Save button in the file menu.
    /// This method can be triggered by the keyboard shortcut CTRL/CMD + ALT + S
    /// </summary>
    public static void SaveButtonClickHandler() {
        // This case should only really be possible in development, not in production
        if (MapData.FileName == null) {
            SaveAsButtonClickHandler();
        } else {
            _savingText.text = "Saving...";
            SaveFile();
        }
    }

    /// <summary>
    /// Click handler for the Save As button in the File menu.
    /// This method can be triggered by the keyboard shortcut CTRL/CMD + SHIFT + ALT + S
    /// </summary>
    public static void SaveAsButtonClickHandler() {
        // The second argument to SaveFilePanel can eventually be replaced with the user's default
        // map file location
        FileBrowser.SetFilters(true, new FileBrowser.Filter("JSON", ".json"));
        FileBrowser.SetDefaultFilter(".json");
        FileBrowser.ShowSaveDialog( (paths) => { ValidateSave(paths[0]); }, null, FileBrowser.PickMode.Files, false, null, "Untitled.json", "Save Map", "Save" );
    }

    private static void ValidateSave(string path) {
        // cancelled selecting a path
        if (path.Equals("")) { return; }
        _savingText.text = "Saving...";

        // Guarantee the file is JSON
        if (!path.Substring(Math.Max(0, path.Length - 5)).Equals(".json")) {
            return;
        }

        MapData.FileName = path;

        SaveFile();
    }

    /// <summary>
    /// Click handler for the Export button in the File menu
    /// This method can be triggered by the keyboard shortcut CTRL/CMD + ALT + E
    /// </summary>
    public static void ExportButtonClickHandler() {
        Debug.Log("Export button clicked");
    }

    /// <summary>
    /// Saves the current state of the map to a json file.
    /// </summary>
    private static void SaveFile() {
        string groundColour = ColorUtility.ToHtmlStringRGB(
            MapEditorManager.Map.GetComponent<Image>().color).ToLower();

        MapData jsonContent = new MapData(new Biome(groundColour), MapEditorManager.SpawnPoint, 
                                          MapEditorManager.Layers, Layer.LayerIndex);

        File.WriteAllText(MapData.FileName, jsonContent.Serialize());
        _savingText.text = "Saved!";
        _hideTextTimer = Time.time + 3;
    }
}