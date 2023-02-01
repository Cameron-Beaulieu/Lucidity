using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectMapObject : MonoBehaviour, IPointerClickHandler {

    public static GameObject SelectedObject;
    private static Outline _outline;
    private MapEditorManager _editor;
    
    private void Start() {
        _editor = GameObject.FindGameObjectWithTag("MapEditorManager")
            .GetComponent<MapEditorManager>();  
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (_editor.ToolStatus["Selection Tool"]) {
            SelectedObject = eventData.pointerClick;


            if (_outline) {
                Destroy(_outline);
            }

            _editor.SelectionOptions.SetActive(true);

            _outline = SelectedObject.AddComponent<Outline>();
            _outline.OutlineMode = Outline.Mode.OutlineAll;
            _outline.OutlineColor = Color.red;
            _outline.OutlineWidth = 2f;
        }
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
}
