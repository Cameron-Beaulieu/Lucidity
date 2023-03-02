using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Render3DScene : MonoBehaviour {

    private static GameObject _map;
    private GameObject _avatar;
    [SerializeField] private List<GameObject> _mapTypes;
    [SerializeField] private List<GameObject> _3DPrefabs;

    private void Awake() {
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
        if (CreateNewMap.ChosenBiome != null) {
            switch(CreateNewMap.ChosenBiome.Name) {
                case Biome.BiomeType.Forest:
                    _map = (GameObject) Instantiate(_mapTypes[0], new Vector3(0f, 0f, 0f), 
                                                    Quaternion.identity);
                    break;
            
                case Biome.BiomeType.Desert:
                    _map = (GameObject) Instantiate(_mapTypes[1], new Vector3(0f, 0f, 0f), 
                                                    Quaternion.identity);
                    break;
            
                case Biome.BiomeType.Ocean:
                    _map = (GameObject) Instantiate(_mapTypes[2], new Vector3(0f, 0f, 0f), 
                                                    Quaternion.identity);
                    break;
            
                default:
                    Debug.Log("Using Default");
                    _map = (GameObject) Instantiate(_mapTypes[0], new Vector3(0f, 0f, 0f), 
                                                    Quaternion.identity);
                    break;
            }
        } else {
            _map = (GameObject) Instantiate(_mapTypes[0], new Vector3 (0f, 0f, 0f), 
                                            Quaternion.identity);
        }

        _map.transform.localScale = new Vector3 (100000, 1f, 100000);
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
                        calculatePlacementPosition(kvp.Value, _3DPrefabs[0]), kvp.Value.Rotation);
                        newGameObject.transform.localScale = kvp.Value.Scale;
                        break;
                    
                    case "House":
                        newGameObject = Instantiate(_3DPrefabs[1], 
                        calculatePlacementPosition(kvp.Value, _3DPrefabs[1]), kvp.Value.Rotation);
                        newGameObject.transform.localScale = kvp.Value.Scale;
                        break;
                    
                    case "Mountain":
                        newGameObject = Instantiate(_3DPrefabs[2], 
                        calculatePlacementPosition(kvp.Value, _3DPrefabs[2]), kvp.Value.Rotation);
                        newGameObject.transform.localScale = kvp.Value.Scale;
                        break;
                    
                    case "Tree":
                        newGameObject = Instantiate(_3DPrefabs[3], 
                        calculatePlacementPosition(kvp.Value, _3DPrefabs[3]), kvp.Value.Rotation);
                        newGameObject.transform.localScale = kvp.Value.Scale;
                        break;
                    
                    default:
                        Debug.Log("using default prefab");
                        newGameObject = Instantiate(_3DPrefabs[0], 
                        calculatePlacementPosition(kvp.Value, _3DPrefabs[0]), kvp.Value.Rotation);
                        newGameObject.transform.localScale = kvp.Value.Scale;
                        break;
                }
            }
        }
    }

    /// <summary>
    /// Places <c>_avatar</c> on the 3D map in accordance with the placement of the spawn point 
    /// on the 2D map.
    /// </summary>
    private void PlaceAvatar() {
        _avatar.transform.position = new Vector3(MapEditorManager.SpawnPoint.x, 1f, 
                                                 MapEditorManager.SpawnPoint.y);
    }

    /// <summary>
    /// Calculates the <c>Vector3</c> position where each new asset should be placed
    /// </summary>
    /// <param name="toBePlaced">
    /// The <c>MapObject</c> of the current 2D map object to be placed on the 3D map
    /// </param>
    /// <param name="prefab">
    /// The 3D prefab matching the 2D asset to be placed
    /// </param>
    private Vector3 calculatePlacementPosition(MapObject toBePlaced, GameObject prefab) {
        float xPosition = (toBePlaced.MapPosition.x  + toBePlaced.MapOffset.x);
        float zPosition = (toBePlaced.MapPosition.y  + toBePlaced.MapOffset.y);
        float yPosition = (toBePlaced.Scale.y / 2) + _map.transform.position.y;
        Vector3 placementPosition = new Vector3(xPosition, yPosition, zPosition);
        return placementPosition;
    }
}