using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapEditorManager : MonoBehaviour {
	public List<AssetController> AssetButtons;
	public List<GameObject> AssetPrefabs;
	public List<GameObject> AssetImage;
	public List<Texture2D> CursorTextures;
	public static LinkedList<EditorAction> Actions;
	public static Dictionary<string, Texture2D> ToolToCursorMap = new Dictionary<string, Texture2D>();
	private static LinkedListNode<EditorAction> _currentAction;
	private static GameObject _map;
	private static GameObject _mapContainer;
	private static int _currentButtonPressed;
	private static GameObject _lastEncounteredObject;

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
		_mapContainer = GameObject.Find("Map Container");
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
		foreach (Texture2D cursor in CursorTextures) {
			ToolToCursorMap.Add(cursor.name, cursor);
		}

		CreateNewMap.SizeType mapSize = CreateNewMap.Size;
		RectTransform mapRect = _map.GetComponent<RectTransform>();
		Vector2 mapScale = _map.transform.localScale;

		switch (mapSize) {
		  case CreateNewMap.SizeType.Small:
			mapScale *= 1f;
			break;
		  case CreateNewMap.SizeType.Medium:
			mapScale *= 1.5f;
			break;
		  case CreateNewMap.SizeType.Large:
			mapScale *= 2f;
			break;
		  default:
			mapScale *= 1.5f;
			break;
		}
		_map.transform.localScale = mapScale;
	}

	void Update() {
		Vector2 worldPosition = Mouse.getMousePosition();
		if (Input.GetMouseButton(0) && AssetButtons[_currentButtonPressed].Clicked 
			&& Tool.ToolStatus["Brush Tool"]) {
			GameObject activeImage = GameObject.FindGameObjectWithTag("AssetImage");
			if (activeImage == null) {
				DynamicBoundingBox.CreateDynamicAssetImage(AssetImage[_currentButtonPressed]);
				activeImage = GameObject.FindGameObjectWithTag("AssetImage");
			}
			float assetWidth = activeImage.transform.localScale.x;
			float assetHeight = activeImage.transform.localScale.y;
			// Check if mouse position relative to its last position and the previously encountered
			// asset would allow for a legal placement. Reduces unnecessary computing
			if (Mouse.LastMousePosition != worldPosition
					&& (LastEncounteredObject == null
						|| Mathf.Abs(worldPosition.x - LastEncounteredObject.transform.position.x)
							>= assetWidth
						|| Mathf.Abs(worldPosition.y - LastEncounteredObject.transform.position.y)
							>= assetHeight)) {
				List<GameObject> mapObjects = new List<GameObject>();
				GameObject newParent = new GameObject();
				newParent.name = AssetPrefabs[_currentButtonPressed].name + " Parent";
				newParent.transform.SetParent(_mapContainer.transform, true);
				newParent.transform.localPosition = new Vector3(
					newParent.transform.localPosition.x,
					newParent.transform.localPosition.y, 0);				
				
				GameObject dynamicBoundingBox = (GameObject) Instantiate(
					AssetPrefabs[_currentButtonPressed],
					new Vector3(worldPosition.x, worldPosition.y, 90), // TODO: why 90 again
					Quaternion.identity);
				dynamicBoundingBox.transform.localScale = 
					new Vector3(dynamicBoundingBox.transform.localScale.x
						+ Zoom.zoomFactor, dynamicBoundingBox.transform.localScale.y
						+ Zoom.zoomFactor, dynamicBoundingBox.transform.localScale.z
						+ Zoom.zoomFactor)
					* DynamicBoundingBox.DynamicSideLength * AssetOptions.BrushSize;
				// dynamicBoundingBox.GetComponent<AssetCollision>().CheckAssetCollisions();
				if (dynamicBoundingBox != null
						&& !dynamicBoundingBox.GetComponent<AssetCollision>().IsInvalidPlacement()
						&& dynamicBoundingBox.GetComponent<AssetCollision>().Collided == false) {
					Debug.Log("Before invocation");
					Debug.Log("Return from invocation: " + dynamicBoundingBox.GetComponent<AssetCollision>().GetCollisionCount());
					Debug.Log("After invocation");
				// 	//Destroy(dynamicBoundingBox);
				// 	foreach (KeyValuePair<int,int> coordinate
				// 			in AssetOptions.RandomAssetArrangement) {
				// 		float xPos = (DynamicBoundingBox.Images[coordinate.Key,coordinate.Value])
				// 						.transform.position.x;
				// 		float yPos = (DynamicBoundingBox.Images[coordinate.Key,coordinate.Value])
				// 						.transform.position.y;
				// 		float zPos = (DynamicBoundingBox.Images[coordinate.Key,coordinate.Value])
				// 						.transform.position.z;

				// 		GameObject newGameObject = Instantiate(
				// 			AssetPrefabs[_currentButtonPressed],
				// 			new Vector3(xPos, yPos, zPos),
				// 			Quaternion.identity, newParent.transform);
				// 		if (newGameObject != null && !newGameObject.GetComponent<AssetCollision>()
				// 				.IsInvalidPlacement()) {
				// 			mapObjects.Add(newGameObject);
				// 		} else {
				// 			Destroy(newGameObject);
				// 			Destroy(newParent);
				// 		}
				// 	}
				} else {
					Debug.Log("Did not enter conditional");
					Destroy(dynamicBoundingBox);
					Destroy(newParent);
				}
				if (mapObjects.Count == 0) {
					// Don't add action to history if there are no objects attached to it
				} else if (Actions == null) {
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
				if (mapObjects.Count > 0) {
					_lastEncounteredObject = mapObjects[0];
				}
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
	private void PermanentlyDeleteActions(LinkedListNode<EditorAction> actionToDelete) {
		while (actionToDelete != null) {
			if (actionToDelete.Value.Type == EditorAction.ActionType.Paint) {
				foreach (GameObject obj in actionToDelete.Value.RelatedObjects) {
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
	public void Undo() {
		if (_currentAction != null) {
			EditorAction actionToUndo = _currentAction.Value;
			switch (actionToUndo.Type) {
				case EditorAction.ActionType.Paint:
					foreach (GameObject obj in actionToUndo.RelatedObjects) {
						if (obj != null) {
							obj.SetActive(false);
						}
					}
					break;
				case EditorAction.ActionType.DeleteMapObject:
					foreach (GameObject obj in actionToUndo.RelatedObjects) {
						if (obj != null) {
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
			if (_currentAction.Previous != null) {
				_currentAction = _currentAction.Previous;
			}
			else {
				_currentAction = null;
			}
		}
	}
}
