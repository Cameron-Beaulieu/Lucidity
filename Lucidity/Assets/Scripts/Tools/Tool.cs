using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tool : MonoBehaviour {
    public static List<string> ToolKeys = new List<string>();
    public static Dictionary<string, bool> ToolStatus = new Dictionary<string, bool>();
    private static GameObject _paintingMenu;
    private static GameObject _selectionMenu;
    private static GameObject _selectionOptions;
    private static GameObject _spawnPointOptions;
    private MapEditorManager _editor;
    private string _name;
    private Color _unselected = new Color(48/255f, 49/255f, 52/255f);

    public static GameObject PaintingMenu {
        get { return _paintingMenu; }
        set { _paintingMenu = value; }
    }

    public static GameObject SelectionMenu {
        get { return _selectionMenu; }
        set { _selectionMenu = value; }
    }

    public static GameObject SelectionOptions {
        get { return _selectionOptions; }
        set { _selectionOptions = value; }
    }

    public static GameObject SpawnPointOptions {
        get { return _spawnPointOptions; }
        set { _spawnPointOptions = value; }
    }

    private void Start() {
        _editor = GameObject.FindGameObjectWithTag("MapEditorManager")
            .GetComponent<MapEditorManager>();
        _name = gameObject.name;
        gameObject.GetComponent<Button>().onClick.AddListener(ToolButtonClickHandler);
    }

    private void Update() {
        if (ToolStatus.ContainsKey(_name)
            && ToolStatus[_name]
            && gameObject.GetComponent<Image>().color != Color.black) {
            gameObject.GetComponent<Image>().color = Color.black;
        } else if (ToolStatus.ContainsKey(_name)
            && !ToolStatus[_name]
            && gameObject.GetComponent<Image>().color != _unselected) {
            gameObject.GetComponent<Image>().color = _unselected;
        }
    }

    /// <summary>
    /// Changes the tool currently in use to the selected tool.
    /// </summary>
    /// <param name="toolSelected">
    /// <c>string</c> corresponding to the newly selected tool.
    /// </param>
    public static void ChangeTools(string toolSelected) {
        if (toolSelected != "Selection Tool" && SelectMapObject.SelectedObject != null) {
            SelectMapObject.UnselectMapObject();
        }
        switch (toolSelected) {
            case "Brush Tool":
                _paintingMenu.SetActive(true);
                _selectionMenu.SetActive(false);
                break;
            case "Selection Tool":
                _paintingMenu.SetActive(false);
                _selectionMenu.SetActive(true);
                if (SelectMapObject.SelectedObject == null) {
                    SelectionOptions.SetActive(false);
                    SpawnPointOptions.SetActive(false);
                } 
                break;
            default:    // Default case is having the selection menu open
                _paintingMenu.SetActive(false);
                _selectionMenu.SetActive(false);
                break;
        }
        foreach (string toolKey in ToolKeys) {
            if (toolKey != toolSelected) {
                ToolStatus[toolKey] = false;
            } else {
                ToolStatus[toolKey] = true;
            }
        }

        if (MapEditorManager.ToolToCursorMap.ContainsKey(toolSelected)) {
            Cursor.SetCursor(MapEditorManager.ToolToCursorMap[toolSelected], new Vector2(16f,16f), 
                             CursorMode.Auto);
        } else {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }

    /// <summary>
    /// Removes the asset following the cursor (if any) and deselects the brush tool button and
    /// selected sprite/terrain.
    /// </summary>
    public void StopPainting() {
        if (ToolStatus["Brush Tool"]) {
            ToolStatus["Brush Tool"] = false;
            Destroy(GameObject.FindGameObjectWithTag("AssetImage"));
            GameObject[] paintButtons = GameObject.FindGameObjectsWithTag("PaintButton");
            foreach (GameObject button in paintButtons) {
                if (button.GetComponent<AssetController>().Clicked) {
                    button.GetComponent<AssetController>().UnselectButton();
                }
            }
        }
    }

    /// <summary>
    /// Change the tool being used by the map editor to appropriately reflect the selected tool.
    /// </summary>
    public void ToolButtonClickHandler() {
        if (_name != "Brush Tool" && ToolStatus["Brush Tool"]) {
            StopPainting();
        }
        ChangeTools(_name);
    }
}
