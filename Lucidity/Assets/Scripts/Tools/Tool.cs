using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tool : MonoBehaviour {
	private MapEditorManager _editor;
	private string _name;
	private Color _unselected = new Color(48/255f, 49/255f, 52/255f);
	private static GameObject _paintingMenu;
	private static GameObject _selectionMenu;
	private static GameObject _selectionOptions;
	public static List<string> ToolKeys = new List<string>();
	public static Dictionary<string, bool> ToolStatus = new Dictionary<string, bool>();

	public static GameObject PaintingMenu {
		get { return _paintingMenu; }
		set { _paintingMenu = value; }
	}

	public static GameObject SelectionMenu {
		get { return _selectionMenu; }
		set { _selectionMenu = value; }
	}

	public static GameObject SelectionOptions {
		get { return _selectionOptions; }
		set { _selectionOptions = value; }
	}

	void Start() {
		_editor = GameObject.FindGameObjectWithTag("MapEditorManager")
			.GetComponent<MapEditorManager>();
		_name = gameObject.name;
		gameObject.GetComponent<Button>().onClick.AddListener(ToolButtonClickHandler);
	}

	void Update() {
		if (ToolStatus.ContainsKey(_name)
				&& ToolStatus[_name]
				&& gameObject.GetComponent<Image>().color != Color.black) {
			gameObject.GetComponent<Image>().color = Color.black;
		} else if (ToolStatus.ContainsKey(_name)
				&& !ToolStatus[_name]
				&& gameObject.GetComponent<Image>().color != _unselected) {
			gameObject.GetComponent<Image>().color = _unselected;
		}
	}

	/// <summary>
	/// Change the tool being used by the map editor to appropriately reflect the selected tool.
	/// </summary>
	public void ToolButtonClickHandler() {
		if (_name != "Brush Tool" && ToolStatus["Brush Tool"]) {
			StopPainting();
		}
		ChangeTools(_name);
	}

	/// <summary>
	/// Changes the tool currently in use to the selected tool.
	/// </summary>
	/// <param name="toolSelected">
	/// <c>string</c> corresponding to the newly selected tool.
	/// </param>
	public static void ChangeTools(string toolSelected) {
		switch (toolSelected) {
			case "Brush Tool":
				_paintingMenu.SetActive(true);
				_selectionMenu.SetActive(false);
				break;
			default:    // Default case is having the selection menu open
				_paintingMenu.SetActive(false);
				_selectionMenu.SetActive(true);
				if (SelectMapObject.SelectedObject == null) {
					SelectionOptions.SetActive(false);
				}
				break;
		}
		foreach (string toolKey in ToolKeys) {
			if (toolKey != toolSelected) {
				ToolStatus[toolKey] = false;
			} else {
				ToolStatus[toolKey] = true;
			}
		}
	}

	/// <summary>
	/// Removes the asset following the cursor (if any) and deselects the brush tool button and
	/// selected sprite/terrain.
	/// </summary>
	public void StopPainting() {
		ToolStatus["Brush Tool"] = false;
		Destroy(GameObject.FindGameObjectWithTag("AssetImage"));
		GameObject[] paintButtons = GameObject.FindGameObjectsWithTag("PaintButton");
		foreach (GameObject button in paintButtons) {
			if (button.GetComponent<AssetController>().Clicked) {
				button.GetComponent<AssetController>().UnselectButton();
			}
		}
	}
}
