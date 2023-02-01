using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectMapObject : MonoBehaviour, IPointerClickHandler {

    public static GameObject SelectedObject;
    private static Outline _outline;
    private MapEditorManager _editor;
    
    private void Start() {
        _editor = GameObject.FindGameObjectWithTag("MapEditorManager")
            .GetComponent<MapEditorManager>();
        //ScaleSlider = GameObject.FindGameObjectWithTag("ScaleSlider");
    }

    public void OnPointerClick(PointerEventData eventData) {
        SelectedObject = eventData.pointerClick;


        if (_outline) {
            Destroy(_outline);
        }

        _editor.ChangeTools("Selection Tool");
        _editor.SelectionOptions.SetActive(true);

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
        bool IsClockwise = true;
        Quaternion oldRotation = SelectedObject.transform.rotation;
        SelectedObject.transform.Rotate(0.0f, 0.0f, -45.0f, Space.World);
        // for UNDO/REDO
        List<GameObject> objectsToRotate = new List<GameObject>() { SelectedObject };
        _editor.Actions.AddAfter(_editor.CurrentAction, new RotateMapObjectAction(objectsToRotate, IsClockwise));
        _editor.CurrentAction = _editor.CurrentAction.Next;
    }

    public void RotateMapObjectCCW() {
        bool IsClockwise = false;
        Quaternion oldRotation = SelectedObject.transform.rotation;
        SelectedObject.transform.Rotate(0.0f, 0.0f, 45.0f, Space.World);
        // for UNDO/REDO
        List<GameObject> objectsToRotate = new List<GameObject>() { SelectedObject };
        _editor.Actions.AddAfter(_editor.CurrentAction, new RotateMapObjectAction(objectsToRotate, IsClockwise));
        _editor.CurrentAction = _editor.CurrentAction.Next;
    }

    public void ScaleMapObject(float value) {
        Vector3 oldSize = SelectedObject.transform.localScale;
        Vector3 newSize = new Vector3(value, value, 1);
        SelectedObject.transform.localScale = newSize;
        //List<GameObject> objectsToScale = new List<GameObject>() { SelectedObject };
        //_editor.Actions.AddAfter(_editor.CurrentAction, new ResizeMapObjectAction(objectsToScale, oldSize, newSize));
        //_editor.CurrentAction = _editor.CurrentAction.Next;
    }
}
