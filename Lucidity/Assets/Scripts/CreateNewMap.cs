using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreateNewMap : MonoBehaviour {
	public enum SizeType {
		Small,
		Medium,
		Large
	}
	[SerializeField] private InputField _mapName;
	[SerializeField] private Dropdown _mapSizeDropdown;
	[SerializeField] private Dropdown _biomeDropdown;
	[SerializeField] private Toggle _startingAssetsToggle;
	[SerializeField] private Button _cancelMapButton;
	[SerializeField] private Button _createMapButton;
	private static SizeType _mapSize;
	private Text _errorMessage;

	public static SizeType Size {
		get { return _mapSize; }
		set { _mapSize = value; }
	}

	private void Start() {
		_createMapButton.onClick.AddListener(CreateMapClickHandler);
		_cancelMapButton.onClick.AddListener(CancelMapClickHandler);
		_errorMessage = GameObject.Find("ErrorMessage").GetComponent<Text>();
	}

	/// <summary>
	/// Button handler for <c>_cancelMapButton</c>, selected through in the Unity editor.
	/// </summary>
	public void CancelMapClickHandler() {
		SceneManager.LoadScene("StarterScreenUI", LoadSceneMode.Single);
	}

	/// <summary>
	/// Button handler for <c>_createMapButton</c>, selected through in the Unity editor.
	/// </summary>
    public void CreateMapClickHandler() {
        if (String.IsNullOrWhiteSpace(_mapName.text)) {
            _errorMessage.text = "You must provide a file name to create a map";
            _mapName.Select();
            return;
        }
        
        if (CreateFile()) {
            SceneManager.LoadScene("MapEditor", LoadSceneMode.Single);
        }
    }

    /// <summary>
    /// Creates a json file at a location specified by the user. Returns true if the file creation
    /// is successful. Otherwise an error message is displayed on the UI and false is returned.
    /// </summary>
    private bool CreateFile() {
        string directory = EditorUtility.OpenFolderPanel("Select Directory", "", "");
        // cancelled selecting a directory
        if (directory.Equals("")) { return false; }

        string fileName = _mapName.text;
		Size = getMapSize();
        
        fileName = directory + "/" + fileName + ".json";

        if (!File.Exists(fileName)) {
            MapData jsonContent = new MapData(fileName, Size, getBiome());
            File.WriteAllText(fileName, jsonContent.Serialize());
            return true;
        }
        
        _errorMessage.text = "There is already a map with that name in the chosen directory";
        _mapName.Select();
        return false;
    }

	/// <summary>
	/// Biome accessor, providing the desired enumerated <c>BiomeType</c> to constructor.
	/// </summary>
	/// <returns>
	/// <c>Biome</c> with the corresponding <c>BiomeType</c>.
	/// </returns>
	public Biome getBiome() {
		switch (_biomeDropdown.value) {
			case 0:
				return new Biome(Biome.BiomeType.Forest);
			case 1:
				return new Biome(Biome.BiomeType.Desert);
			case 2:
				return new Biome(Biome.BiomeType.Ocean);
			default:
				return new Biome(Biome.BiomeType.Forest);
		}
	}

	/// <summary>
	/// Map name accessor.
	/// </summary>
	/// <returns>
	/// <c>string</c> of the map name.
	/// </returns>
	public string getMapName() { return _mapName.text; }

	/// <summary>
	/// Biome size accessor.
	/// </summary>
	/// <returns>
	/// Enumerated <c>SizeType</c> corresponding to the map size.
	/// </returns>
	public SizeType getMapSize() {
		switch(_mapSizeDropdown.value) {
			case 0:
				return SizeType.Small;
			case 1:
				return SizeType.Medium;
			case 2:
				return SizeType.Large;
			default:
				return SizeType.Medium;
		}
	}

	/// <summary>
	/// Starting asset toggle accessor.
	/// </summary>
	/// <returns>
	/// <c>bool</c> of starting asset toggle.
	/// </returns>
	public bool getStartingAssetsToggle() { return _startingAssetsToggle.isOn; }
}
