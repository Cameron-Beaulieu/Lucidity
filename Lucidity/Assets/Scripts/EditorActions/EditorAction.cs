using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EditorAction {
	private ActionType _type;
	private List<GameObject> _relatedObjects = new List<GameObject>();
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

	public ActionType Type {
		get { return _type; }
		set { _type = value; }
	}
	
	public List<GameObject> RelatedObjects {
		get { return _relatedObjects; }
		set { _relatedObjects = value; }
	}
}
