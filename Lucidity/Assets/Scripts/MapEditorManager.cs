using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditorManager : MonoBehaviour {
    public List<AssetController> AssetButtons;
    public List<GameObject> AssetPrefabs;
    public List<GameObject> AssetImage;
    public int CurrentButtonPressed;
    private LinkedList<EditorAction> _actions;
    private LinkedListNode<EditorAction> _currentAction;

    private void Update() {
        Vector2 worldPosition = getMousePosition();

        if (Input.GetMouseButtonDown(0)
                && AssetButtons[CurrentButtonPressed].Clicked && worldPosition.x > -5f) {
            GameObject mapObject = (GameObject) Instantiate(AssetPrefabs[CurrentButtonPressed],
                        new Vector3(worldPosition.x, worldPosition.y, 0),
                        Quaternion.identity);
                if (_actions == null) {
                    _actions = new LinkedList<EditorAction>();
                    _actions.AddFirst(new PaintAction(mapObject));
                    _currentAction = _actions.First;
                } else {
                    if (_currentAction.Next != null) {
                        // these actions can no longer be redone -- currently this errors bc we're still able to place assets on the toolbar itself
                        // so if i click redo with an asset selected, it'll put an asset on the toolbar (adding to the Next for _currentAction) and want to destroy any objects that had been undone before
                        PermanentlyDeleteActions(_currentAction.Next);
                        LinkedListNode<EditorAction> actionToRemove = _currentAction.Next;
                        while (actionToRemove != null) {
                            _actions.Remove(actionToRemove);
                            actionToRemove = actionToRemove.Next;
                        }
                    }
                    _actions.AddAfter(_currentAction, new PaintAction(mapObject));
                    _currentAction = _currentAction.Next;
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

    // [1,2,3,4]

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
}
