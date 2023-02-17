using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectMapObject : MonoBehaviour, IPointerClickHandler {

    public static GameObject SelectedObject;
    private static Outline _outline;
    private MapEditorManager _editor;
    public static float SliderValue;
    [SerializeField] private Slider _scaleSliderValue;
    [SerializeField] private TMP_Text _sliderText;
    
    private void Start() {
        _editor = GameObject.FindGameObjectWithTag("MapEditorManager")
            .GetComponent<MapEditorManager>();
    }

    public void OnPointerClick(PointerEventData eventData) {
        SelectedObject = eventData.pointerClick;

        if (_outline) {
            Destroy(_outline);
        }

        _editor.ChangeTools("Selection Tool");
        _editor.SelectionOptions.SetActive(true);

        _sliderText = GameObject.Find("ValueText").GetComponent<TMP_Text>();
        _sliderText.text = (SelectedObject.transform.localScale.x).ToString("0.0" + "x");

        _outline = SelectedObject.AddComponent<Outline>();
        _outline.OutlineMode = Outline.Mode.OutlineAll;
        _outline.OutlineColor = Color.red;
        _outline.OutlineWidth = 2f;
    }

    public void DeleteMapObject() {
        SelectedObject.SetActive(false);
        Destroy(_outline);
        List<GameObject> objectsToDelete = new List<GameObject>() {SelectedObject};
        _editor.Actions.AddAfter(_editor.CurrentAction, new DeleteMapObjectAction(objectsToDelete));
        _editor.CurrentAction = _editor.CurrentAction.Next;
        SelectedObject = null;
        _editor.SelectionOptions.SetActive(false);
    }

    public void RotateMapObjectCW() {
        Quaternion oldRotation = SelectedObject.transform.rotation;
        // will change to 90 degrees once we have real assets
        SelectedObject.transform.Rotate(0.0f, 0.0f, -45.0f, Space.World);
        // for UNDO/REDO
        List<GameObject> objectsToRotate = new List<GameObject>() { SelectedObject };
        _editor.Actions.AddAfter(_editor.CurrentAction, new RotateMapObjectAction(objectsToRotate, true));
        _editor.CurrentAction = _editor.CurrentAction.Next;
    }

    public void RotateMapObjectCCW() {
        Quaternion oldRotation = SelectedObject.transform.rotation;
        // will change to 90 degrees once we have real assets
        SelectedObject.transform.Rotate(0.0f, 0.0f, 45.0f, Space.World);
        // for UNDO/REDO
        List<GameObject> objectsToRotate = new List<GameObject>() { SelectedObject };
        _editor.Actions.AddAfter(_editor.CurrentAction, new RotateMapObjectAction(objectsToRotate, false));
        _editor.CurrentAction = _editor.CurrentAction.Next;
    }

    public void ScaleMapObject(float value) {
        // update the slider text value
        _sliderText.text = (_scaleSliderValue.value).ToString("0.0" + "x");
        SliderValue = _scaleSliderValue.value;
    }
}
