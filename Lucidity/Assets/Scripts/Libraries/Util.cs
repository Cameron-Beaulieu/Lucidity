using System.Collections;
using System.Collections.Generic;
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
        MapEditorManager.CurrentButtonPressed = 0;
        Tool.ToolKeys.Clear();
        Tool.ToolStatus.Clear();
        Tool.PaintingMenu = null;
        Tool.SelectionMenu = null;
        Tool.SelectionOptions = null;
        Tool.SpawnPointOptions = null;
        Mouse.LastMousePosition = Vector2.zero;
        Layer.LayerStatus.Clear();
        Layer.LayerIndex.Clear();
        Layer.LayerNames.Clear();
    }
}
