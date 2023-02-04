using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectMapObject : MonoBehaviour, IPointerClickHandler {

	public static GameObject SelectedObject;
	private static Outline _outline;

	public void OnPointerClick(PointerEventData eventData) {
		if (Tool.ToolStatus["Selection Tool"]) {
			SelectedObject = eventData.pointerClick;
			if (_outline != null) {
				Destroy(_outline);
			}
			Tool.SelectionOptions.SetActive(true);
			_outline = SelectedObject.AddComponent<Outline>();
			_outline.OutlineMode = Outline.Mode.OutlineAll;
			_outline.OutlineColor = Color.red;
			_outline.OutlineWidth = 2f;
		}
	}

	/// <summary>
	/// Deletes the selected map object.
	/// </summary>
	public void DeleteMapObject() {
		SelectedObject.SetActive(false);
		Destroy(_outline);
		List<GameObject> objectsToDelete = new List<GameObject>(){SelectedObject};
		MapEditorManager.Actions.AddAfter(MapEditorManager.CurrentAction, new DeleteMapObjectAction(objectsToDelete));
		MapEditorManager.CurrentAction = MapEditorManager.CurrentAction.Next;
		SelectedObject = null;
		Tool.SelectionOptions.SetActive(false);
	}
}
