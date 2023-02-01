using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectMapObject : MonoBehaviour, IPointerClickHandler {

    public static GameObject SelectedObject;
    private static Outline _outline;
    private MapEditorManager _editor;
    private Slider _scaleSlider;
    
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
        Quaternion oldRotation = SelectedObject.transform.rotation;
        Quaternion newRotation = Quaternion.Euler(oldRotation.x, oldRotation.y, oldRotation.z - 45);
        SelectedObject.transform.rotation = newRotation;
        Debug.Log(oldRotation.eulerAngles);
        Debug.Log(newRotation.eulerAngles);
        Debug.Log("cw clicked");
    }

    public void RotateMapObjectCCW() {
        Quaternion oldRotation = SelectedObject.transform.rotation;
        Quaternion newRotation = Quaternion.Euler(oldRotation.x, oldRotation.y, oldRotation.z + 45);
        SelectedObject.transform.rotation = newRotation;
        Debug.Log(oldRotation.eulerAngles);
        Debug.Log(newRotation.eulerAngles);
        Debug.Log("ccw clicked");
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
