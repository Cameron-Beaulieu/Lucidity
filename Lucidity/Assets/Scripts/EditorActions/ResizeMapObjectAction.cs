using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizeMapObjectAction : EditorAction {
	private Vector2 _oldSize;
	private Vector2 _newSize;

	/// <summary>
	/// Resize map object constructor, initializing the <c>EditorAction</c> attributes and map
	/// object scales.
	/// </summary>
	/// <param name="relatedObjects">
	/// List of <c>GameObject</c> that are related to the layer to be renamed.
	/// </param>
	/// <param name="oldSize">
	/// 2D vector corresponding to the original scale of the map object to be resized.
	/// </param>
	/// <param name="newSize">
	/// 2D vector corresponding to the new scale for the map object to be resized to.
	/// </param>
	public ResizeMapObjectAction (
			List<GameObject> relatedObjects,
			Vector2 oldSize,
			Vector2 newSize) {
		base.Type = ActionType.RotateMapObject;
		base.RelatedObjects = relatedObjects;
		_oldSize = oldSize;
		_newSize = newSize;
	}

	public Vector2 OldSize {
		get { return _oldSize; }
	}

	public Vector2 NewSize {
		get { return _newSize; }
	}
}
