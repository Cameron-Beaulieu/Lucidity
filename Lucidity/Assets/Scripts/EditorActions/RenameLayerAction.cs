using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenameLayerAction : EditorAction {
    private string _oldName;
    private string _newName;

    public RenameLayerAction (List<GameObject> relatedObjects, string oldName, string newName) {
        base.Type = ActionType.RenameLayer;
        base.RelatedObjects = relatedObjects;
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
