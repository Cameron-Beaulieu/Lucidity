using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util {
    
    /// <summary>
    /// Resets all static variables in MapEditorManager and Tool. This is necessary to avoid 
    /// interference between tests, or when MapEditor is reloaded.
    /// </summary>
    public static void ResetStaticVariables() {
        AssetCollision.LayerCollisions.Clear();
        MapEditorManager.MapObjects.Clear();
        MapEditorManager.Layers.Clear();
        MapEditorManager.Actions = null;
        MapEditorManager.CurrentAction = null;
        MapEditorManager.ToolToCursorMap.Clear();
        MapEditorManager.Map = null;
        MapEditorManager.MapContainer = null;
        MapEditorManager.SpawnPoint = Vector2.zero;
        MapEditorManager.CurrentButtonPressed = 0;
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
    }

    /// <summary>
    /// Resets all asset buttons in MapEditorManager. This is necessary to avoid issues with old
    /// buttons being referenced when MapEditor is reloaded.
    /// </summary>
    public static void ResetAssetButtons() {
        MapEditorManager editor = GameObject.Find("MapEditorManager")
            .GetComponent<MapEditorManager>();
        editor.AssetButtons.Clear();
        foreach (GameObject paintButton in GameObject
                .FindGameObjectsWithTag("PaintButton")) {
            editor.AssetButtons.Add(paintButton.GetComponent<AssetController>());
        }
    }
}
