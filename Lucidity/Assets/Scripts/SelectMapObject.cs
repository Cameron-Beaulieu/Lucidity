using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectMapObject : MonoBehaviour, IPointerClickHandler {

    private static GameObject _selectedObject;
    private static Outline _outline;
    private MapEditorManager _editor;
    
    private void Start() {
        _editor = GameObject.FindGameObjectWithTag("MapEditorManager")
            .GetComponent<MapEditorManager>();
    }

    public void OnPointerClick(PointerEventData eventData) {
        _selectedObject = eventData.pointerClick;

        if (_outline) {
            Destroy(_outline);
        }

        _editor.ChangeTools("Selection Tool");

        _outline = _selectedObject.AddComponent<Outline>();
        _outline.OutlineMode = Outline.Mode.OutlineAll;
        _outline.OutlineColor = Color.red;
        _outline.OutlineWidth = 2f;
    }

    public void DeleteMapObject() {
        _selectedObject.SetActive(false);
        Destroy(_outline);
        List<GameObject> objectsToDelete = new List<GameObject>() {_selectedObject};
        MapEditorManager.Actions.AddAfter(MapEditorManager.CurrentAction, new DeleteMapObjectAction(objectsToDelete));
        MapEditorManager.CurrentAction = MapEditorManager.CurrentAction.Next;
    }
}
