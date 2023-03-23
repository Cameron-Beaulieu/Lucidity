using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EditorAction {
    public enum ActionType {
        Paint,
        DeleteMapObject,
        MoveMapObject,
        ResizeMapObject,
        RotateMapObject,
        CreateLayer,
        DeleteLayer,
        MoveLayer,
        RenameLayer
    }
    private ActionType _type;
    private List<(int, GameObject)> _relatedObjects = new List<(int, GameObject)>();

    public ActionType Type {
        get { return _type; }
        set { _type = value; }
    }

    public List<(int, GameObject)> RelatedObjects {
        get { return _relatedObjects; }
        set { _relatedObjects = value; }
    }

    public override string ToString() {
        string result = "Action Type: " + Type + "\n";
        result += "Related Objects: \n";
        foreach ((int, GameObject) obj in RelatedObjects) {
            result += "ID: " + obj.Item1 + "\n";
        }
        return result;
    }
}
