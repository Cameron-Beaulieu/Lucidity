using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizeMapObjectAction : EditorAction {
    private Vector2 _oldSize;
    private Vector2 _newSize;

    public ResizeMapObjectAction (GameObject gameObject, Vector2 oldSize, Vector2 newSize) {
        base.setActionType(ActionType.RotateMapObject);
        base.setGameObject(gameObject);
        _oldSize = oldSize;
        _newSize = newSize;
    }

    public Vector2 OldSize {
        get { return _oldSize; }
    }

    public Vector2 NewSize {
        get { return _newSize; }
    }
}
