using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteMapObjectAction : EditorAction {

	/// <summary>
	/// Delete map object constructor, initializing the <c>EditorAction</c> attributes.
	/// </summary>
	/// <param name="relatedObjects">
	/// List of <c>GameObject</c> that are related to the map object to be deleted.
	/// </param>
	public DeleteMapObjectAction(List<GameObject> relatedObjects) {
		base.Type = ActionType.DeleteMapObject;
		base.RelatedObjects = relatedObjects;
	}
}
