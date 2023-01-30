using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateLayerAction : EditorAction {
    public CreateLayerAction (GameObject gameObject) {
        base.setActionType(ActionType.CreateLayer);
        base.setGameObject(gameObject);
    }
}
