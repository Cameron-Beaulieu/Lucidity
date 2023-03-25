using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    
    private void ToggleDropdownMenu() {
        DropdownMenu.SetActive(!DropdownMenu.activeSelf);
        if (DropdownMenu.activeSelf) {
            _navButtonImage.color = _navButtonClickedColor;
        } else {
            _navButtonImage.color = _navButtonColor;
        }
    }


}
