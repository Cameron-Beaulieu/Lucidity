using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapEditorManager : MonoBehaviour {
    public List<AssetController> AssetButtons;
    public List<GameObject> AssetPrefabs;
    public List<GameObject> AssetImage;
    public int CurrentButtonPressed;
    public LinkedList<EditorAction> Actions;
    public LinkedListNode<EditorAction> CurrentAction;
    public Dictionary<string, bool> ToolStatus = new Dictionary<string, bool>();
    private List<string> _toolKeys = new List<string>();
    public InputField CountInput;
    public int Count;
    [SerializeField] private GameObject _map;
    [SerializeField] private GameObject _mapContainer;
    private GameObject _selectionMenu;
    public GameObject SelectionOptions;
    private GameObject _paintingMenu;
    private Vector2 _lastMousePosition;
    public GameObject LastEncounteredObject;
    [SerializeField] private Slider _brushSizeSlider;
    [SerializeField] private Text _brushSizeText;
    [SerializeField] private float _brushSize;
    public List<Texture2D> CursorTextures;
    private Dictionary<string, Texture2D> _cursorDict = new Dictionary<string, Texture2D>();

    void Awake() {
        Count = 1;
        ShowBrushSizeSlider();
        _paintingMenu = GameObject.Find("Painting Menu");
        _selectionMenu = GameObject.Find("Selection Menu");
        SelectionOptions = GameObject.FindGameObjectWithTag("SelectionScrollContent");
        _selectionMenu.SetActive(false);
        GameObject[] selectableTools = GameObject.FindGameObjectsWithTag("SelectableTool");
        foreach (GameObject tool in selectableTools) {
            if (tool.name == "Brush Tool") {
                ToolStatus.Add(tool.name, true);
            } else {
                ToolStatus.Add(tool.name, false);
            }
            _toolKeys.Add(tool.name);
        }

        foreach (Texture2D cursor in CursorTextures) {
            _cursorDict.Add(cursor.name, cursor);
        }

        string mapSize = CreateNewMap.mapSize;
        RectTransform mapRect = _map.GetComponent<RectTransform>();
        Vector2 mapScale = _map.transform.localScale;

        switch (mapSize) {
            case "Small":
                mapScale *= 1f;
                break;
            case "Medium":
                mapScale *= 1.5f;
                break;
            case "Large":
                mapScale *= 2f;
                break;
            default:
                Debug.Log("Error with sending map size");
                mapScale *= 1.5f;
                break;
        }
        _map.transform.localScale = mapScale;
    }

    private void Update() {
        Vector2 worldPosition = getMousePosition();
        if (Input.GetMouseButton(0)
                && AssetButtons[CurrentButtonPressed].Clicked && ToolStatus["Brush Tool"]) {
            GameObject activeImage = GameObject.FindGameObjectWithTag("AssetImage");
            float assetWidth = activeImage.transform.localScale.x;
            float assetHeight = activeImage.transform.localScale.y;
            // Check if mouse position relative to its last position and the previously encountered
            // asset would allow for a legal placement. Reduces unnecessary computing.
            if (_lastMousePosition != worldPosition &&
                (!(LastEncounteredObject)
                    || Mathf.Abs(worldPosition.x - LastEncounteredObject.transform.position.x)
                        >= assetWidth
                    || Mathf.Abs(worldPosition.y - LastEncounteredObject.transform.position.y)
                        >= assetHeight)) {
                List<GameObject> mapObjects = new List<GameObject>();
                for (int i = 0; i < Count; i++) {
                    GameObject tempParent = new GameObject();
                    tempParent.name = AssetPrefabs[CurrentButtonPressed].name + " Parent";
                    tempParent.transform.SetParent(_mapContainer.transform, true);
                    tempParent.transform.localPosition = 
                        new Vector3(tempParent.transform.localPosition.x,
                            tempParent.transform.localPosition.y, 0);
                    GameObject temp = ((GameObject) Instantiate(AssetPrefabs[CurrentButtonPressed],
                            new Vector3(worldPosition.x + i*2, worldPosition.y, 90),
                                Quaternion.identity, tempParent.transform));
                    temp.transform.localScale = new Vector3 (temp.transform.localScale.x 
                        + Zoom.zoomFactor, temp.transform.localScale.y + Zoom.zoomFactor, 
                            temp.transform.localScale.z + Zoom.zoomFactor);
                    if (temp != null 
                        && !temp.GetComponent<AssetCollision>().IsInvalidPlacement()) {
                        mapObjects.Add(temp);
                    } else {
                        Destroy(tempParent);
                    }
                }
                if (mapObjects.Count == 0) { 
                    // Don't add action to history if there are no objects attached to it
                }
                else if (Actions == null) {
                    Actions = new LinkedList<EditorAction>();
                    Actions.AddFirst(new PaintAction(mapObjects));
                    CurrentAction = Actions.First;
                } else {
                    if (CurrentAction != null && CurrentAction.Next != null) {
                        // these actions can no longer be redone
                        PermanentlyDeleteActions(CurrentAction.Next);
                        LinkedListNode<EditorAction> actionToRemove = CurrentAction.Next;
                        while (actionToRemove != null) {
                            Actions.Remove(actionToRemove);
                            actionToRemove = actionToRemove.Next;
                        }
                        Actions.AddAfter(CurrentAction, new PaintAction(mapObjects));
                        CurrentAction = CurrentAction.Next;
                    } else if (CurrentAction != null) {
                        Actions.AddAfter(CurrentAction, new PaintAction(mapObjects));
                        CurrentAction = CurrentAction.Next;
                    } else if (CurrentAction == null && Actions != null) {
                        // there is only one action and it has been undone
                        PermanentlyDeleteActions(Actions.First);
                        Actions.Clear();
                        Actions.AddFirst(new PaintAction(mapObjects));
                        CurrentAction = Actions.First;
                    }
                }
                if (mapObjects.Count > 0) {
                    LastEncounteredObject = mapObjects[0];
                }
            }
            _lastMousePosition = worldPosition;
        }
        // TODO: Implement other actions here
    }

    public static Vector2 getMousePosition() {
        Vector2 screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        return Camera.main.ScreenToWorldPoint(screenPosition);
    }

    public void Undo() {
        if (CurrentAction != null) {
            EditorAction actionToUndo = CurrentAction.Value;
            switch(actionToUndo.Type) {
                case EditorAction.ActionType.Paint:
                    foreach (GameObject obj in actionToUndo.RelatedObjects) {
                        if (obj != null) {
                            obj.SetActive(false);
                        }
                    }
                    break;
                case EditorAction.ActionType.DeleteMapObject:
                    foreach (GameObject obj in actionToUndo.RelatedObjects) {
                        if (obj != null) {
                            obj.SetActive(true);
                        }
                    }
                    break;
                case EditorAction.ActionType.MoveMapObject:
                    // TODO: Implement
                    break;
                case EditorAction.ActionType.ResizeMapObject:
                    // TODO: Implement
                    break;
                case EditorAction.ActionType.RotateMapObject:
                    // TODO: Implement
                    break;
                case EditorAction.ActionType.CreateLayer:
                    // TODO: Implement
                    break;
                case EditorAction.ActionType.DeleteLayer:
                    // TODO: Implement
                    break;
                case EditorAction.ActionType.MoveLayer:
                    // TODO: Implement
                    break;
                case EditorAction.ActionType.RenameLayer:
                    // TODO: Implement
                    break;
            }
            if (CurrentAction.Previous != null) {
                CurrentAction = CurrentAction.Previous;
            } else {
                CurrentAction = null;
            }
        }
    }

    public void Redo() {
        if ((CurrentAction == null && Actions != null) || CurrentAction.Next != null) {
            // if current action is null but actions list isn't, 
            // then we want to redo from the beginning
            // else we want to redo from the current action
            EditorAction actionToRedo = Actions.First.Value;
            if (CurrentAction != null) {
                actionToRedo = CurrentAction.Next.Value;
            }
            
            // EditorAction actionToRedo = CurrentAction.Next.Value;
            switch(actionToRedo.Type) {
                case EditorAction.ActionType.Paint:
                    foreach (GameObject obj in actionToRedo.RelatedObjects) {
                        if (obj != null) {
                            obj.SetActive(true);
                        }
                    }
                    break;
                case EditorAction.ActionType.DeleteMapObject:
                    foreach (GameObject obj in actionToRedo.RelatedObjects) {
                        if (obj != null) {
                            obj.SetActive(false);
                        }
                    }
                    break;
                case EditorAction.ActionType.MoveMapObject:
                    // TODO: Implement
                    break;
                case EditorAction.ActionType.ResizeMapObject:
                    // TODO: Implement
                    break;
                case EditorAction.ActionType.RotateMapObject:
                    // TODO: Implement
                    break;
                case EditorAction.ActionType.CreateLayer:
                    // TODO: Implement
                    break;
                case EditorAction.ActionType.DeleteLayer:
                    // TODO: Implement
                    break;
                case EditorAction.ActionType.MoveLayer:
                    // TODO: Implement
                    break;
                case EditorAction.ActionType.RenameLayer:
                    // TODO: Implement
                    break;
            }

            if (CurrentAction == null) {
                CurrentAction = Actions.First;
            } else {
                CurrentAction = CurrentAction.Next;
            }
        }
    }

    /// <summary>
    /// Permanently deletes all MapObjects associated with actions in the list.
    /// This is done to ensure no inactive MapObjects associated with actions 
    /// that can no longer be redone are left in the scene.
    /// </summary>
    private void PermanentlyDeleteActions(LinkedListNode<EditorAction> actionToDelete) {
        while (actionToDelete != null) {
            if (actionToDelete.Value.Type == EditorAction.ActionType.Paint) {
                foreach (GameObject obj in actionToDelete.Value.RelatedObjects) {
                    Destroy(obj);
                }
            }
            actionToDelete = actionToDelete.Next;
        }
    }

    /// <summary>
    /// Removes the asset following the cursor (if any) and deselects the brush tool button and selected sprite/terrain.
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
    /// Changes which tool is currently being used based on user's selection.
    /// </summary>
    public void ChangeTools(string toolSelected) {

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
                }
                break;
            default:
                _paintingMenu.SetActive(false);
                _selectionMenu.SetActive(false);
                break;
        }

        if (_cursorDict.ContainsKey(toolSelected)) {
            Cursor.SetCursor(_cursorDict[toolSelected], Vector2.zero, CursorMode.Auto);
        } else {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }

        foreach (string toolKey in _toolKeys) {
            if (toolKey != toolSelected) {
                ToolStatus[toolKey] = false;
            } else {
                ToolStatus[toolKey] = true;
            }
        }
    }

    public void ReadCountInput(string s) {
        Count = int.Parse(s);
        // Restrict input to only be positive
        if (Count < 0) {
            Count *= -1;
            CountInput.text = "" + Count;
        }
    }

    public void ShowBrushSizeSlider() {
        _brushSize = _brushSizeSlider.value;
        string sliderMessage = _brushSize + " px";
        _brushSizeText.text = sliderMessage;
    }
}
