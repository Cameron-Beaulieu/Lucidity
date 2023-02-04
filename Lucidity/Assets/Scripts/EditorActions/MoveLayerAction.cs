using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLayerAction : EditorAction {
	private int _newIndex;
	private int _oldIndex;

	public int NewIndex {
		get { return _newIndex; }
	}

	public int OldIndex {
		get { return _oldIndex; }
	}

	/// <summary>
	/// Move layer constructor, initializing the <c>EditorAction</c> attributes and layer numbers.
	/// </summary>
	/// <param name="relatedObjects">
	/// List of <c>GameObject</c> that are related to the layer to be moved.
	/// </param>
	/// <param name="oldIndex">
	/// <c>int</c> corresponding to the original index of the layer to be moved.
	/// </param>
	/// <param name="newIndex">
	/// <c>int</c> corresponding to the new index for the layer to be moved to.
	/// </param>
	public MoveLayerAction (List<GameObject> relatedObjects, int oldIndex, int newIndex) {
		base.Type = ActionType.MoveLayer;
		base.RelatedObjects = relatedObjects;
		_oldIndex = oldIndex;
		_newIndex = newIndex;
	}
}
