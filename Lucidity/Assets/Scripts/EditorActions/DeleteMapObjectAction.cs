using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteMapObjectAction : EditorAction {
    public DeleteMapObjectAction (List<GameObject> relatedObjects) {
        base.Type = ActionType.DeleteMapObject;
        base.RelatedObjects = relatedObjects;
    }
}
