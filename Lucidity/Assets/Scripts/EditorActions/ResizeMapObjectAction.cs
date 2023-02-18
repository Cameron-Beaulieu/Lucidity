using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizeMapObjectAction : EditorAction {
	private Vector3 _newSize;
	private Vector3 _oldSize;

	/// <summary>
	/// Resize map object constructor, initializing the <c>EditorAction</c> attributes and map
	/// object scales.
	/// </summary>
	/// <param name="relatedObjects">
	/// List of <c>GameObject</c> that are related to the layer to be renamed.
	/// </param>
	/// <param name="oldSize">
	/// <c>Vector2</c> corresponding to the original scale of the map object to be resized.
	/// </param>
	/// <param name="newSize">
	/// <c>Vector2</c> corresponding to the new scale for the map object to be resized to.
	/// </param>
	public ResizeMapObjectAction(List<GameObject> relatedObjects, Vector3 oldSize, Vector3 newSize) {
		base.Type = ActionType.ResizeMapObject;
		base.RelatedObjects = relatedObjects;
		_oldSize = oldSize;
		_newSize = newSize;
	}
	public Vector3 OldSize {
		get { return _oldSize; }
	}

	public Vector3 NewSize {
		get { return _newSize; }
	}
}
