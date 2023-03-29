using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateMapObjectAction : EditorAction {
    private bool _isClockwise;

    public bool IsClockwise {
        get { return _isClockwise; }
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
    public RotateMapObjectAction(List<(int, GameObject)> relatedObjects,
                                 bool isClockwise) {
        base.Type = ActionType.RotateMapObject;
        base.RelatedObjects = relatedObjects;
        _isClockwise = isClockwise;
    }
}
