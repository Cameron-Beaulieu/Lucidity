using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Render3DScene : MonoBehaviour {

    private static GameObject _map;
    // Arbitrary value used to scale the 2D map to the 3D map
    // Scales size of map + assets + distance between assets
    private float _scaleFactor;

    // Arbitrary value to scale down the map to make up for the assets only being 1 x 1 x 1
    private float _mapScaledownFactor = 1f;
    private GameObject _avatar;
    [SerializeField] private List<GameObject> _mapTypes;
    [SerializeField] private List<GameObject> _3DPrefabs;

    void Awake() {
        _avatar = GameObject.Find("Avatar");
        CreateMap();
        PlaceAssets();
        PlaceAvatar();        
    }

    /// <summary>
	/// Creates the terrain of the 3D scene during 2D to 3D conversion 
	/// </summary>
    private void CreateMap() {
        // Adds a plane with a certain material based on the desired terrain type
        if(CreateNewMap.ChosenBiome != null) {
            switch(CreateNewMap.ChosenBiome.Name) {
                case Biome.BiomeType.Forest:
                    _map = (GameObject) Instantiate(_mapTypes[0], new Vector3 (0f, 0f, 0f), Quaternion.identity);
                    break;
            
                case Biome.BiomeType.Desert:
                    _map = (GameObject) Instantiate(_mapTypes[1], new Vector3 (0f, 0f, 0f), Quaternion.identity);
                    break;
            
                case Biome.BiomeType.Ocean:
                    _map = (GameObject) Instantiate(_mapTypes[2], new Vector3 (0f, 0f, 0f), Quaternion.identity);
                    break;
            
                default:
                    Debug.Log("Using Default");
                    _map = (GameObject) Instantiate(_mapTypes[0], new Vector3 (0f, 0f, 0f), Quaternion.identity);
                    break;
            }
        }
        else {
            _map = (GameObject) Instantiate(_mapTypes[0], new Vector3 (0f, 0f, 0f), Quaternion.identity);
        }

        Vector3 mapScale = _map.transform.localScale;
        CreateNewMap.SizeType mapSize = CreateNewMap.Size;
        float mapWidth;
        float mapHeight;
        float xScale;
        float zScale;

        // The scale of the map is based on the map size
        // Everything is scaled by a _scaleFactor to create a larger navigatable terrain
        // Map height and width are taken from the 2D map size, scaleFactor is arbitrary
		switch (mapSize) {
		  case CreateNewMap.SizeType.Small:
			mapWidth = 1400f;
            mapHeight = 810f;
            _scaleFactor = 100f;
			break;
		  case CreateNewMap.SizeType.Medium:
		  	mapWidth = 2100f;
            mapHeight = 1215f;
            _scaleFactor = 150f;
			break;
		  case CreateNewMap.SizeType.Large:
		  	mapWidth = 2800f;
            mapHeight = 1620f;
            _scaleFactor = 200f;
			break;
		  default:
		  	mapWidth = 2100f;
            mapHeight = 1215f;
            _scaleFactor = 150f;
			break;
		}

        _scaleFactor = 1f;

        // Adjusting map size and dividing by 10 to properly scale it
        xScale = mapWidth * _scaleFactor / _mapScaledownFactor;
        zScale = mapHeight * _scaleFactor / _mapScaledownFactor;
		_map.transform.localScale = new Vector3 (xScale, 1f, zScale);
    }

    /// <summary>
	/// Places each asset from the 2D map on the 3D map
	/// </summary>
    private void PlaceAssets() {
        GameObject newGameObject; 
        foreach (KeyValuePair <int, MapObject> kvp in MapEditorManager.MapObjects) {
            if(kvp.Value.IsActive) {
                switch (kvp.Value.Name) {
                    case "Fortress":
                        newGameObject = Instantiate(_3DPrefabs[0], 
                        calculatePlacementHeight(kvp.Value, _3DPrefabs[0]), kvp.Value.Rotation);
                        newGameObject.transform.localScale = kvp.Value.Scale * _scaleFactor;
                        break;
                    
                    case "House":
                        newGameObject = Instantiate(_3DPrefabs[1], 
                        calculatePlacementHeight(kvp.Value, _3DPrefabs[1]), kvp.Value.Rotation);
                        newGameObject.transform.localScale = kvp.Value.Scale * _scaleFactor;
                        break;
                    
                    case "Mountain":
                        newGameObject = Instantiate(_3DPrefabs[2], 
                        calculatePlacementHeight(kvp.Value, _3DPrefabs[2]), kvp.Value.Rotation);
                        newGameObject.transform.localScale = kvp.Value.Scale * _scaleFactor;
                        break;
                    
                    case "Tree":
                        newGameObject = Instantiate(_3DPrefabs[3], 
                        calculatePlacementHeight(kvp.Value, _3DPrefabs[3]), kvp.Value.Rotation);
                        newGameObject.transform.localScale = kvp.Value.Scale * _scaleFactor;
                        break;
                    
                    default:
                        Debug.Log("using default prefab");
                        newGameObject = Instantiate(_3DPrefabs[0], 
                        calculatePlacementHeight(kvp.Value, _3DPrefabs[0]), kvp.Value.Rotation);
                        newGameObject.transform.localScale = kvp.Value.Scale * _scaleFactor;
                        break;
                }
            }
        }
    }

    private void PlaceAvatar() {
        _avatar.transform.position = new Vector3(MapEditorManager.SpawnPoint.x * _scaleFactor, 1f, MapEditorManager.SpawnPoint.y * _scaleFactor);
    }

    /// <summary>
	/// Calculates the Vector3 position where each new asset should be placed
	/// </summary>
	/// <param name="toBePlaced">
	/// The MapObject of the current 2D map object to be placed on the 3D map
	/// </param>
	/// <param name="prefab">
	/// The 3D prefab matching the 2D asset to be placed
	/// </param>
    private Vector3 calculatePlacementHeight(MapObject toBePlaced, GameObject prefab) {
        float xPosition = (toBePlaced.MapPosition.x  + toBePlaced.MapOffset.x) * _scaleFactor ;
        float zPosition = (toBePlaced.MapPosition.y  + toBePlaced.MapOffset.y) * _scaleFactor ;
        float yPosition = (prefab.transform.localScale.y * _scaleFactor / 2) + _map.transform.position.y;
        Vector3 placementPosition = new Vector3(xPosition, yPosition, zPosition);
        return placementPosition;
    }
}