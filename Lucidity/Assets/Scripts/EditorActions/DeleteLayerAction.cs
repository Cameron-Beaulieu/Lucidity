using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteLayerAction : EditorAction {
    public DeleteLayerAction (GameObject gameObject) {
        base.setActionType(ActionType.DeleteLayer);
        base.setGameObject(gameObject);
    }
}
