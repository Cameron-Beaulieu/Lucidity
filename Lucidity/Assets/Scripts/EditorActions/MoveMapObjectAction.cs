using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMapObjectAction : EditorAction {
    private Vector2 _oldPosition;
    private Vector2 _newPosition;

    public MoveMapObjectAction (GameObject gameObject, Vector2 oldPosition, Vector2 newPosition) {
        base.setActionType(ActionType.MoveMapObject);
        base.setGameObject(gameObject);
        _oldPosition = oldPosition;
        _newPosition = newPosition;
    }

    public Vector2 OldPosition {
        get { return _oldPosition; }
    }

    public Vector2 NewPosition {
        get { return _newPosition; }
    }
}
