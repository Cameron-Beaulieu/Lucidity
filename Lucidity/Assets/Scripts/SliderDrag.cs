using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
 
public delegate void EndSliderDragEventHandler (float val);
 
[RequireComponent(typeof (Slider))]
public class SliderDrag : MonoBehaviour, IPointerUpHandler 
{
    public event EndSliderDragEventHandler EndDrag;
    private MapEditorManager _editor;

    private void Start() {
        _editor = GameObject.FindGameObjectWithTag("MapEditorManager").GetComponent<MapEditorManager>();
    }
    
    private float SliderValue {
        get {
            return gameObject.GetComponent<Slider>().value;
        }
    }

    public void OnPointerUp (PointerEventData data)
    {
        if (EndDrag != null) {
            EndDrag(SliderValue);
        }
        Vector3 oldSize = SelectMapObject.SelectedObject.transform.localScale;
        Vector3 newSize = new Vector3(SliderValue, SliderValue, SliderValue);
        SelectMapObject.SelectedObject.transform.localScale = newSize;

        //Check for collision with other objects

        // for UNDO/REDO
        List<GameObject> objectsToScale = new List<GameObject>() { SelectMapObject.SelectedObject };
        _editor.Actions.AddAfter(_editor.CurrentAction, new ResizeMapObjectAction(objectsToScale, oldSize, newSize));
        _editor.CurrentAction = _editor.CurrentAction.Next;
    }
}