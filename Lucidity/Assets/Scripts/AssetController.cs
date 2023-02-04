using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssetController : MonoBehaviour {
	public bool Clicked;
	public int Id;
	private static GameObject _prevParentContainer;
	private Button _assetButton;
	private MapEditorManager _editor;

	void Start() {
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
		Vector2 worldPosition = Mouse.getMousePosition();
		Clicked = true;
		MapEditorManager.CurrentButtonPressed = Id;
		GameObject activeImage = GameObject.FindGameObjectWithTag("AssetImage");
		// if there is an Image being shown on hover already, destroy it
		if (activeImage != null) {
			Destroy(activeImage);
		}
		// Creates image that will follow mouse
		Instantiate(
			_editor.AssetImage[Id], 
			new Vector3(worldPosition.x, worldPosition.y, 90), 
			Quaternion.identity
		);
		GameObject parentContainer = GameObject
			.Find(_editor.AssetPrefabs[Id].transform.parent.name);
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
}
