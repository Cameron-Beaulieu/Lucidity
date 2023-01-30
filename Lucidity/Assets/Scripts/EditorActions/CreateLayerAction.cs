using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateLayerAction : EditorAction {
    public CreateLayerAction (List<GameObject> relatedObjects) {
        base.Type = ActionType.CreateLayer;
        base.RelatedObjects = relatedObjects;
    }
}
