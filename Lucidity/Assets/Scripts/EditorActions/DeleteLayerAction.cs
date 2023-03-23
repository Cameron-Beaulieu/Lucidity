using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteLayerAction : EditorAction {

    /// <summary>
    /// Delete layer constructor, initializing the <c>EditorAction</c> attributes.
    /// </summary>
    /// <param name="relatedObjects">
    /// List of <c>GameObject</c> that are related to the layer to be deleted.
    /// </param>
    public DeleteLayerAction(List<(int, GameObject)> relatedObjects) {
        base.Type = ActionType.DeleteLayer;
        base.RelatedObjects = relatedObjects;
    }
}
