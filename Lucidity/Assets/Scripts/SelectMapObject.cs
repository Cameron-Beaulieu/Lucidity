using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectMapObject : MonoBehaviour, IPointerClickHandler {
	public static GameObject SelectedObject;
	private static Outline _outline;
	public static float SliderValue;
	private MapEditorManager _editor;
	private LayerMask _filterMask;
	[SerializeField] private Slider _scaleSliderValue;
	[SerializeField] private TMP_Text _sliderText;

	private void Start() {
		_editor = GameObject.FindGameObjectWithTag("MapEditorManager")
			.GetComponent<MapEditorManager>();
	}

	public void OnPointerClick(PointerEventData eventData) {
		if (Tool.ToolStatus["Selection Tool"]) {
			SelectedObject = eventData.pointerClick;
			if (_outline != null) {
				Destroy(_outline);
			}
			if (SelectedObject.name == "Spawn Point") {
				Tool.SpawnPointOptions.SetActive(true);
				Tool.SelectionOptions.SetActive(false);
			} else {
				Tool.SpawnPointOptions.SetActive(false);
				Tool.SelectionOptions.SetActive(true);
			}
			GameObject.Find("SelectedObjectLabel").GetComponent<TMPro.TextMeshProUGUI>().text 
				= "Editing " + SelectedObject.name;

			_sliderText = GameObject.Find("ValueText").GetComponent<TMP_Text>();
			_scaleSliderValue = GameObject.Find("Slider").GetComponent<Slider>();
			_sliderText.text = (SelectedObject.transform.localScale.x).ToString("0.0" + "x");
			_scaleSliderValue.value = SelectedObject.transform.localScale.x;

			_outline = SelectedObject.AddComponent<Outline>();
			_outline.OutlineMode = Outline.Mode.OutlineAll;
			_outline.OutlineColor = Color.red;
			_outline.OutlineWidth = 2f;
		}
	}

	public static void DeselectObject() {
		if (SelectedObject != null) {
			SelectedObject = null;
			if (_outline != null) {
				Destroy(_outline);
			}
			Tool.SelectionOptions.SetActive(false);
			Tool.SpawnPointOptions.SetActive(false);
		}
	}

	/// <summary>
	/// Deletes the selected map object.
	/// </summary>
	public void DeleteMapObject() {
		MapEditorManager.MapObjects[SelectedObject.GetInstanceID()].IsActive = false;
		SelectedObject.SetActive(false);
		Destroy(_outline);
		List<GameObject> objectsToDelete = new List<GameObject>(){SelectedObject};
		// If a map was just loaded, deleting could be the first Action
		if (MapEditorManager.Actions != null) {
			MapEditorManager.Actions.AddAfter(MapEditorManager.CurrentAction, new DeleteMapObjectAction(objectsToDelete));
			MapEditorManager.CurrentAction = MapEditorManager.CurrentAction.Next;
		} else {
			MapEditorManager.Actions = new LinkedList<EditorAction>();
			MapEditorManager.Actions.AddFirst(new DeleteMapObjectAction(objectsToDelete));
			MapEditorManager.CurrentAction = MapEditorManager.Actions.First;
		}
		SelectedObject = null;
		Tool.SelectionOptions.SetActive(false);
	}

	public void RotateMapObjectCW() {
		Quaternion oldRotation = SelectedObject.transform.rotation;
		// will change to 90 degrees once we have real assets
		SelectedObject.transform.Rotate(0.0f, 0.0f, -45.0f, Space.World);
		// for UNDO/REDO
		List<GameObject> objectsToRotate = new List<GameObject>() { SelectedObject };
		MapEditorManager.Actions.AddAfter(MapEditorManager.CurrentAction, new RotateMapObjectAction(objectsToRotate, true));
		MapEditorManager.CurrentAction = MapEditorManager.CurrentAction.Next;
	}

	public void RotateMapObjectCCW() {
		Quaternion oldRotation = SelectedObject.transform.rotation;
		// will change to 90 degrees once we have real assets
		SelectedObject.transform.Rotate(0.0f, 0.0f, 45.0f, Space.World);
		// for UNDO/REDO
		List<GameObject> objectsToRotate = new List<GameObject>() { SelectedObject };
		MapEditorManager.Actions.AddAfter(MapEditorManager.CurrentAction, new RotateMapObjectAction(objectsToRotate, false));
		MapEditorManager.CurrentAction = MapEditorManager.CurrentAction.Next;
	}

	public void ScaleMapObject(float value) {
		// update the slider text value
		_sliderText.text = (_scaleSliderValue.value).ToString("0.0" + "x");
		SliderValue = _scaleSliderValue.value;
	}
}
