using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NavButtonDropdown : MonoBehaviour {
    public GameObject DropdownMenu;
    private Button _navButton;
    private Image _navButtonImage;
    private Color _navButtonColor;
    private Color _navButtonClickedColor = new Color(0.48f, 0.49f, 0.52f);

    private void Start() {
        _navButton = GetComponent<Button>();
        _navButtonImage = GetComponent<Image>();
        _navButtonColor = _navButtonImage.color;
        _navButton.onClick.AddListener(ToggleDropdownMenu);
    }

    private void Update() {
        if (DropdownMenu.activeSelf && Input.GetMouseButtonDown(0)) {
            GameObject selectedObject = EventSystem.current.currentSelectedGameObject;
            
            // if the user clicks outside of the dropdown menu, close it
            if (selectedObject == null || (selectedObject != null && selectedObject != gameObject 
                && selectedObject != DropdownMenu 
                && !(selectedObject.transform.IsChildOf(DropdownMenu.transform)))) {
                DropdownMenu.SetActive(false);
                _navButtonImage.color = _navButtonColor;
            }
        }
    }
    
    /// <summary>
    /// Toggles the visibility of the dropdown menu.
    /// </summary>
    private void ToggleDropdownMenu() {
        DropdownMenu.SetActive(!DropdownMenu.activeSelf);
        if (DropdownMenu.activeSelf) {
            _navButtonImage.color = _navButtonClickedColor;
        } else {
            _navButtonImage.color = _navButtonColor;
        }
    }
}
