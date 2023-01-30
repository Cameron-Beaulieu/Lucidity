using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssetController : MonoBehaviour {
    public int Id;
    public bool Clicked = false;
    private MapEditorManager _editor;
    private static GameObject _prevParentContainer;

    void Start() {
        _editor = GameObject.FindGameObjectWithTag("MapEditorManager")
            .GetComponent<MapEditorManager>();
    }

    public void ButtonClicked() {
        Vector2 worldPosition = MapEditorManager.getMousePosition();
        Clicked = true;

        GameObject activeImage = GameObject.FindGameObjectWithTag("AssetImage");

        // if there is an Image being shown on hover already, destroy it
        if (activeImage != null) {
            Destroy(activeImage);
        }

        // Creates image that will follow mouse
        Instantiate(_editor.AssetImage[Id], 
                    new Vector3(worldPosition.x, worldPosition.y, 0), 
                    Quaternion.identity);

        _editor.CurrentButtonPressed = Id;

        GameObject parentContainer = GameObject.Find(
            _editor.AssetPrefabs[Id].transform.parent.name);

        // Un-highlight previously selected asset in "Sprites" pane
        if (_prevParentContainer != null) {
            _prevParentContainer.GetComponent<Image>().color = new Color32(66, 71, 80, 100);
        }

        // Highlight asset in "Sprites" pane
        parentContainer.GetComponent<Image>().color = new Color32(0, 0, 0, 100);
        _prevParentContainer = parentContainer;

        // set painting status
        _editor.ChangeTools("Brush Tool");
    }

    /// <summary>
    /// Unselects the associated asset button.
    /// </summary>
    public void UnselectButton() { 
        Clicked = false;
        if (_prevParentContainer != null) {
            _prevParentContainer.GetComponent<Image>().color = new Color32(66, 71, 80, 100);
        }
    }
}
