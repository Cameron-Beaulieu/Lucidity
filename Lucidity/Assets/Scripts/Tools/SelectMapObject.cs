using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectMapObject : MonoBehaviour, IPointerClickHandler {
    public static GameObject SelectedObject;
    public static bool IsTesting = false;
    private static Outline _outline;

    public void OnPointerClick(PointerEventData eventData) {
        if (Tool.ToolStatus["Selection Tool"]) {
            GameObject clickedObject;
            if (IsTesting) {
                clickedObject = SelectedObject;
            } else {
                clickedObject = eventData.pointerClick;
            }
            int id = clickedObject.GetInstanceID();
            MapEditorManager editor = GameObject.FindGameObjectWithTag("MapEditorManager")
                .GetComponent<MapEditorManager>();
            // Check if the selected object is on the current layer, or if it is the spawn point
            // TODO: uncomment if statement and swap for other one once layer reversion complete
            // if (MapEditorManager.Layers[editor.CurrentLayer].ContainsKey(id)
            //         || clickedObject.name == "Spawn Point") {
            if (MapEditorManager.MapObjects.ContainsKey(id)
                    || clickedObject.name == "Spawn Point") {
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
        Tool.SelectionOptions.SetActive(false);
        Tool.SpawnPointOptions.SetActive(false);
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
