using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteLayerAction : EditorAction {
    // public string LayerName;
    // public int LayerIndex;
    // public List<MapObject> DeletedMapObjects;

    // /// <summary>
    // /// Delete layer constructor, initializing the <c>EditorAction</c> attributes.
    // /// </summary>
    // /// <param name="relatedObjects">
    // /// List of <c>GameObject</c> that are related to the layer to be deleted.
    // /// </param>
    // /// <param name="layerName">
    // /// A <c>string</c> representing the name of the layer being deleted
    // /// </param>
    // /// <param name="relatedObjects">
    // /// A <c>int</c> representing the index of the layer withing the Layers list in MEM
    // /// </param>
    // public DeleteLayerAction(string layerName, int layerIndex, List<MapObject> deletedMapObjects) {
    //     base.Type = ActionType.DeleteLayer;
    //     base.RelatedObjects = new List<GameObject>{};
    //     LayerName = layerName;
    //     LayerIndex = layerIndex;
    //     DeletedMapObjects = deletedMapObjects;
    // }
    /// <summary>
    /// Create layer constructor, initializing the <c>EditorAction</c> attributes.
    /// </summary>
    /// <param name="relatedObjects">
    /// List of <c>GameObject</c> that are related to the layer to be created.
    /// </param>
    public DeleteLayerAction(List<GameObject> relatedObjects) {
        base.Type = ActionType.CreateLayer;
        base.RelatedObjects = relatedObjects;
    }
}
