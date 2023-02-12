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
 
    private float SliderValue {
        get {
            return gameObject.GetComponent<Slider>().value;
        }
    }
 
    public void OnPointerUp (PointerEventData data)
    {
        if (EndDrag != null) 
        {
            EndDrag(SliderValue);
        }
        Debug.Log("Sliding finished!");
        Debug.Log(gameObject.GetComponent<Slider>().value);
    }
}