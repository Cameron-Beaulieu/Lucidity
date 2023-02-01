using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateMapObjectAction : EditorAction {
    private bool _isClockwise;

    public RotateMapObjectAction (List<GameObject> relatedObjects, bool IsClockwise) {
        base.Type = ActionType.RotateMapObject;
        base.RelatedObjects = relatedObjects;
        _isClockwise = IsClockwise;
    }

    public bool IsClockwise {
        get { return _isClockwise; }
    }
}
