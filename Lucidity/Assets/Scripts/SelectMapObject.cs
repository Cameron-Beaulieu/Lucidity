using UnityEngine;
using UnityEngine.EventSystems;

public class SelectMapObject : MonoBehaviour, IPointerClickHandler {

    private GameObject _selectedObject;
    // private static GameObject _prevSelectedObject;
    private static Outline _outline;
    private MapEditorManager _editor;
    
    private void Awake() {
        _editor = GameObject.FindGameObjectWithTag("MapEditorManager")
            .GetComponent<MapEditorManager>();
    }

    public void OnPointerClick(PointerEventData eventData) {
        _selectedObject = eventData.pointerClick;

        // if (_prevSelectedObject == null) {
        //     _prevSelectedObject = _selectedObject;
        // } else {
        //     Destroy(_outline);
        // }

        if (_outline) {
            Destroy(_outline);
        }

        _editor.ChangeTools("Selection Tool");

        _outline = _selectedObject.AddComponent<Outline>();
        _outline.OutlineMode = Outline.Mode.OutlineAll;
        _outline.OutlineColor = Color.red;
        _outline.OutlineWidth = 2f;
    }
}