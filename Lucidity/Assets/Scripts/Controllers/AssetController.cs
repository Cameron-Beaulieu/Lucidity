using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssetController : MonoBehaviour {
    public int Id;
    public bool Clicked;
    private static MapEditorManager _editor;
    private Button _assetButton;
    private static GameObject _prevParentContainer;

    private void Awake() {
        Clicked = false;
        _editor = GameObject.FindGameObjectWithTag("MapEditorManager")
            .GetComponent<MapEditorManager>();
    }

    private void Start() {
        _assetButton = gameObject.GetComponent<Button>();
        _assetButton.onClick.AddListener(SelectAssetClickHandler);
    }

    /// <summary>
    /// Button handler for <c>_assetButton</c>.
    /// </summary>
    public void SelectAssetClickHandler() {
        Clicked = true;
        MapEditorManager.CurrentButtonPressed = Id;
        GameObject activeImage = GameObject.FindGameObjectWithTag("AssetImage");
        // if there is an Image being shown on hover already, destroy it
        if (activeImage != null) {
            Destroy(activeImage);
        }
        DynamicBoundingBox.CreateDynamicAssetImage(_editor.AssetImage[Id],
                                                   Mouse.GetMousePosition());

        GameObject parentContainer = GameObject.Find(gameObject.transform.parent.name);
        // Unselect previously selected asset in "Sprites" panel unless it 
        // is the same asset as that that is being selected
        if (_prevParentContainer != null && _prevParentContainer != parentContainer) {
            _prevParentContainer.GetComponentInChildren<AssetController>().UnselectButton();
        }
        
        // Highlight asset in "Sprites" pane
        parentContainer.GetComponent<Image>().color = new Color32(0, 0, 0, 100);
        _prevParentContainer = parentContainer;
    }

    /// <summary>
    /// Unselects the selected button.
    /// </summary>
    public void UnselectButton() { 
        Clicked = false;
        if (_prevParentContainer != null) {
            _prevParentContainer.GetComponent<Image>().color = new Color32(66, 71, 80, 100);
        }
    }

    private void OnDisable () {
        if (Clicked && !Tool.ToolStatus["Brush Tool"]) {
            UnselectButton();
        }
    }
}
