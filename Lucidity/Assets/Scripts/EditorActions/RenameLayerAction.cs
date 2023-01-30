using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenameLayerAction : EditorAction {
    private string _oldName;
    private string _newName;

    public RenameLayerAction (GameObject gameObject, string oldName, string newName) {
        base.setActionType(ActionType.RenameLayer);
        base.setGameObject(gameObject);
        _oldName = oldName;
        _newName = newName;
    }

    public string OldName {
        get { return _oldName; }
    }

    public string NewName {
        get { return _newName; }
    }
}
