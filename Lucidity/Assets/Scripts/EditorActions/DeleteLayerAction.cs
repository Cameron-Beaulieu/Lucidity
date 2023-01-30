using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteLayerAction : EditorAction {
    public DeleteLayerAction (List<GameObject> relatedObjects) {
        base.Type = ActionType.DeleteLayer;
        base.RelatedObjects = relatedObjects;
    }
}
