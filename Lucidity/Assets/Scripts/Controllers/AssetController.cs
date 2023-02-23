using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssetController : MonoBehaviour {
    public int Id;
    public bool Clicked;
    private MapEditorManager _editor;
    private Button _assetButton;
    private static GameObject _prevParentContainer;

    private void Start() {
        _editor = GameObject.FindGameObjectWithTag("MapEditorManager")
            .GetComponent<MapEditorManager>();
        _assetButton = gameObject.GetComponent<Button>();
        _assetButton.onClick.AddListener(SelectAssetClickHandler);
        Clicked = false;
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

        MapEditorManager.CurrentButtonPressed = Id;

        GameObject parentContainer = GameObject.Find(
            gameObject.transform.parent.name);
        // Un-highlight previously selected asset in "Sprites" pane
        if (_prevParentContainer != null) {
            _prevParentContainer.GetComponent<Image>().color = new Color32(66, 71, 80, 100);
        }
        // Highlight asset in "Sprites" pane
        parentContainer.GetComponent<Image>().color = new Color32(0, 0, 0, 100);
        _prevParentContainer = parentContainer;
        // set painting status
        Tool.ChangeTools("Brush Tool");
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
