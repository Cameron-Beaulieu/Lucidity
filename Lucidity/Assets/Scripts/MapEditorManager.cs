using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapEditorManager : MonoBehaviour {
	public static LinkedList<EditorAction> Actions;
	public List<AssetController> AssetButtons;
	public List<GameObject> AssetPrefabs;
	public List<GameObject> AssetImage;
	private static LinkedListNode<EditorAction> _currentAction;
	private static int _currentButtonPressed;
	private static GameObject _lastEncounteredObject;
	private static GameObject _map;

	public static LinkedListNode<EditorAction> CurrentAction {
		get { return _currentAction; }
		set { _currentAction = value; }
	}

	public static int CurrentButtonPressed {
		get { return _currentButtonPressed; }
		set { _currentButtonPressed = value; }
	}

	public static GameObject LastEncounteredObject {
		get { return _lastEncounteredObject; }
		set { _lastEncounteredObject = value; }
	}

	void Awake() {
		_map = GameObject.Find("Map");
		Tool.PaintingMenu = GameObject.Find("Painting Menu");
		Tool.SelectionMenu = GameObject.Find("Selection Menu");
		Tool.SelectionOptions = GameObject.FindGameObjectWithTag("SelectionScrollContent");
		Tool.SelectionMenu.SetActive(false);
		GameObject.Find("Undo").GetComponent<Button>().onClick.AddListener(Undo);
		GameObject.Find("Redo").GetComponent<Button>().onClick.AddListener(Redo);
		GameObject[] selectableTools = GameObject.FindGameObjectsWithTag("SelectableTool");
		foreach (GameObject tool in selectableTools) {
			if (tool.name == "Brush Tool") {
				Tool.ToolStatus.Add(tool.name, true);
			} else {
				Tool.ToolStatus.Add(tool.name, false);
			}
			Tool.ToolKeys.Add(tool.name);
		}

		CreateNewMap.SizeType mapSize = CreateNewMap.Size;
		RectTransform mapRect = _map.GetComponent<RectTransform>();

		switch (mapSize) {
		  case CreateNewMap.SizeType.Small:
			_map.transform.localScale = new Vector2(1f, 1f);
			break;
		  case CreateNewMap.SizeType.Medium:
			_map.transform.localScale = new Vector2(1.5f, 1.5f);
			break;
		  case CreateNewMap.SizeType.Large:
			_map.transform.localScale = new Vector2(2f, 2f);
			break;
		  default:
			_map.transform.localScale = new Vector2(1.5f, 1.5f);
			break;
		}
	}

	void Update() {
		Vector2 worldPosition = Mouse.getMousePosition();
		if (Input.GetMouseButton(0) && AssetButtons[_currentButtonPressed].Clicked) {
			float assetWidth = AssetPrefabs[_currentButtonPressed].transform.localScale.x;
			float assetHeight = AssetPrefabs[_currentButtonPressed].transform.localScale.y;
			// Check if mouse position relative to its last position and the previously encountered
			// asset would allow for a legal placement. Reduces unnecessary computing
			if (Mouse.LastMousePosition != worldPosition
				&& (LastEncounteredObject == null
					|| Mathf.Abs(worldPosition.x - LastEncounteredObject.transform.position.x)
						>= assetWidth
					|| Mathf.Abs(worldPosition.y - LastEncounteredObject.transform.position.y)
						>= assetHeight))
			{
				List<GameObject> mapObjects = new List<GameObject>();
				for (int i = 0; i < AssetOptions.AssetCount; i++) {
					GameObject newGameObject = (GameObject) Instantiate(
						AssetPrefabs[_currentButtonPressed],
						new Vector3(worldPosition.x + i*2, worldPosition.y, 0),
						Quaternion.identity
					);
					if (newGameObject != null) {
						mapObjects.Add(newGameObject);
					}
				}
				if (Actions == null) {
					Actions = new LinkedList<EditorAction>();
					Actions.AddFirst(new PaintAction(mapObjects));
					_currentAction = Actions.First;
				} else {
					if (_currentAction != null && _currentAction.Next != null) {
						// These actions can no longer be redone
						PermanentlyDeleteActions(_currentAction.Next);
						LinkedListNode<EditorAction> actionToRemove = _currentAction.Next;
						while (actionToRemove != null) {
							Actions.Remove(actionToRemove);
							actionToRemove = actionToRemove.Next;
						}
						Actions.AddAfter(_currentAction, new PaintAction(mapObjects));
						_currentAction = _currentAction.Next;
					} else if (_currentAction != null) {
						Actions.AddAfter(_currentAction, new PaintAction(mapObjects));
						_currentAction = _currentAction.Next;
					} else if (_currentAction == null && Actions != null) {
						// There is only one action and it has been undone
						PermanentlyDeleteActions(Actions.First);
						Actions.Clear();
						Actions.AddFirst(new PaintAction(mapObjects));
						_currentAction = Actions.First;
					}
				}
				_lastEncounteredObject = mapObjects[0];
			}
			Mouse.LastMousePosition = worldPosition;
		}
		// TODO: Implement other actions here
	}

	/// <summary>
	/// Permanently deletes all map objects associated with actions in the list. This is done to
	/// ensure no inactive map objects associated with actions that can no longer be redone are
	/// left in the scene.
	/// </summary>
	/// <param name="actionToDelete">
	/// <c>LinkedListNode</c> of the <c>EditorAction</c> to remove from the linked list (and its
	/// associated actions).
	/// </param>
	private void PermanentlyDeleteActions(LinkedListNode<EditorAction> actionToDelete)
	{
		while (actionToDelete != null)
		{
			if (actionToDelete.Value.Type == EditorAction.ActionType.Paint)
			{
				foreach (GameObject obj in actionToDelete.Value.RelatedObjects)
				{
					Destroy(obj);
				}
			}
			actionToDelete = actionToDelete.Next;
		}
	}

	/// <summary>
	/// Action redo functionality.
	/// </summary>
	public void Redo() {
		if ((_currentAction == null && Actions != null) || _currentAction.Next != null) {
			// If CurrentAction is null and Actions is not, then we want to redo from the
			// beginning; else, we want to redo from the current section
			EditorAction actionToRedo = Actions.First.Value;
			if (_currentAction != null) {
				actionToRedo = _currentAction.Next.Value;
			}
			switch(actionToRedo.Type) {
				case EditorAction.ActionType.Paint:
					foreach (GameObject obj in actionToRedo.RelatedObjects) {
						if (obj != null) {
							obj.SetActive(true);
						}
					}
					break;
				case EditorAction.ActionType.DeleteMapObject:
					foreach (GameObject obj in actionToRedo.RelatedObjects) {
						if (obj != null) {
							obj.SetActive(false);
						}
					}
					break;
				case EditorAction.ActionType.MoveMapObject:
					// TODO: Implement
					break;
				case EditorAction.ActionType.ResizeMapObject:
					// TODO: Implement
					break;
				case EditorAction.ActionType.RotateMapObject:
					// TODO: Implement
					break;
				case EditorAction.ActionType.CreateLayer:
					// TODO: Implement
					break;
				case EditorAction.ActionType.DeleteLayer:
					// TODO: Implement
					break;
				case EditorAction.ActionType.MoveLayer:
					// TODO: Implement
					break;
				case EditorAction.ActionType.RenameLayer:
					// TODO: Implement
					break;
			}
			if (_currentAction == null) {
				_currentAction = Actions.First;
			} else {
				_currentAction = _currentAction.Next;
			}
		}
	}

	/// <summary>
	/// Action redo functionality.
	/// </summary>
	public void Undo()
	{
		if (_currentAction != null)
		{
			EditorAction actionToUndo = _currentAction.Value;
			switch (actionToUndo.Type)
			{
				case EditorAction.ActionType.Paint:
					foreach (GameObject obj in actionToUndo.RelatedObjects)
					{
						if (obj != null)
						{
							obj.SetActive(false);
						}
					}
					break;
				case EditorAction.ActionType.DeleteMapObject:
					foreach (GameObject obj in actionToUndo.RelatedObjects)
					{
						if (obj != null)
						{
							obj.SetActive(true);
						}
					}
					break;
				case EditorAction.ActionType.MoveMapObject:
					// TODO: Implement
					break;
				case EditorAction.ActionType.ResizeMapObject:
					// TODO: Implement
					break;
				case EditorAction.ActionType.RotateMapObject:
					// TODO: Implement
					break;
				case EditorAction.ActionType.CreateLayer:
					// TODO: Implement
					break;
				case EditorAction.ActionType.DeleteLayer:
					// TODO: Implement
					break;
				case EditorAction.ActionType.MoveLayer:
					// TODO: Implement
					break;
				case EditorAction.ActionType.RenameLayer:
					// TODO: Implement
					break;
			}
			if (_currentAction.Previous != null)
			{
				_currentAction = _currentAction.Previous;
			}
			else
			{
				_currentAction = null;
			}
		}
	}
}
