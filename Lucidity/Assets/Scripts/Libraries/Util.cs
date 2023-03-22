using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Util {
    
    /// <summary>
    /// Resets all static variables in MapEditorManager and Tool. This is necessary to avoid 
    /// interference between tests, or when MapEditor is reloaded.
    /// </summary>
    public static void ResetStaticVariables() {
        MapEditorManager.MapObjects.Clear();
        MapEditorManager.Layers.Clear();
        MapEditorManager.Actions = null;
        MapEditorManager.CurrentAction = null;
        MapEditorManager.ToolToCursorMap.Clear();
        MapEditorManager.Map = null;
        MapEditorManager.MapContainer = null;
        MapEditorManager.SpawnPoint = Vector2.zero;
        MapEditorManager.ReloadFlag = false;
        MapEditorManager.CurrentButtonPressed = 0;
        AssetOptions.AssetCount = 1;
        AssetOptions.Spread = 1;
        Tool.ToolKeys.Clear();
        Tool.ToolStatus.Clear();
        Tool.PaintingMenu = null;
        Tool.SelectionMenu = null;
        Tool.SelectionOptions = null;
        Tool.SpawnPointOptions = null;
        Mouse.LastMousePosition = Vector2.zero;
        Layer.LayerContainer = null;
        Layer.LayerIndex.Clear();
        Layer.LayerStatus.Clear();
        Layer.LayerToBeNamed = -1;
        Layer.LayerNames.Clear();
        StartupScreen.FilePath = null;
        SelectMapObject.SelectedObject = null;
    }

    /// <summary>
    /// Resets all asset buttons in MapEditorManager. This is necessary to avoid issues with old
    /// buttons being referenced when MapEditor is reloaded.
    /// </summary>
    public static void ResetAssetButtons() {
        MapEditorManager editor = GameObject.Find("MapEditorManager")
            .GetComponent<MapEditorManager>();
        editor.AssetButtons.Clear();
        GameObject[] paintButtons = GameObject.FindGameObjectsWithTag("PaintButton");
        paintButtons = paintButtons.OrderBy(x => x.name.ToLower()).ToArray();
        foreach (GameObject paintButton in paintButtons) {
            paintButton.GetComponent<AssetController>().GetInstanceID();
            editor.AssetButtons.Add(paintButton.GetComponent<AssetController>());
        }
    }
}
