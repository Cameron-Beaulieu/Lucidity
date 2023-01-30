using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLayerAction : EditorAction {
    private int _oldIndex;
    private int _newIndex;

    public MoveLayerAction (List<GameObject> relatedObjects, int oldIndex, int newIndex) {
        base.Type = ActionType.MoveLayer;
        base.RelatedObjects = relatedObjects;
        _oldIndex = oldIndex;
        _newIndex = newIndex;
    }

    public int OldIndex {
        get { return _oldIndex; }
    }

    public int NewIndex {
        get { return _newIndex; }
    }
}
