using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateLayerAction : EditorAction {

    /// <summary>
    /// Create layer constructor, initializing the <c>EditorAction</c> attributes.
    /// </summary>
    /// <param name="relatedObjects">
    /// List of <c>GameObject</c> that are related to the layer to be created.
    /// </param>
    public CreateLayerAction(List<GameObject> relatedObjects) {
        base.Type = ActionType.CreateLayer;
        base.RelatedObjects = relatedObjects;
    }
}
