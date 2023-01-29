using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteMapObjectAction : EditorAction {
    public DeleteMapObjectAction (GameObject gameObject) {
        base.setActionType(ActionType.DeleteMapObject);
        base.setGameObject(gameObject);
    }
}
