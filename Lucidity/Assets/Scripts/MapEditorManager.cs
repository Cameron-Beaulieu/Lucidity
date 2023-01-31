using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapEditorManager : MonoBehaviour {
    public List<AssetController> AssetButtons;
    public List<GameObject> AssetPrefabs;
    public List<GameObject> AssetImage;
    public int CurrentButtonPressed;
    private LinkedList<EditorAction> _actions;
    private LinkedListNode<EditorAction> _currentAction;
    public Dictionary<string, bool> ToolStatus = new Dictionary<string, bool>();
    private List<string> _toolKeys = new List<string>();
    public InputField CountInput;
    public int Count;
    private GameObject _selectionMenu;
    private GameObject _paintingMenu;
    private Vector2 _lastMousePosition;
    public GameObject LastEncounteredObject;
    [SerializeField] private Slider _brushSizeSlider;
    [SerializeField] private Text _brushSizeText;
    [SerializeField] private float _brushSize;

    void Start() {
        Count = 1;
        ShowBrushSizeSlider();
        _paintingMenu = GameObject.Find("Painting Menu");
        _selectionMenu = GameObject.Find("Selection Menu");
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
    }

    private void Update() {
        Vector2 worldPosition = getMousePosition();
        // TODO: remove && worldPosition.x > -5 from if statement
        if (Input.GetMouseButton(0)
                && AssetButtons[CurrentButtonPressed].Clicked && worldPosition.x > -4) {
            float assetWidth = AssetPrefabs[CurrentButtonPressed].transform.localScale.x;
            float assetHeight = AssetPrefabs[CurrentButtonPressed].transform.localScale.y;
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
                    GameObject temp = ((GameObject) Instantiate(AssetPrefabs[CurrentButtonPressed],
                            new Vector3(worldPosition.x + i*2, worldPosition.y, 0),
                            Quaternion.identity));
                    if (temp != null) {
                        mapObjects.Add(temp);
                    }
                }
                if (_actions == null) {
                    _actions = new LinkedList<EditorAction>();
                _actions.AddFirst(new PaintAction(mapObjects));
                    _currentAction = _actions.First;
                } else {
                    if (_currentAction != null && _currentAction.Next != null) {
                        // these actions can no longer be redone
                        PermanentlyDeleteActions(_currentAction.Next);
                        LinkedListNode<EditorAction> actionToRemove = _currentAction.Next;
                        while (actionToRemove != null) {
                            _actions.Remove(actionToRemove);
                            actionToRemove = actionToRemove.Next;
                        }
                _actions.AddAfter(_currentAction, new PaintAction(mapObjects));
                        _currentAction = _currentAction.Next;
                    } else if (_currentAction != null) {
                _actions.AddAfter(_currentAction, new PaintAction(mapObjects));
                        _currentAction = _currentAction.Next;
                    } else if (_currentAction == null && _actions != null) {
                        // there is only one action and it has been undone
                        PermanentlyDeleteActions(_actions.First);
                        _actions.Clear();
                _actions.AddFirst(new PaintAction(mapObjects));
                        _currentAction = _actions.First;
                    }
                }
                LastEncounteredObject = mapObjects[0];
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
        if (_currentAction != null) {
            EditorAction actionToUndo = _currentAction.Value;
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
            if (_currentAction.Previous != null) {
                _currentAction = _currentAction.Previous;
            } else {
                _currentAction = null;
            }
        }
    }

    public void Redo() {
        if ((_currentAction == null && _actions != null) || _currentAction.Next != null) {
            // if current action is null but actions list isn't, 
            // then we want to redo from the beginning
            // else we want to redo from the current action
            EditorAction actionToRedo = _actions.First.Value;
            if (_currentAction != null) {
                actionToRedo = _currentAction.Next.Value;
            }
            
            // EditorAction actionToRedo = _currentAction.Next.Value;
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

            if (_currentAction == null) {
                _currentAction = _actions.First;
            } else {
                _currentAction = _currentAction.Next;
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
        ToolStatus["Brush Tool"] = false;
        Destroy(GameObject.FindGameObjectWithTag("AssetImage"));
        GameObject[] paintButtons = GameObject.FindGameObjectsWithTag("PaintButton");
        foreach (GameObject button in paintButtons) {
            if (button.GetComponent<AssetController>().Clicked) {
                button.GetComponent<AssetController>().UnselectButton();
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
            // Default case is having the selection menu open
            default:
                _paintingMenu.SetActive(false);
                _selectionMenu.SetActive(true);
                break;
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
