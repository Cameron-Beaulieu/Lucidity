using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenameLayerAction : EditorAction {
    private string _newName;
    private string _oldName;

    public string NewName {
        get { return _newName; }
    }

    public string OldName {
        get { return _oldName; }
    }

    /// <summary>
    /// Rename layer constructor, initializing the <c>EditorAction</c> attributes and layer names.
    /// </summary>
    /// <param name="relatedObjects">
    /// List of <c>GameObject</c> that are related to the layer to be renamed.
    /// </param>
    /// <param name="oldName">
    /// <c>string</c> corresponding to the original name of the layer to be renamed.
    /// </param>
    /// <param name="newName">
    /// <c>string</c> corresponding to the new name for the layer to be renamed to.
    /// </param>
    public RenameLayerAction(List<(int, GameObject)> relatedObjects, string oldName, string newName) {
        base.Type = ActionType.RenameLayer;
        base.RelatedObjects = relatedObjects;
        _oldName = oldName;
        _newName = newName;
    }
}
