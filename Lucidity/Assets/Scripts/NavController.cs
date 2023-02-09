using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class NavController : MonoBehaviour {
	/// <summary>
	/// Click handler for the New button in the file menu.
	/// </summary>
    public void NewButtonClickHandler() {
        Debug.Log("New button clicked");
    }

	/// <summary>
	/// Click handler for the Open button in the file menu.
	/// </summary>
    [MenuItem("NavMenu/Open %&o")]
    public static void OpenButtonClickHandler() {
        Debug.Log("Open button clicked");
    }

	/// <summary>
	/// Click handler for the Save button in the file menu.
	/// This method can be triggered by the keyboard shortcut CTRL/CMD + ALT + S
	/// </summary>
    [MenuItem("NavMenu/Save %&s")]
    public static void SaveButtonClickHandler() {
		// This case should only really be possible in development, not in production
		if (MapData.FileName == null) {
			SaveAsButtonClickHandler();
		} else {
			SaveFile();
		}
    }

	/// <summary>
	/// Click handler for the Save As button in the File menu.
	/// This method can be triggered by the keyboard shortcut CTRL/CMD + SHIFT + ALT + S
	/// </summary>
	[MenuItem("NavMenu/SaveAs %#&s")]
    public static void SaveAsButtonClickHandler() {
        // The second argument to SaveFilePanel can eventually be replaced with the user's default
		// map file location
		string path = EditorUtility.SaveFilePanel("Select Directory", "", "Untitled.json", "json");
        // cancelled selecting a path
        if (path.Equals("")) { return; }

		// Guarantee the file is JSON
		while (!path.Substring(Math.Max(0, path.Length - 5)).Equals(".json")) {
			bool tryAgain = EditorUtility.DisplayDialog(
				"Invalid file name", "Your map can only be saved as a JSON file.", "Try again", 
				"Cancel");
			if (!tryAgain) {return;}

			path = EditorUtility.SaveFilePanel("Select Directory", "", "Untitled.json", "json");
		}

		MapData.FileName = path;

        SaveFile();
    }

	/// <summary>
	/// Click handler for the Export button in the File menu
	/// </summary>
    public void ExportButtonClickHandler() {
        Debug.Log("Export button clicked");
    }

	/// <summary>
	/// Saves the current state of the map to a json file.
	/// </summary>
	private static void SaveFile() {
		string groundColour = ColorUtility.ToHtmlStringRGB(
			MapEditorManager.Map.GetComponent<Image>().color).ToLower();
        CreateNewMap.SizeType size = CreateNewMap.Size;

        MapData jsonContent = new MapData(size, new Biome(groundColour), 
										  MapEditorManager.MapObjects);

        File.WriteAllText(MapData.FileName, jsonContent.Serialize());   
	}
}