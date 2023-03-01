using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util {
    /// <summary>
    /// Resets all static variables in MapEditorManager and Tool. This is necessary to avoid 
    /// interference between tests, or when MapEditor is reloaded.
    /// </summary>
    public static void ResetStaticVariables() {
        MapEditorManager.MapObjects = new Dictionary<int, MapObject>();
        MapEditorManager.Layers = new List<Dictionary<int, MapObject>>();
        MapEditorManager.Actions = null;
        MapEditorManager.CurrentAction = null;
        MapEditorManager.ToolToCursorMap = new Dictionary<string, Texture2D>();
        MapEditorManager.Map = null;
        MapEditorManager.MapContainer = null;
        MapEditorManager.SpawnPoint = Vector2.zero;
        MapEditorManager.CurrentButtonPressed = 0;
        MapEditorManager.LastEncounteredObject = null;
        Tool.ToolKeys = new List<string>();
        Tool.ToolStatus = new Dictionary<string, bool>();
        Tool.PaintingMenu = null;
        Tool.SelectionMenu = null;
        Tool.SelectionOptions = null;
        Tool.SpawnPointOptions = null;
        Mouse.LastMousePosition = Vector2.zero;
        Layer.LayerStatus = new Dictionary<string, bool>();
        Layer.LayerIndex = new Dictionary<string, int>();
        Layer.LayerNames = new List<string>();
    }
}
