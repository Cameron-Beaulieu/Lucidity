using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteMapObjectAction : EditorAction {

    public int LayerID;
    public MapObject MapObject;
    public int GameObjectID;

    /// <summary>
    /// Delete map object constructor, initializing the <c>EditorAction</c> attributes.
    /// </summary>
    /// <param name="relatedObjects">
    /// List of <c>GameObject</c> that are related to the map object to be deleted.
    /// </param>
    public DeleteMapObjectAction(List<(int, GameObject)> relatedObjects, int layerID, MapObject mapObject, int gameObjectID) {
        base.Type = ActionType.DeleteMapObject;
        base.RelatedObjects = relatedObjects;
        LayerID = layerID;
        MapObject = mapObject;
        GameObjectID = gameObjectID;
    }
}
