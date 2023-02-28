using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Util : MonoBehaviour {

    public static float FloatTolerance = 0.005f; // used for comparing floats 
    
    /// <summary>
    /// Checks that all tools are unselected except the one specified.
    /// </summary>
    /// <param name="toolSelected">The name of the tool that should be selected.</param>
    public static void CheckAllOtherToolsAreUnselected(string toolSelected) {
        foreach (string tool in Tool.ToolKeys) {
            if (tool != toolSelected) {
                Assert.IsFalse(Tool.ToolStatus[tool]);
            }
        }
    }

    /// <summary>
    /// Resets all static variables in MapEditorManager and Tool. This is necessary to avoid 
    /// interference between tests.
    /// </summary>
    public static void ResetStaticVariables() {
        MapEditorManager.MapObjects = new Dictionary<int, MapObject>();
        MapEditorManager.Layers = new List<Dictionary<int, MapObject>>();
        MapEditorManager.Actions = null;
        MapEditorManager.CurrentAction = null;
        MapEditorManager.ToolToCursorMap = new Dictionary<string, Texture2D>();
        MapEditorManager.Map = null;
        MapEditorManager.MapContainer = null;
        MapEditorManager.CurrentButtonPressed = 0;
        MapEditorManager.LastEncounteredObject = null;
        Tool.ToolKeys = new List<string>();
        Tool.ToolStatus = new Dictionary<string, bool>();
        Mouse.LastMousePosition = Vector2.zero;
        Layer.LayerStatus = new Dictionary<string, bool>();
        Layer.LayerIndex = new Dictionary<string, int>();
        Layer.LayerNames = new List<string>();
    }

    /// <summary>
    /// Paints an asset for testing (for tests where painting is not the thing being tested).
    /// </summary>
    public static void PaintAnAsset(Vector2 positionToPlace, string assetName) {
        if (!Tool.ToolStatus["Brush Tool"]) {
            GameObject.Find("Brush Tool").GetComponent<Button>().onClick.Invoke();
        }
        GameObject.Find(assetName + "Button").GetComponent<Button>().onClick.Invoke();
        GameObject.Find("MapEditorManager").GetComponent<MapEditorManager>()
            .PaintAtPosition(positionToPlace);
    }

}
