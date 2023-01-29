using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EditorAction {
    public enum ActionType {
        Paint,
        DeleteMapObject,
        MoveMapObject,
        ResizeMapObject,
        RotateMapObject,
        CreateLayer,
        DeleteLayer,
        MoveLayer,
        RenameLayer
    }
    private ActionType _actionType;
    private GameObject _gameObject;

    public ActionType getActionType() {
        return _actionType;
    }

    public GameObject getGameObject() {
        return _gameObject;
    }

    public void setActionType(ActionType actionType) {
        _actionType = actionType;
    }

    public void setGameObject(GameObject gameObject) {
        _gameObject = gameObject;
    } 

}
