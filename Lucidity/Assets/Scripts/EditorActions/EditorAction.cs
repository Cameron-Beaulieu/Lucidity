using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EditorAction {
	public enum ActionType
	{
		Paint,
		DeleteMapObject,
		MoveMapObject,
		ResizeMapObject,
		RotateMapObject,
		CreateLayer,
		DeleteLayer,
		MoveLayer,
		RenameLayer
	}
	private List<GameObject> _relatedObjects = new List<GameObject>();
	private ActionType _type;
	
	public List<GameObject> RelatedObjects {
		get { return _relatedObjects; }
		set { _relatedObjects = value; }
	}

	public ActionType Type {
		get { return _type; }
		set { _type = value; }
	}
}
