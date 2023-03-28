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
            if (MapEditorManager.Layers[MapEditorManager.CurrentLayer].ContainsKey(id)
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
        MapEditorManager.Layers[layer].Remove(SelectedObject.GetInstanceID());
        SelectedObject.GetComponent<Image>().color = Color.white;
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
                        new DeleteMapObjectAction(objectsToDelete, layer, 
                            MapEditorManager.MapObjects[SelectedObject.GetInstanceID()]));
                    MapEditorManager.CurrentAction = MapEditorManager.CurrentAction.Next;
                } else if (MapEditorManager.CurrentAction != null) {
                    MapEditorManager.Actions.AddAfter(MapEditorManager.CurrentAction, 
                        new DeleteMapObjectAction(objectsToDelete, layer, 
                            MapEditorManager.MapObjects[SelectedObject.GetInstanceID()]));
                    MapEditorManager.CurrentAction = MapEditorManager.CurrentAction.Next;
                } else if (MapEditorManager.CurrentAction == null 
                           && MapEditorManager.Actions != null) {
                    // There is only one action and it has been undone
                    MapEditorManager.PermanentlyDeleteActions(MapEditorManager.Actions.First);
                    MapEditorManager.Actions.Clear();
                    MapEditorManager.Actions.AddFirst(new DeleteMapObjectAction(objectsToDelete, 
                        layer, MapEditorManager.MapObjects[SelectedObject.GetInstanceID()]));
                    MapEditorManager.CurrentAction = MapEditorManager.Actions.First;
                }
        SelectedObject = null;
        Tool.SelectionOptions.SetActive(false);
    }

    public void RotateMapObject(bool isClockwise) {
        if (SelectedObject != null) {
            AssetCollision collisionScript = SelectedObject.GetComponent<AssetCollision>();
            float originalRotation = SelectedObject.transform.parent.rotation.z;
            float newRotation = originalRotation;
            if (isClockwise) {
                newRotation-= 90;
                SelectedObject.transform.parent.Rotate(0, 0, -90);
            } else {
                newRotation += 90;
                SelectedObject.transform.parent.Rotate(0, 0, 90);
            }

            // add to MapObjects and Layers
            MapEditorManager.MapObjects[SelectedObject.GetInstanceID()].Rotation = new Quaternion(
                SelectedObject.transform.parent.rotation.x, 
                SelectedObject.transform.parent.rotation.y, 
                SelectedObject.transform.parent.rotation.z, 
                SelectedObject.transform.parent.rotation.w);
            MapEditorManager.Layers[MapEditorManager.CurrentLayer][SelectedObject.GetInstanceID()].Rotation = 
                new Quaternion(SelectedObject.transform.parent.rotation.x, 
                SelectedObject.transform.parent.rotation.y, 
                SelectedObject.transform.parent.rotation.z, 
                SelectedObject.transform.parent.rotation.w);

            bool isColliding = collisionScript.RotationCausesCollision(isClockwise, SelectedObject);
            if (isColliding) {
                Debug.Log("colliding");

                // take out from MapObjects and Layers
            } else {
                // add to actions history
                RotateMapObjectAction action = new RotateMapObjectAction(new List<(int, GameObject)>{(SelectedObject.GetInstanceID(), SelectedObject)}, 
                    originalRotation, newRotation);
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
                                                      action);
                    MapEditorManager.CurrentAction = MapEditorManager.CurrentAction.Next;
                } else if (MapEditorManager.CurrentAction != null) {
                    MapEditorManager.Actions.AddAfter(MapEditorManager.CurrentAction, 
                                                      action);
                    MapEditorManager.CurrentAction = MapEditorManager.CurrentAction.Next;
                } else if (MapEditorManager.CurrentAction == null 
                           && MapEditorManager.Actions != null) {
                    // There is only one action and it has been undone
                    MapEditorManager.PermanentlyDeleteActions(MapEditorManager.Actions.First);
                    MapEditorManager.Actions.Clear();
                    MapEditorManager.Actions.AddFirst(action);
                    MapEditorManager.CurrentAction = MapEditorManager.Actions.First;
                }
            }
        }
    }
}
