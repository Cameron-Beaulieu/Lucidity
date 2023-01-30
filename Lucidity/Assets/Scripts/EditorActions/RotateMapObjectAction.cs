using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateMapObjectAction : EditorAction {
    private Quaternion _oldRotation;
    private Quaternion _newRotation;

    public RotateMapObjectAction (GameObject gameObject, Quaternion oldRotation, Quaternion newRotation) {
        base.setActionType(ActionType.RotateMapObject);
        base.setGameObject(gameObject);
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
