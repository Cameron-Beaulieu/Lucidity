using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ResizeMapObject : Slider, IPointerUpHandler {

    private TMP_Text _text;
    private AssetCollision _collisionScript;
    private float _currentSelectionOriginalScale;

    private new void Start() {
        _text = transform.parent.Find("ValueText").GetComponent<TMP_Text>();
        if (SelectMapObject.SelectedObject != null) {
            _collisionScript = SelectMapObject.SelectedObject.GetComponent<AssetCollision>();
        }
        onValueChanged.AddListener(OnValueChanged);
    }

    public void UpdateScaleText(float selectedObjectScale) {
        value = Mathf.Round(selectedObjectScale * 10f) / 10f;
        _currentSelectionOriginalScale = value;
    }

    public void OnValueChanged(float newValue) {
        if (SelectMapObject.SelectedObject != null) {
            float roundedValue = Mathf.Round(newValue * 10f) / 10f;
            _text.text = roundedValue + "x";
            Transform parent = SelectMapObject.SelectedObject.transform.parent;
            parent.localScale = new Vector3(Util.ParentAssetDefaultScale * roundedValue, 
                                            Util.ParentAssetDefaultScale * roundedValue, 1);
        }
    }

    public override void OnPointerUp(PointerEventData eventData) {
        bool isColliding = _collisionScript.ScaleCausesCollision(_currentSelectionOriginalScale, SelectMapObject.SelectedObject);
        if (isColliding) {
            value = _currentSelectionOriginalScale;
            float roundedValue = Mathf.Round(value * 10f) / 10f;
            _text.text = roundedValue + "x";
        } else {
            float newParentScale = Util.ParentAssetDefaultScale * value;
            float oldParentScale = Util.ParentAssetDefaultScale * _currentSelectionOriginalScale;
            MapEditorManager.MapObjects[SelectMapObject.SelectedObject.GetInstanceID()]
                .Scale = new Vector3(newParentScale, newParentScale, newParentScale);
            // change in layers as well

            // add to actions history
            List<(int, GameObject)> relatedObjects = 
                new List<(int, GameObject)>{(SelectMapObject.SelectedObject.GetInstanceID(), 
                                             SelectMapObject.SelectedObject)};
            ResizeMapObjectAction action = new ResizeMapObjectAction(relatedObjects, 
                                                                     oldParentScale, 
                                                                     newParentScale);
            if (MapEditorManager.CurrentAction != null 
                && MapEditorManager.CurrentAction.Next != null) {
                // these actions can no longer be redone
                MapEditorManager.PermanentlyDeleteActions(MapEditorManager.CurrentAction.Next);
                LinkedListNode<EditorAction> actionToRemove = MapEditorManager.CurrentAction.Next;
                while (actionToRemove != null) {
                    LinkedListNode<EditorAction> temp = actionToRemove.Next;
                    MapEditorManager.Actions.Remove(actionToRemove);
                    actionToRemove = temp;
                }
                MapEditorManager.Actions.AddAfter(MapEditorManager.CurrentAction, action);
                MapEditorManager.CurrentAction = MapEditorManager.CurrentAction.Next;
            } else if (MapEditorManager.CurrentAction != null) {
                MapEditorManager.Actions.AddAfter(MapEditorManager.CurrentAction, action);
                MapEditorManager.CurrentAction = MapEditorManager.CurrentAction.Next;
            } else if (MapEditorManager.CurrentAction == null 
                       && MapEditorManager.Actions != null) {
                // there is only one action and it has been undone
                MapEditorManager.PermanentlyDeleteActions(MapEditorManager.Actions.First);
                MapEditorManager.Actions.Clear();
                MapEditorManager.Actions.AddFirst(action);
                MapEditorManager.CurrentAction = MapEditorManager.Actions.First;
            }

            // update original scale in case user scales the same object again while still selected
            _currentSelectionOriginalScale = value;
        }
    }
}
