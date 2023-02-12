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
    private SliderDrag _slider;
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
        Debug.Log((SelectedObject.transform.localScale).ToString("0.0" + "x"));

        // I'm not sure why this is needed here but not in the ScaleMapObject method
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
        Vector3 oldSize = SelectedObject.transform.localScale;
        Vector3 newSize = new Vector3(value, value, 1);
        SelectedObject.transform.localScale = newSize;
        // for UNDO/REDO
        // List<GameObject> objectsToScale = new List<GameObject>() { SelectedObject };
        // _slider.OnPointerClick.AddEventListener(EndDrag => {
        //     _editor.Actions.AddAfter(_editor.CurrentAction, new ResizeMapObjectAction(objectsToScale, oldSize, newSize));
        //     _editor.CurrentAction = _editor.CurrentAction.Next;
        // });
        List<GameObject> objectsToScale = new List<GameObject>() { SelectedObject };
        _editor.Actions.AddAfter(_editor.CurrentAction, new ResizeMapObjectAction(objectsToScale, oldSize, newSize));
        _editor.CurrentAction = _editor.CurrentAction.Next;
        _sliderText.text = (_scaleSliderValue.value).ToString("0.0" + "x");
    }
}
