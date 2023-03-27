using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteMapObjectAction : EditorAction {

    public int LayerID;
    public MapObject MapObject;

    /// <summary>
    /// Delete map object constructor, initializing the <c>EditorAction</c> attributes.
    /// </summary>
    /// <param name="relatedObjects">
    /// List of <c>GameObject</c> that are related to the map object to be deleted.
    /// </param>
    /// <param name="layerID">
    /// <c>int</c> matching the id of the layer that the <c>MapObject</c> is being deleted from.
    /// </param>
    /// <param name="mapObject">
    /// <c>MapObject</c> matching the <c>GameObject</c> being deleted
    /// </param>
    public DeleteMapObjectAction(List<(int, GameObject)> relatedObjects, int layerID, MapObject mapObject) {
        base.Type = ActionType.DeleteMapObject;
        base.RelatedObjects = relatedObjects;
        LayerID = layerID;
        MapObject = mapObject;
    }
}
