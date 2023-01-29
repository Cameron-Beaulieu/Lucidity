using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintAction : EditorAction {
    public PaintAction (GameObject gameObject) {
        base.setActionType(ActionType.Paint);
        base.setGameObject(gameObject);
    }
}
