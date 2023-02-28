using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectMapObject : MonoBehaviour, IPointerClickHandler {
    public static GameObject SelectedObject;
    private static Outline _outline;

    public void OnPointerClick(PointerEventData eventData) {
        if (Tool.ToolStatus["Selection Tool"]) {
            MapEditorManager editor = GameObject.FindGameObjectWithTag("MapEditorManager")
                .GetComponent<MapEditorManager>();
            GameObject clickedObject = eventData.pointerClick;
            int id = clickedObject.GetInstanceID();
            // First check if the selected object is on the current layer
            if (MapEditorManager.Layers[editor.CurrentLayer].ContainsKey(id)) {
                SelectedObject = clickedObject;
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
                _outline = SelectedObject.AddComponent<Outline>();
                _outline.OutlineMode = Outline.Mode.OutlineAll;
                _outline.OutlineColor = Color.red;
                _outline.OutlineWidth = 2f;
            }
        }
    }

    public static void UnselectMapObject() {
        if (SelectedObject != null) {
            SelectedObject = null;
        }
        if (_outline != null) {
            Destroy(_outline);
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
            MapEditorManager.Actions.AddAfter(MapEditorManager.CurrentAction, 
                                              new DeleteMapObjectAction(objectsToDelete));
            MapEditorManager.CurrentAction = MapEditorManager.CurrentAction.Next;
        } else {
            MapEditorManager.Actions = new LinkedList<EditorAction>();
            MapEditorManager.Actions.AddFirst(new DeleteMapObjectAction(objectsToDelete));
            MapEditorManager.CurrentAction = MapEditorManager.Actions.First;
        }
        SelectedObject = null;
        Tool.SelectionOptions.SetActive(false);
    }
}
