using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateMapObjectAction : EditorAction {
    private Quaternion _newRotation;
    private Quaternion _oldRotation;

    public Quaternion NewRotation {
        get { return _newRotation; }
    }

    public Quaternion OldRotation {
        get { return _oldRotation; }
    }

    /// <summary>
    /// Rotate map object constructor, initializing the <c>EditorAction</c> attributes and map
    /// Quaternion values.
    /// </summary>
    /// <param name="relatedObjects">
    /// List of <c>GameObject</c> that are related to the map object to be rotated.
    /// </param>
    /// <param name="oldRotation">
    /// <c>Quaternion</c> corresponding to the original orientation of the map object to be
    /// rotated.
    /// </param>
    /// <param name="newRotation">
    /// <c>Quaternion</c> corresponding to the new orientation of the map object to be rotated to.
    /// </param>
    public RotateMapObjectAction(List<GameObject> relatedObjects,
                                Quaternion oldRotation,Quaternion newRotation) {
        base.Type = ActionType.RotateMapObject;
        base.RelatedObjects = relatedObjects;
        _oldRotation = oldRotation;
        _newRotation = newRotation;
    }
}
