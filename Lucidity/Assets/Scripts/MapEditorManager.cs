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

    void Start() {
        Debug.Log("This should only run once");
        Count = 1;
        GameObject[] selectableTools = GameObject.FindGameObjectsWithTag("SelectableTool");
        foreach (GameObject tool in selectableTools) {
            if (tool.name == "Selection Tool") {
                ToolStatus.Add(tool.name, true);
            } else {
                ToolStatus.Add(tool.name, false);
            }
            _toolKeys.Add(tool.name);
        }
    }

    private void Update() {
        Vector2 worldPosition = getMousePosition();

        if (Input.GetMouseButtonDown(0)
                && AssetButtons[CurrentButtonPressed].Clicked) {
            for (int i = 0; i < Count; i++) {
                GameObject mapObject = (GameObject) Instantiate(AssetPrefabs[CurrentButtonPressed],
                        new Vector3(worldPosition.x + i*2, worldPosition.y, 0),
                        Quaternion.identity);
                if (_actions == null) {
                    _actions = new LinkedList<EditorAction>();
                    _actions.AddFirst(new PaintAction(mapObject));
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
                        _actions.AddAfter(_currentAction, new PaintAction(mapObject));
                        _currentAction = _currentAction.Next;
                    } else if (_currentAction != null) {
                        _actions.AddAfter(_currentAction, new PaintAction(mapObject));
                        _currentAction = _currentAction.Next;
                    } else if (_currentAction == null && _actions != null) {
                        // there is only one action and it has been undone
                        PermanentlyDeleteActions(_actions.First);
                        _actions.Clear();
                        _actions.AddFirst(new PaintAction(mapObject));
                        _currentAction = _actions.First;
                    }
                }
            }
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
            switch(actionToUndo.getActionType()) {
                case EditorAction.ActionType.Paint:
                    actionToUndo.getGameObject().SetActive(false);
                    break;
                case EditorAction.ActionType.DeleteMapObject:
                    actionToUndo.getGameObject().SetActive(true);
                    break;
                case EditorAction.ActionType.MoveMapObject:
                    actionToUndo.getGameObject().transform.position = ((MoveMapObjectAction) actionToUndo).OldPosition;
                    break;
                case EditorAction.ActionType.ResizeMapObject:
                    actionToUndo.getGameObject().transform.localScale = ((ResizeMapObjectAction) actionToUndo).OldSize;
                    break;
                case EditorAction.ActionType.RotateMapObject:
                    actionToUndo.getGameObject().transform.rotation = ((RotateMapObjectAction) actionToUndo).OldRotation;
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
            switch(actionToRedo.getActionType()) {
                case EditorAction.ActionType.Paint:
                    actionToRedo.getGameObject().SetActive(true);
                    break;
                case EditorAction.ActionType.DeleteMapObject:
                    actionToRedo.getGameObject().SetActive(false);
                    break;
                case EditorAction.ActionType.MoveMapObject:
                    actionToRedo.getGameObject().transform.position = ((MoveMapObjectAction) actionToRedo).NewPosition;
                    break;
                case EditorAction.ActionType.ResizeMapObject:
                    actionToRedo.getGameObject().transform.localScale = ((ResizeMapObjectAction) actionToRedo).NewSize;
                    break;
                case EditorAction.ActionType.RotateMapObject:
                    actionToRedo.getGameObject().transform.rotation = ((RotateMapObjectAction) actionToRedo).NewRotation;
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
            if (actionToDelete.Value.getActionType() == EditorAction.ActionType.Paint) {
             Destroy(actionToDelete.Value.getGameObject());
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
        foreach (string toolKey in _toolKeys) {
            if (toolKey != toolSelected) {
                ToolStatus[toolKey] = false;
            } else {
                ToolStatus[toolKey] = true;
            }
        }
    }

    public void ReadCountInput(string s) {
        Debug.Log("ReadCountInput method");
        Count = int.Parse(s);
        // Restrict input to only be positive
        if (Count < 0) {
            Count *= -1;
            CountInput.text = "" + Count;
        }
    }
}
