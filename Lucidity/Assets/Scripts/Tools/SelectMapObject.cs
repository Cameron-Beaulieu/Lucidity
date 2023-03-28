using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectMapObject : MonoBehaviour, IPointerClickHandler {
    public static GameObject SelectedObject;
    public static bool IsTesting = false;

    public void OnPointerClick(PointerEventData eventData) {
        MapEditorManager.Reversion = false;
        MapEditorManager.LoadFlag = false;
        if (Tool.ToolStatus["Selection Tool"]) {
            GameObject clickedObject;
            if (IsTesting) {
                clickedObject = gameObject;
            } else {
                clickedObject = eventData.pointerClick;
            }
            int id = clickedObject.GetInstanceID();
            MapEditorManager editor = GameObject.Find("MapEditorManager")
                .GetComponent<MapEditorManager>();
            if (MapEditorManager.Layers[editor.CurrentLayer].ContainsKey(id)
                    || clickedObject.name == "Spawn Point") {
                if (SelectedObject != null) {
                    UnselectMapObject();
                }
                SelectedObject = clickedObject;
                if (SelectedObject.name == "Spawn Point") {
                    Tool.SpawnPointOptions.SetActive(true);
                    Tool.SelectionOptions.SetActive(false);
                } else {
                    Tool.SpawnPointOptions.SetActive(false);
                    Tool.SelectionOptions.SetActive(true);
                }
                GameObject.Find("SelectedObjectLabel").GetComponent<TMPro.TextMeshProUGUI>().text 
                    = "Editing " + SelectedObject.name;
                SelectedObject.GetComponent<Image>().color = new Color32(73, 48, 150, 255);
                if (SelectedObject.name != "Spawn Point") {
                    GameObject.Find("ScaleContainer/Slider").GetComponent<ResizeMapObject>().UpdateScaleText(
                        SelectedObject.transform.localScale.x);
                }
            }
        }
    }

    public static void UnselectMapObject() {
        if (SelectedObject != null) {
            SelectedObject.GetComponent<Image>().color = Color.white;
            SelectedObject = null;
            Tool.SelectionOptions.SetActive(false);
            Tool.SpawnPointOptions.SetActive(false);
        }
    }

    /// <summary>
    /// Deletes the selected map object.
    /// </summary>
    public void DeleteMapObject() {
        MapEditorManager.MapObjects[SelectedObject.GetInstanceID()].IsActive = false;
        int layer = MapEditorManager.LayerContainsMapObject(SelectedObject.GetInstanceID());
        MapEditorManager.Layers[layer][SelectedObject.GetInstanceID()].IsActive = false;
        SelectedObject.SetActive(false);
        List<(int, GameObject)> objectsToDelete = 
            new List<(int, GameObject)>(){(SelectedObject.GetInstanceID(), SelectedObject)};
        if (MapEditorManager.CurrentAction != null 
            && MapEditorManager.CurrentAction.Next != null) {
                    // These actions can no longer be redone
                    MapEditorManager.PermanentlyDeleteActions(MapEditorManager.CurrentAction.Next);
                    LinkedListNode<EditorAction> actionToRemove = 
                        MapEditorManager.CurrentAction.Next;
                    while (actionToRemove != null) {
                        LinkedListNode<EditorAction> temp = actionToRemove.Next;
                        MapEditorManager.Actions.Remove(actionToRemove);
                        actionToRemove = temp;
                    }
                    MapEditorManager.Actions.AddAfter(MapEditorManager.CurrentAction, 
                                                      new DeleteMapObjectAction(objectsToDelete));
                    MapEditorManager.CurrentAction = MapEditorManager.CurrentAction.Next;
                } else if (MapEditorManager.CurrentAction != null) {
                    MapEditorManager.Actions.AddAfter(MapEditorManager.CurrentAction, 
                                                      new DeleteMapObjectAction(objectsToDelete));
                    MapEditorManager.CurrentAction = MapEditorManager.CurrentAction.Next;
                } else if (MapEditorManager.CurrentAction == null 
                           && MapEditorManager.Actions != null) {
                    // There is only one action and it has been undone
                    MapEditorManager.PermanentlyDeleteActions(MapEditorManager.Actions.First);
                    MapEditorManager.Actions.Clear();
                    MapEditorManager.Actions.AddFirst(new DeleteMapObjectAction(objectsToDelete));
                    MapEditorManager.CurrentAction = MapEditorManager.Actions.First;
                }
        SelectedObject = null;
        Tool.SelectionOptions.SetActive(false);
    }
}
