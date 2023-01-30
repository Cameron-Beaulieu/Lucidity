using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMapObjectAction : EditorAction {
    private Vector2 _oldPosition;
    private Vector2 _newPosition;

    public MoveMapObjectAction (List<GameObject> relatedObjects, Vector2 oldPosition, Vector2 newPosition) {
        base.Type = ActionType.MoveMapObject;
        base.RelatedObjects = relatedObjects;
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
