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
    private static TMP_Text _savingText;
    private static float _hideTextTimer = 0f;
    private static GameObject _modal;
    private static GameObject _modalButtons;
    private static TMP_Text _modalText;
    private static string _saveBeforeLoadPrompt = "Would you like to save your current map before opening a new one?";
    private static string _saveBeforeNewPrompt = "Would you like to save your current map before creating a new one?";

    private void Start() {
        _savingText = GameObject.Find("Saving Text").GetComponent<TMP_Text>();

        // add listeners to save modal option buttons
        _modal = GameObject.Find("SaveModal");
        _modalText = _modal.transform.Find("Text (TMP)").GetComponent<TMP_Text>();
        _modalButtons = _modal.transform.Find("Button Group").gameObject;
        // yes button
        _modalButtons.transform.Find("Yes Button").gameObject.GetComponent<Button>().onClick
            .AddListener(() => {
            _modal.SetActive(false);
            SaveButtonClickHandler();
            if (_modalText.text == _saveBeforeLoadPrompt) {
                StartupScreen.LoadMapClickHandler();
            } else if (_modalText.text == _saveBeforeNewPrompt) {
                SceneManager.LoadScene("MapCreation", LoadSceneMode.Single);
            }
        });
        // no button
        _modalButtons.transform.Find("No Button").gameObject.GetComponent<Button>().onClick
            .AddListener(() => {
            _modal.SetActive(false);
            if (_modalText.text == _saveBeforeLoadPrompt) {
                StartupScreen.LoadMapClickHandler();
            } else if (_modalText.text == _saveBeforeNewPrompt) {
                SceneManager.LoadScene("MapCreation", LoadSceneMode.Single);
            }
        });
        // cancel button
        _modalButtons.transform.Find("Cancel Button").gameObject.GetComponent<Button>().onClick
            .AddListener(() => {
            _modal.SetActive(false);
        });
        _modal.SetActive(false); // hide modal by default
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
        _modalText.text = _saveBeforeNewPrompt;
        _modal.SetActive(true);
        
    }

    /// <summary>
    /// Click handler for the Open button in the file menu.
    /// This method can be triggered by the keyboard shortcut CTRL/CMD + ALT + O
    /// </summary>
    public static void OpenButtonClickHandler() {
        _modalText.text = _saveBeforeLoadPrompt;
        _modal.SetActive(true);
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
        FileBrowser.ShowSaveDialog((paths) => { ValidateSave(paths[0]); }, null, 
                                   FileBrowser.PickMode.Files, false, null, "Untitled.json", 
                                   "Save Map", "Save");
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