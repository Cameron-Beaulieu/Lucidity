using System.Collections;
using System.Collections.Generic;
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
	[SerializeField] private Button _createMapButton;
	[SerializeField] private Button _cancelMapButton;
	[SerializeField] private Toggle _startingAssetsToggle;
	private static SizeType _mapSize;

	private void Start() {
		_createMapButton.onClick.AddListener(CreateMapClickHandler);
		_cancelMapButton.onClick.AddListener(CancelMapClickHandler);
	}

	public static SizeType Size {
		get { return _mapSize; }
		set { _mapSize = value; }
	}

	/// <summary>
	/// Map name accessor.
	/// </summary>
	/// <returns>
	/// <c>string</c> of the map.
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
	/// Biome accessor, providing the desired enumerated <c>BiomeType</c> to constructor.
	/// </summary>
	/// <returns>
	/// <c>Biome</c> with the corresponding <c>BiomeType</c>.
	/// </returns>
	public Biome getBiome() {
		switch(_biomeDropdown.value) {
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
	/// Starting asset toggle accessor.
	/// </summary>
	/// <returns>
	/// <c>bool</c> of starting asset toggle.
	/// </returns>
	public bool getStartingAssetsToggle() { return _startingAssetsToggle.isOn; }

	/// <summary>
	/// Button handler for <c>_createMapButton</c>, selected through in the Unity editor.
	/// </summary>
	public void CreateMapClickHandler() {
		Debug.Log("Map name: " + getMapName());
		Debug.Log("Map size: " + getMapSize());
		Debug.Log("Biome: " + getBiome().Name);
		Debug.Log("Start with assets: " + getStartingAssetsToggle());
		
		_mapSize = getMapSize();
		SceneManager.LoadScene("MapEditor", LoadSceneMode.Single);
	}

	/// <summary>
	/// Button handler for <c>_cancelMapButton</c>, selected through in the Unity editor.
	/// </summary>
	public void CancelMapClickHandler() {
		Debug.Log("Cancel button clicked");
		// TODO: Implement cancel creation user flow
	}
}
