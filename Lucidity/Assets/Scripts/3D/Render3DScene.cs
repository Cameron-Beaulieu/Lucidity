using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Render3DScene : MonoBehaviour {

    private static GameObject _map;
    private GameObject _avatar;
    private GameObject _editor;
    // private float _scaleUpFactor = 81f;
    // private float _mapEditorParentScaleDownFactor = 81f;
    [SerializeField] private List<GameObject> _mapTypes;
    [SerializeField] private List<GameObject> _3DPrefabs;

    private void Awake() {
        _avatar = GameObject.Find("Avatar");
        _editor = GameObject.Find("MapEditorManager");
        GameObject.Find("BackButton").GetComponent<Button>().onClick.AddListener(RevertTo2D);

        CreateMap();
        PlaceAssets();
        PlaceAvatar();        
        _editor.SetActive(false);
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
                        // newGameObject = Instantiate(_3DPrefabs[0], 
                        // calculatePlacementPosition(kvp.Value, _3DPrefabs[0]), kvp.Value.Rotation);
                        // newGameObject.transform.localScale = newGameObject.transform.localScale * map3DBaseScale;
                        Place3DObject(_3DPrefabs[0], kvp);
                        break;
                    
                    case "House":
                        // newGameObject = Instantiate(_3DPrefabs[1], 
                        // calculatePlacementPosition(kvp.Value, _3DPrefabs[1]), kvp.Value.Rotation);
                        // newGameObject.transform.localScale = kvp.Value.Scale / mapEditorParentBaseScale * map3DBaseScale;
                        Place3DObject(_3DPrefabs[1], kvp);
                        break;
                    
                    case "Mountain":
                        // newGameObject = Instantiate(_3DPrefabs[2], 
                        // calculatePlacementPosition(kvp.Value, _3DPrefabs[2]), kvp.Value.Rotation);
                        // newGameObject.transform.localScale = new Vector3(newGameObject.transform.localScale.x * kvp.Value.Scale.x, newGameObject.transform.localScale.y * kvp.Value.Scale.y, newGameObject.transform.localScale.z * kvp.Value.Scale.z) / 81f * map3DBaseScale;// newGameObject.transform.localScale * kvp.Value.Scale;// * map3DBaseScale;
                        Place3DObject(_3DPrefabs[2], kvp);
                        break;
                    
                    case "Tree":
                        // newGameObject = Instantiate(_3DPrefabs[3], 
                        // calculatePlacementPosition(kvp.Value, _3DPrefabs[3]), kvp.Value.Rotation);
                        // newGameObject.transform.localScale = new Vector3(newGameObject.transform.localScale.x * kvp.Value.Scale.x, newGameObject.transform.localScale.y * kvp.Value.Scale.y, newGameObject.transform.localScale.z * kvp.Value.Scale.z) / 81f * map3DBaseScale;//newGameObject.transform.localScale * kvp.Value.Scale;// * map3DBaseScale;
                        Place3DObject(_3DPrefabs[3], kvp);
                        break;
                    
                    default:
                        Debug.Log("using default prefab");
                        // newGameObject = Instantiate(_3DPrefabs[0], 
                        // calculatePlacementPosition(kvp.Value, _3DPrefabs[0]), kvp.Value.Rotation);
                        // newGameObject.transform.localScale = kvp.Value.Scale / mapEditorParentBaseScale * map3DBaseScale;
                        Place3DObject(_3DPrefabs[0], kvp);
                        break;
                }
                // newGameObject.transform.localScale = new Vector3(newGameObject.transform.localScale.x * kvp.Value.Scale.x, newGameObject.transform.localScale.y * kvp.Value.Scale.y, newGameObject.transform.localScale.z * kvp.Value.Scale.z);
                // newGameObject.transform.localPosition = new Vector3(newGameObject.transform.localPosition.x, (newGameObject.transform.localScale.y / 2 + _map.transform.position.y), newGameObject.transform.localPosition.z);
                //new Vector3(newGameObject.transform.localScale.x * (kvp.Value.Scale.x / _mapEditorParentScaleDownFactor), newGameObject.transform.localScale.y * (kvp.Value.Scale.y / _mapEditorParentScaleDownFactor), newGameObject.transform.localScale.z * (kvp.Value.Scale.z / _mapEditorParentScaleDownFactor)) * _scaleUpFactor;
            }
        }
    }

    private void Place3DObject(GameObject prefab, KeyValuePair <int,MapObject> kvp) {
        GameObject newGameObject = Instantiate(prefab, new Vector3(0,0,0), kvp.Value.Rotation);
        newGameObject.transform.localScale = new Vector3(newGameObject.transform.localScale.x * kvp.Value.Scale.x, newGameObject.transform.localScale.y * kvp.Value.Scale.y, newGameObject.transform.localScale.z * kvp.Value.Scale.z);
        newGameObject.transform.position = calculatePlacementPosition(kvp.Value, prefab);
        Debug.Log("Placing " + kvp.Value.Name + " at " + newGameObject.transform.position);
    }

    /// <summary>
    /// Places <c>_avatar</c> on the 3D map in accordance with the placement of the spawn point 
    /// on the 2D map.
    /// </summary>
    private void PlaceAvatar() {
        Debug.Log("placing avatar");
        _avatar.transform.position = new Vector3(MapEditorManager.SpawnPoint.x, _avatar.transform.position.y, 
                                                 MapEditorManager.SpawnPoint.y);
    }

    /// <summary>
    /// Calculates the <c>Vector3</c> position where each new asset should be placed
    /// </summary>
    /// <param name="mapObjectData">
    /// The <c>MapObject</c> of the current 2D map object to be placed on the 3D map
    /// </param>
    /// <param name="prefab">
    /// The 3D prefab matching the 2D asset to be placed
    /// </param>
    private Vector3 calculatePlacementPosition(MapObject mapObjectData, GameObject toBePlaced) {
        float xPosition = (mapObjectData.MapPosition.x  + mapObjectData.MapOffset.x);
        float zPosition = (mapObjectData.MapPosition.y  + mapObjectData.MapOffset.y);
        float yPosition = toBePlaced.GetComponent<MeshCollider>().bounds.size.y / 2 + _map.transform.position.y;
        Vector3 placementPosition = new Vector3(xPosition, yPosition, zPosition);
        return placementPosition;
    }

    /// <summary>
    /// Reverts from the 3D scene back to the 2D scene
    /// </summary>
    public void RevertTo2D(){
        _editor.SetActive(true);
        MapEditorManager.ReloadFlag = true;
        SceneManager.LoadScene("MapEditor", LoadSceneMode.Single);
    }
}