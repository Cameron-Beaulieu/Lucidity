using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Render3DScene : MonoBehaviour {

    private static GameObject _map;
    private float scaleFactor;
    [SerializeField] private List<GameObject> MapTypes;
    [SerializeField] private List<GameObject> _3DPrefabs;

    void Awake(){
        Debug.Log("Starting Awake");
        CreateMap();
        PlaceAssets();        
    }

    /// <summary>
	/// Creates the terrain of the 3D scene during 2D to 3D conversion 
	/// </summary>
    private void CreateMap(){
        // Adds a 1 x 1 x 1 cude with a certain material based on the desired terrain type
        switch(CreateNewMap.ChosenBiome.Name){
            case Biome.BiomeType.Forest:
                _map = (GameObject) Instantiate(MapTypes[0], new Vector3 (0f, 0f, 0f), new Quaternion (0f, 0f, 0f, 0f));
                break;
            
            case Biome.BiomeType.Desert:
                _map = (GameObject) Instantiate(MapTypes[1], new Vector3 (0f, 0f, 0f), new Quaternion (0f, 0f, 0f, 0f));
                break;
            
            case Biome.BiomeType.Ocean:
                _map = (GameObject) Instantiate(MapTypes[2], new Vector3 (0f, 0f, 0f), new Quaternion (0f, 0f, 0f, 0f));
                break;
            
            default:
                Debug.Log("Using Default");
                _map = (GameObject) Instantiate(MapTypes[3], new Vector3 (0f, 0f, 0f), new Quaternion (0f, 0f, 0f, 0f));
                break;
        }

        Vector3 mapScale = _map.transform.localScale;
        CreateNewMap.SizeType mapSize = CreateNewMap.Size;
        float mapWidth;
        float mapHeight;
        float xScale;
        float zScale;

        // The scale of the map is based on the map size
        // Everything is scaled by a scaleFactor to create a larger navigatable terrain
		switch (mapSize) {
		  case CreateNewMap.SizeType.Small:
			mapWidth = 1400f;
            mapHeight = 810f;
            scaleFactor = 100f;
			break;
		  case CreateNewMap.SizeType.Medium:
		  	mapWidth = 2100f;
            mapHeight = 1215f;
            scaleFactor = 150f;
			break;
		  case CreateNewMap.SizeType.Large:
		  	mapWidth = 2800f;
            mapHeight = 1620f;
            scaleFactor = 200f;
			break;
		  default:
		  	mapWidth = 2100f;
            mapHeight = 1215f;
            scaleFactor = 150f;
			break;
		}

        // Adjusting map size and dividing by 100 to account for starting width & height of asset
        xScale = mapWidth * scaleFactor / 10;
        zScale = mapHeight * scaleFactor /10;
		_map.transform.localScale = new Vector3 (xScale, 1f, zScale);
    }

    /// <summary>
	/// Places each asset from the 2D map on the 3D map
	/// </summary>
    private void PlaceAssets(){
        GameObject newGameObject; 
        Debug.Log("Starting Placement");
        foreach (KeyValuePair <int, MapObject> kvp in MapEditorManager.MapObjects) {
            Debug.Log(kvp.Value.Name);
            switch (kvp.Value.Name){
                case "Fortress":
                    newGameObject = Instantiate(_3DPrefabs[0], 
                    calculatePlacementHeight(kvp.Value, _3DPrefabs[0]), kvp.Value.Rotation);
                    newGameObject.transform.localScale = kvp.Value.Scale * scaleFactor;
                    break;
                
                case "House":
                    newGameObject = Instantiate(_3DPrefabs[1], 
                    calculatePlacementHeight(kvp.Value, _3DPrefabs[1]), kvp.Value.Rotation);
                    newGameObject.transform.localScale = kvp.Value.Scale * scaleFactor;
                    break;
                
                case "Mountain":
                    newGameObject = Instantiate(_3DPrefabs[2], 
                    calculatePlacementHeight(kvp.Value, _3DPrefabs[2]), kvp.Value.Rotation);
                    newGameObject.transform.localScale = kvp.Value.Scale * scaleFactor;
                    break;
                
                case "Tree":
                    newGameObject = Instantiate(_3DPrefabs[3], 
                    calculatePlacementHeight(kvp.Value, _3DPrefabs[3]), kvp.Value.Rotation);
                    newGameObject.transform.localScale = kvp.Value.Scale * scaleFactor;
                    break;
                
                default:
                    Debug.Log("using default prefab");
                    newGameObject = Instantiate(_3DPrefabs[0], 
                    calculatePlacementHeight(kvp.Value, _3DPrefabs[0]), kvp.Value.Rotation);
                    newGameObject.transform.localScale = kvp.Value.Scale * scaleFactor;
                    break;
            }
        }
        Debug.Log("Ending Placement");
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
    private Vector3 calculatePlacementHeight(MapObject toBePlaced, GameObject prefab){
        float xPosition = (toBePlaced.MapPosition.x  + toBePlaced.MapOffset.x) * scaleFactor ;
        float zPosition = (toBePlaced.MapPosition.y  + toBePlaced.MapOffset.y) * scaleFactor ;
        float yPosition = (prefab.transform.localScale.y * scaleFactor / 2) + _map.transform.position.y;
        Vector3 placementPosition = new Vector3(xPosition, yPosition, zPosition);
        return placementPosition;
    }
}