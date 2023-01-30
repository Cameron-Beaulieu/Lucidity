using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateMapObjectAction : EditorAction {
    private Quaternion _oldRotation;
    private Quaternion _newRotation;

    public RotateMapObjectAction (List<GameObject> relatedObjects, Quaternion oldRotation, Quaternion newRotation) {
        base.Type = ActionType.RotateMapObject;
        base.RelatedObjects = relatedObjects;
        _oldRotation = oldRotation;
        _newRotation = newRotation;
    }

    public Quaternion OldRotation {
        get { return _oldRotation; }
    }

    public Quaternion NewRotation {
        get { return _newRotation; }
    }
}
