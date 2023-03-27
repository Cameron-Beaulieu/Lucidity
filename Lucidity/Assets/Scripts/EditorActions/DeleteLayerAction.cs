using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteLayerAction : EditorAction {
    public string LayerName;

    /// <summary>
    /// Delete layer constructor, initializing the <c>EditorAction</c> attributes.
    /// </summary>
    /// <param name="relatedObjects">
    /// List of <c>GameObject</c> that are related to the layer to be deleted.
    /// </param>
    /// <param name="layerName">
    /// <c>string</c> matching the name of the layer being deleted
    /// </param>
    public DeleteLayerAction(List<(int, GameObject)> relatedObjects, string layerName) {
        base.Type = ActionType.DeleteLayer;
        base.RelatedObjects = relatedObjects;
        LayerName = layerName;
    }
}
