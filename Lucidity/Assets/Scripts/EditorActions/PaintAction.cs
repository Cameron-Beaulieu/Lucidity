using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintAction : EditorAction {

	/// <summary>
	/// Paint action constructor, initializing the <c>EditorAction</c> attributes.
	/// </summary>
	/// <param name="relatedObjects">
	/// List of <c>GameObject</c> that are related to the paint action.
	/// </param>
	public PaintAction (List<GameObject> relatedObjects) {
		base.Type = ActionType.Paint;
		base.RelatedObjects = relatedObjects;
	}
}
