using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NavButtonDropdown : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public GameObject DropdownMenu;
    private NavMenuDropdown _dropdownScript;

    private void Start() {
        _dropdownScript = DropdownMenu.GetComponent<NavMenuDropdown>();
    }
    
    public void OnPointerEnter(PointerEventData eventData) {
        DropdownMenu.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (!_dropdownScript.MouseIsOverDropdownMenu) {
            DropdownMenu.SetActive(false);
        }
    }
}
