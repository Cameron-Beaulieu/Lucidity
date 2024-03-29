using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Render3DScene : MonoBehaviour {

    public static bool EscapeTestingInput;
    private static GameObject _map;
    private GameObject _avatar;
    private GameObject _editor;
    private GameObject _optionsPanel;
    private GameObject _controlPanel;
    private GameObject _messagePanel;
    private MoveCamera _camera;
    private AvatarMovement _movement;
    private Dictionary<int, GameObject> _sceneObjects = new Dictionary<int, GameObject>();
    [SerializeField] private List<GameObject> _mapTypes;
    [SerializeField] private List<GameObject> _3DPrefabs;

    private void Awake() {
        _avatar = GameObject.Find("Avatar");
        _editor = GameObject.Find("MapEditorManager");
        _optionsPanel = GameObject.Find("OptionsPanel");
        _controlPanel = GameObject.Find("ControlPanel");
        _messagePanel = GameObject.Find("MessagePanel");
        _camera = GameObject.Find("Camera Holder").GetComponent<MoveCamera>();
        _movement = GameObject.Find("Avatar").GetComponent<AvatarMovement>();
        GameObject.Find("BackButton").GetComponent<Button>().onClick.AddListener(RevertTo2D);
        GameObject.Find("ExitOptionsButton").GetComponent<Button>().onClick
            .AddListener(SwitchFocus);

        // set the map name by dropping file path using regex
        if (MapData.FileName != null) {
            string pattern = @"(?<=\\|\/)[^\\\/]*(?=\.\w+$)";
            Match m = Regex.Match(MapData.FileName, pattern, RegexOptions.IgnoreCase);
            if (m.Success) {
                string name = m.Value;
                GameObject.Find("OptionsPanel/BackText").GetComponent<TMP_Text>().text =
                    "Currently Navigating: " + name;
            }
        }

        CreateMap();
        PlaceAssets();
        PlaceAvatar();
        FixObjectHeights();        
        _editor.SetActive(false);
        _optionsPanel.SetActive(false);
        _controlPanel.SetActive(false);
        _messagePanel.SetActive(true);
    }

    private void Update() {        
        if (Input.GetKeyDown("escape") || EscapeTestingInput) {
            SwitchFocus();
        }
    }

    /// <summary>
    /// Switches focus between in the 3D navigation mode, and with the 3D options menu.
    /// </summary>
    private void SwitchFocus() {
        _optionsPanel.SetActive(!_optionsPanel.activeSelf);
        _controlPanel.SetActive(!_controlPanel.activeSelf);
        _messagePanel.SetActive(!_messagePanel.activeSelf);
        _camera.enabled = _messagePanel.activeSelf;
        _movement.enabled = _messagePanel.activeSelf;
        _movement.NoclipToggleHandler();
        if (_optionsPanel.activeSelf && _controlPanel.activeSelf) {
            _movement.SetGravity(false);
        }
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
            
                default:
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
        foreach (Dictionary<int, MapObject> layer in MapEditorManager.Layers) {
            foreach (KeyValuePair <int, MapObject> kvp in layer) {
                if(kvp.Value.IsActive) {
                    switch (kvp.Value.Name) {
                        case "Fortress":
                            Place3DObject(_3DPrefabs[0], kvp);
                            break;
                        
                        case "House":
                            Place3DObject(_3DPrefabs[1], kvp);
                            break;
                        
                        case "Mountain":
                            Place3DObject(_3DPrefabs[2], kvp);
                            break;
                        
                        case "Tree":
                            Place3DObject(_3DPrefabs[3], kvp);
                            break;
                        
                        default:
                            Place3DObject(_3DPrefabs[0], kvp);
                            break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Places a 3D asset on the 3D map
    /// </summary>
    /// <param name="prefab">
    /// The 3D prefab of the <c>MapObject</c> to be placed on the 3D map
    /// </param>
    /// <param name="kvp">
    /// The data of the <c>MapObject</c> to be placed on the 3D map
    /// </param>
    private void Place3DObject(GameObject prefab, KeyValuePair <int,MapObject> kvp) {
        GameObject newGameObject = Instantiate(prefab, new Vector3(0,0,0), Quaternion.identity);
        newGameObject.transform.rotation = Quaternion.Euler(0, -kvp.Value.Rotation.eulerAngles.z, 
                                                            0);
        newGameObject.transform.localScale = new Vector3(
            newGameObject.transform.localScale.x * kvp.Value.Scale.x, 
            newGameObject.transform.localScale.y * kvp.Value.Scale.y, 
            newGameObject.transform.localScale.z * kvp.Value.Scale.z);
        newGameObject.transform.position = calculatePlacementPosition(kvp.Value, newGameObject);

        // if an asset is not a mountain and below ground, calculate the distance needed to move it
        // up to the surface of the terrain; this is done by calculating the distance between the
        // bottom-most point of the asset and the surface of the terrain (the lowest bounds of the
        // mesh collider multipled by the negative of the scale of the object since the lowest
        // bounds will be negative due to being below ground)
        // this ignores mountain assets due to the way the asset looks at the bottom
        if (IsBelowGround(newGameObject) && kvp.Value.Name != "Mountain") {
            newGameObject.transform.position = new Vector3(newGameObject.transform.position.x, 
                newGameObject.GetComponent<MeshCollider>().bounds.min.y * -kvp.Value.Scale.y, 
                newGameObject.transform.position.z);
        }
        _sceneObjects[kvp.Value.Id] = newGameObject;
    }

    /// <summary>
    /// Checks if the placed 3D asset is below the ground
    /// </summary>
    /// <param name="gameObjectToCheck">
    /// The asset placed on the 3D map
    /// </param>
    /// <returns>
    /// <c>true</c> if the asset is below the ground, <c>false</c> otherwise
    /// </returns>
    private bool IsBelowGround(GameObject gameObjectToCheck) {
        GameObject ground = GameObject.Find("ForestPlane(Clone)");
        MeshCollider gameObjectCollider = gameObjectToCheck.GetComponent<MeshCollider>();
        MeshCollider groundCollider = ground.GetComponent<MeshCollider>();
        return gameObjectCollider.bounds.Intersects(groundCollider.bounds);
    }

    /// <summary>
    /// Places <c>_avatar</c> on the 3D map in accordance with the placement of the spawn point 
    /// on the 2D map.
    /// </summary>
    private void PlaceAvatar() {
        _avatar.transform.position = new Vector3(MapEditorManager.SpawnPoint.x, 
                                                 _avatar.transform.position.y, 
                                                 MapEditorManager.SpawnPoint.y);
    }

    /// <summary>
    /// Calculates the <c>Vector3</c> position of the asset based on its <c>MapObject</c> data
    /// </summary>
    /// <param name="mapObjectData">
    /// The data of the current 2D <c>MapObject</c> to be placed on the 3D map
    /// </param>
    /// <param name="toBePlaced">
    /// The 3D <c>GameObject</c> on the map that is being placed
    /// </param>
    /// <returns>
    /// The <c>Vector3</c> position where the new asset should be placed
    /// </returns>
    private Vector3 calculatePlacementPosition(MapObject mapObjectData, GameObject toBePlaced) {
        float xPosition = (mapObjectData.MapPosition.x  + mapObjectData.MapOffset.x);
        float zPosition = (mapObjectData.MapPosition.y  + mapObjectData.MapOffset.y);
        Vector3 placementPosition = new Vector3(xPosition, 0, zPosition);
        return placementPosition;
    }

    /// <summary>
    /// After all objects have been placed, this method will fix the heights of objects
    /// that were stacked on top of each other on separate layers.
    /// Currently this only applies to trees stacked on mountains.
    /// </summary>
    private void FixObjectHeights() {
        foreach (List<MapObject> mapObjects in AssetCollision.LayerCollisions) {
            // sometimes mapObjects are in the list that shouldn't be
            if (!_sceneObjects.ContainsKey(mapObjects[0].Id) || 
                !_sceneObjects.ContainsKey(mapObjects[1].Id)) {
                return;
            }
            // mountain will always be at index 0, tree will always be index 1
            // the List is created in AssetCollisions.GetAssetCollisions
            GameObject mountain = _sceneObjects[mapObjects[0].Id];  
            GameObject tree = _sceneObjects[mapObjects[1].Id];
            Vector3 treePos = tree.transform.position;
            MeshCollider mountainCollider = mountain.GetComponent<MeshCollider>();
            float mountainHeight = mountainCollider.bounds.size.y * 
                mountain.transform.localScale.y;
            float treeHeight = tree.GetComponent<MeshCollider>().bounds.size.y * 
                tree.transform.localScale.y;
            // both colliders need to be convex for raycasting
            mountainCollider.convex = true;
            tree.GetComponent<MeshCollider>().convex = true;    
            // move tree above mountain
            tree.transform.position = new Vector3(treePos.x, mountainHeight, treePos.z);
            // update the tree position
            treePos = tree.transform.position;
            // Ray pointing down from tree                       
            Ray ray = new Ray(treePos, new Vector3(0, -1f, 0));
            RaycastHit hit;
            // if hit: move the tree down to that point (and a little more)
            // else: move the tree to the ground
            if (mountainCollider.Raycast(ray, out hit, mountainHeight)) {
                tree.transform.position = new Vector3(
                    treePos.x, mountainHeight - hit.distance - 0.25f * treeHeight, treePos.z);
            } else {
                tree.transform.position = new Vector3(treePos.x, 0f, treePos.z);
            }
        }
    }

    /// <summary>
    /// Reverts from the 3D scene back to the 2D scene
    /// </summary>
    public void RevertTo2D() {
        PlayerPrefs.SetFloat("sensitivity", _camera.Sensitivity / 100);
        PlayerPrefs.SetFloat("speed", _movement.Speed / 100);
        PlayerPrefs.SetInt("noclip", _movement.Noclip ? 1 : 0);
        PlayerPrefs.Save();
        EscapeTestingInput = false;
        _editor.SetActive(true);
        MapEditorManager.ReloadFlag = true;
        _editor.SetActive(true);
        SceneManager.LoadScene("MapEditor", LoadSceneMode.Single);
    }
}