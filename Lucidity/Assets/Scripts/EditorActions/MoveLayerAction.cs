using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLayerAction : EditorAction {
    private int _oldIndex;
    private int _newIndex;

    public MoveLayerAction (GameObject gameObject, int oldIndex, int newIndex) {
        base.setActionType(ActionType.MoveLayer);
        base.setGameObject(gameObject);
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
