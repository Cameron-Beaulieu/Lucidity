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
	[SerializeField] private Slider _paintObjectScaleSlider;

	private void Start() {
		_editor = GameObject.FindGameObjectWithTag("MapEditorManager")
			.GetComponent<MapEditorManager>();
		_assetButton = gameObject.GetComponent<Button>();
		_assetButton.onClick.AddListener(SelectAssetClickHandler);
		Clicked = false;
		_paintObjectScaleSlider = GameObject.Find("Slider").GetComponent<Slider>();
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
		// Creates image that will follow mouse
		// GameObject hoverImage = Instantiate(_editor.AssetImage[Id],
		// 			new Vector3(worldPosition.x, worldPosition.y, 90),
		// 			Quaternion.identity);
		// hoverImage.transform.localScale = new Vector3(hoverImage.transform.localScale.x + Zoom.zoomFactor, 
		//             hoverImage.transform.localScale.y + Zoom.zoomFactor, 
		//             hoverImage.transform.localScale.z + Zoom.zoomFactor);
		CreateFollowingImage(_editor.AssetImage[Id], _paintObjectScaleSlider);

		MapEditorManager.CurrentButtonPressed = Id;

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
		Tool.ChangeTools("Brush Tool");
	}

	public static void CreateFollowingImage(GameObject prefab, Slider slider) {
		Vector2 worldPosition = Mouse.getMousePosition();
		GameObject hoverImage = Instantiate(prefab,
					new Vector3(worldPosition.x, worldPosition.y, 90),
					Quaternion.identity);
		hoverImage.transform.localScale = new Vector3((hoverImage.transform.localScale.x + Zoom.zoomFactor) * slider.value,
					(hoverImage.transform.localScale.y + Zoom.zoomFactor) * slider.value,
					(hoverImage.transform.localScale.z + Zoom.zoomFactor) * slider.value);
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

	void OnDisable()
	{
		if (Clicked && !Tool.ToolStatus["Brush Tool"])
		{
			UnselectButton();
		}
	}
}