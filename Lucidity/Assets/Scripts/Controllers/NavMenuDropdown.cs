using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NavMenuDropdown : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public bool MouseIsOverDropdownMenu = false;

    public void OnPointerEnter(PointerEventData eventData) {
        // if (gameObject.activeSelf) {
            MouseIsOverDropdownMenu = true;
            Debug.Log("Mouse is over dropdown menu");
        // } else {
        //     Debug.Log("Dropdown menu is not active");
        // }
    }

    public void OnPointerExit(PointerEventData eventData) {
        Debug.Log(eventData.pointerEnter);
        // if (gameObject.activeSelf) {
            MouseIsOverDropdownMenu = false;
            Debug.Log("Mouse is not over dropdown menu");
        // }
    }
}
