using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintAction : EditorAction {
    public PaintAction (List<GameObject> relatedObjects) {
        base.Type = ActionType.Paint;
        base.RelatedObjects = relatedObjects;
    }
}
