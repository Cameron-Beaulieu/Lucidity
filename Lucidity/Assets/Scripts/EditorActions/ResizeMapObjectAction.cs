using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizeMapObjectAction : EditorAction {
    private Vector3 _oldSize;
    private Vector3 _newSize;

    public ResizeMapObjectAction (List<GameObject> relatedObjects, Vector3 oldSize, Vector3 newSize) {
        base.Type = ActionType.ResizeMapObject;
        base.RelatedObjects = relatedObjects;
        _oldSize = oldSize;
        _newSize = newSize;
    }

    public Vector3 OldSize {
        get { return _oldSize; }
    }

    public Vector3 NewSize {
        get { return _newSize; }
    }
}
