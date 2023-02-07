using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMapObjectAction : EditorAction {
	private Vector2 _newPosition;
	private Vector2 _oldPosition;

	public Vector2 NewPosition {
		get { return _newPosition; }
	}

	public Vector2 OldPosition {
		get { return _oldPosition; }
	}

	/// <summary>
	/// Move map object constructor, initializing the <c>EditorAction</c> attributes and map
	/// object positions.
	/// </summary>
	/// <param name="relatedObjects">
	/// List of <c>GameObject</c> that are related to the map object to be moved.
	/// </param>
	/// <param name="oldPosition">
	/// <c>Vector2</c> corresponding to the original 2D position of the map object to be moved.
	/// </param>
	/// <param name="newPosition">
	/// <c>Vector2</c> corresponding to the new 2D position for the map object to be moved to.
	/// </param>
	public MoveMapObjectAction (List<GameObject> relatedObjects,
								Vector2 oldPosition,
								Vector2 newPosition) {
		base.Type = ActionType.MoveMapObject;
		base.RelatedObjects = relatedObjects;
		_oldPosition = oldPosition;
		_newPosition = newPosition;
	}
}
