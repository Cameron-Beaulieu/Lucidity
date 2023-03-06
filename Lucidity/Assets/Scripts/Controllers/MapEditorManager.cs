using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MapEditorManager : MonoBehaviour {
    public List<AssetController> AssetButtons;
    public List<GameObject> AssetPrefabs;
    public List<GameObject> AssetImage;
    public Biome SelectedBiome;
    public List<string> AssetNames;
    public List<Texture2D> CursorTextures;
    public static Dictionary<int, MapObject> MapObjects = new Dictionary<int, MapObject>();
    public static List<Dictionary<int, MapObject>> Layers = new List<Dictionary<int, MapObject>>();
    public int CurrentLayer = 0;
    [SerializeField] private GameObject _layerPrefab;
    public static LinkedList<EditorAction> Actions;
    public static Dictionary<string, Texture2D> ToolToCursorMap = 
        new Dictionary<string, Texture2D>();
    private static LinkedListNode<EditorAction> _currentAction;
    public static GameObject Map;
    public static GameObject MapContainer;
    public static Vector2 SpawnPoint;
    private static int _currentButtonPressed;
    private static GameObject _lastEncounteredObject;
    public static bool ReloadFlag;

    public static LinkedListNode<EditorAction> CurrentAction {
        get { return _currentAction; }
        set { _currentAction = value; }
    }

    public static int CurrentButtonPressed {
        get { return _currentButtonPressed; }
        set { _currentButtonPressed = value; }
    }

    public static GameObject LastEncounteredObject {
        get { return _lastEncounteredObject; }
        set { _lastEncounteredObject = value; }
    }

    private void Awake() {
        if (StartupScreen.FilePath != null && !ReloadFlag) {
            // Static variables must be reset if a new map is loaded from another map
            Util.ResetStaticVariables();
            LoadMap();
            MapData.FileName = StartupScreen.FilePath;
        } else {
            SelectedBiome = CreateNewMap.ChosenBiome;
        }

        Map = GameObject.Find("Map");
        MapContainer = GameObject.Find("Map Container");
        // below is a temp fix while moving objects is not implemented; allows for testing
        // different spawn points by moving it within the editor before playing
        SpawnPoint = GameObject.Find("Spawn Point").transform.localPosition; 

        Tool.PaintingMenu = GameObject.Find("Painting Menu");
        Tool.SelectionMenu = GameObject.Find("Selection Menu");
        Tool.SelectionOptions = GameObject.Find("SelectionOptionsScrollContent");
        Tool.SpawnPointOptions = GameObject.Find("SpawnPointOptionsScrollContent");
        Tool.SelectionMenu.SetActive(false);
        GameObject.Find("Undo").GetComponent<Button>().onClick.AddListener(Undo);
        GameObject.Find("Redo").GetComponent<Button>().onClick.AddListener(Redo);
        GameObject.Find("3D-ify Button").GetComponent<Button>().onClick.AddListener(ConvertTo3D);
        GameObject[] selectableTools = GameObject.FindGameObjectsWithTag("SelectableTool");

        if(!ReloadFlag) {
        foreach (GameObject tool in selectableTools) {
            if (tool.name == "Brush Tool") {
                Tool.ToolStatus.Add(tool.name, true);
            } else {
                Tool.ToolStatus.Add(tool.name, false);
            }
            Tool.ToolKeys.Add(tool.name);
        }

        foreach (Texture2D cursor in CursorTextures) {
            ToolToCursorMap.Add(cursor.name, cursor);
        }
        DontDestroyOnLoad(this.gameObject);
        } 
    }

    private void Start() {
        if(!ReloadFlag) {
            List<GameObject> tempLayerList = Layering.AddLayer(_layerPrefab);
        }
        else {
            ReloadScene();
            Tool.ChangeTools("Brush Tool");
        }
    }

    private void Update() {
        Vector2 worldPosition = Mouse.GetMousePosition();
        if (Input.GetMouseButton(0) && AssetButtons[_currentButtonPressed].Clicked 
            && Tool.ToolStatus["Brush Tool"]) {
            PaintAtPosition(worldPosition);
        }
        // TODO: Implement other actions here
    }

    /// <summary>
    /// Paints the asset at the given position.
    /// </summary>
    public void PaintAtPosition(Vector2 worldPosition) {
        GameObject activeImage = GameObject.FindGameObjectWithTag("AssetImage");
        if (activeImage == null) {
            AssetController.CreateFollowingImage(AssetImage[_currentButtonPressed]);
            activeImage = GameObject.FindGameObjectWithTag("AssetImage");
        }
        Collider2D collider = activeImage.GetComponent<Collider2D>();
        float assetWidth = collider.bounds.size.x; //activeImage.transform.localScale.x;
        float assetHeight = collider.bounds.size.y; //activeImage.transform.localScale.y;
        // Check if mouse position relative to its last position and the previously encountered
        // asset would allow for a legal placement. Reduces unnecessary computing
        if (Mouse.LastMousePosition != worldPosition
                && (LastEncounteredObject == null
                    || Mathf.Abs(worldPosition.x - LastEncounteredObject.transform.position.x)
                        >= assetWidth
                    || Mathf.Abs(worldPosition.y - LastEncounteredObject.transform.position.y)
                        >= assetHeight)) {
            List<GameObject> newMapObjects = new List<GameObject>();

            for (int i = 0; i < AssetOptions.AssetCount; i++) {
                GameObject newParent = new GameObject();
                newParent.name = AssetPrefabs[_currentButtonPressed].name + " Parent";
                newParent.transform.SetParent(MapContainer.transform, true);
                newParent.transform.position = new Vector3(worldPosition.x + i*2, 
                                                            worldPosition.y, 0);
                newParent.transform.localPosition = new Vector3(
                    newParent.transform.localPosition.x,
                    newParent.transform.localPosition.y, 0);

                GameObject newGameObject = (GameObject) Instantiate(
                    AssetPrefabs[_currentButtonPressed],
                    new Vector3(worldPosition.x + i*2, worldPosition.y, 
                                88), // the world Z position of the UI
                    Quaternion.identity, newParent.transform);
                newGameObject.transform.localScale = 
                    new Vector3(newGameObject.transform.localScale.x + Zoom.zoomFactor, 
                                newGameObject.transform.localScale.y + Zoom.zoomFactor, 
                                newGameObject.transform.localScale.z + Zoom.zoomFactor);

                if (newGameObject != null && !newGameObject.GetComponent<AssetCollision>()
                        .IsInvalidPlacement()) {
                    newMapObjects.Add(newGameObject);
                    AddNewMapObject(newGameObject, AssetNames[_currentButtonPressed], 
                                    newParent, MapObjects);
                    AddNewMapObject(newGameObject, AssetNames[_currentButtonPressed], 
                                    newParent, Layers[CurrentLayer]);
                } else {
                    Destroy(newParent);
                }
            }
            if (newMapObjects.Count == 0) {
                // Don't add action to history if there are no objects attached to it
            } else if (Actions == null) {
                Actions = new LinkedList<EditorAction>();
                Actions.AddFirst(new PaintAction(newMapObjects));
                _currentAction = Actions.First;
            } else {
                if (_currentAction != null && _currentAction.Next != null) {
                    // These actions can no longer be redone
                    PermanentlyDeleteActions(_currentAction.Next);
                    LinkedListNode<EditorAction> actionToRemove = _currentAction.Next;
                    while (actionToRemove != null) {
                        LinkedListNode<EditorAction> temp = actionToRemove.Next;
                        Actions.Remove(actionToRemove);
                        actionToRemove = temp;
                    }
                    Actions.AddAfter(_currentAction, new PaintAction(newMapObjects));
                    _currentAction = _currentAction.Next;
                } else if (_currentAction != null) {
                    Actions.AddAfter(_currentAction, new PaintAction(newMapObjects));
                    _currentAction = _currentAction.Next;
                } else if (_currentAction == null && Actions != null) {
                    // There is only one action and it has been undone
                    PermanentlyDeleteActions(Actions.First);
                    Actions.Clear();
                    Actions.AddFirst(new PaintAction(newMapObjects));
                    _currentAction = Actions.First;
                }
            }
            if (newMapObjects.Count > 0) {
                _lastEncounteredObject = newMapObjects[0];
            }
        }
        Mouse.LastMousePosition = worldPosition;
    }

    /// <summary>
    /// Permanently deletes all map objects associated with actions in the list. This is done to
    /// ensure no inactive map objects associated with actions that can no longer be redone are
    /// left in the scene.
    /// </summary>
    /// <param name="actionToDelete">
    /// <c>LinkedListNode</c> of the <c>EditorAction</c> to remove from the linked list (and its
    /// associated actions).
    /// </param>
    public static void PermanentlyDeleteActions(LinkedListNode<EditorAction> actionToDelete) {
        while (actionToDelete != null) {
            if (actionToDelete.Value.Type == EditorAction.ActionType.Paint) {
                foreach (GameObject obj in actionToDelete.Value.RelatedObjects) {
                    int id = obj.GetInstanceID();
                    MapObjects.Remove(id);
                    // Remove the related object from whichever layer it was on
                    Layers[LayerContainsMapObject(id)].Remove(id);
                    Destroy(obj);
                }
            }
            actionToDelete = actionToDelete.Next;
        }
    }

    /// <summary>
    /// Action redo functionality.
    /// </summary>
    public void Redo() {
        if ((_currentAction == null && Actions != null) || _currentAction.Next != null) {
            // If CurrentAction is null and Actions is not, then we want to redo from the
            // beginning; else, we want to redo from the current section
            EditorAction actionToRedo = Actions.First.Value;
            if (_currentAction != null) {
                actionToRedo = _currentAction.Next.Value;
            }

            switch(actionToRedo.Type) {
                case EditorAction.ActionType.Paint:
                    foreach (GameObject obj in actionToRedo.RelatedObjects) {
                        if (obj != null) {
                            int id = obj.GetInstanceID();
                            MapObjects[id].IsActive = true;
                            // TODO: uncomment during 3D layers ticket
                            // Commented out until 2D reversion with layers is complete
                            // Layers[LayerContainsMapObject(id)][id].IsActive = true;
                            obj.SetActive(true);
                        }
                    }
                    break;
                case EditorAction.ActionType.DeleteMapObject:
                    foreach (GameObject obj in actionToRedo.RelatedObjects) {
                        if (obj != null) {
                            int id = obj.GetInstanceID();
                            MapObjects[id].IsActive = false;
                            // TODO: uncomment during 3D layers ticket
                            // Commented out until 2D reversion with layers is complete
                            // Layers[LayerContainsMapObject(id)][id].IsActive = false;
                            obj.SetActive(false);
                        }
                    }
                    break;
                case EditorAction.ActionType.MoveMapObject:
                    // TODO: Implement
                    break;
                case EditorAction.ActionType.ResizeMapObject:
                    // TODO: Implement
                    break;
                case EditorAction.ActionType.RotateMapObject:
                    // TODO: Implement
                    break;
                case EditorAction.ActionType.CreateLayer:
                    List<GameObject> newLayerList = Layering.AddLayer(_layerPrefab);
                    break;
                case EditorAction.ActionType.DeleteLayer:
                    // TODO: Implement
                    break;
                case EditorAction.ActionType.MoveLayer:
                    // TODO: Implement
                    break;
                case EditorAction.ActionType.RenameLayer:
                    // TODO: Implement
                    break;
            }

            if (_currentAction == null) {
                _currentAction = Actions.First;
            } else {
                _currentAction = _currentAction.Next;
            }
        }
    }

    /// <summary>
    /// Action redo functionality.
    /// </summary>
    public void Undo() {
        if (_currentAction != null) {
            EditorAction actionToUndo = _currentAction.Value;

            switch (actionToUndo.Type) {
                case EditorAction.ActionType.Paint:
                    foreach (GameObject obj in actionToUndo.RelatedObjects) {
                        if (obj != null) {
                            int id = obj.GetInstanceID();
                            MapObjects[id].IsActive = false;
                            // TODO: uncomment during 3D layers ticket
                            // Commented out until 2D reversion with layers is complete
                            // Layers[LayerContainsMapObject(id)][id].IsActive = false;
                            obj.SetActive(false);
                        }
                    }
                    break;
                case EditorAction.ActionType.DeleteMapObject:
                    foreach (GameObject obj in actionToUndo.RelatedObjects) {
                        if (obj != null) {
                            int id = obj.GetInstanceID();
                            MapObjects[id].IsActive = true;
                            // TODO: uncomment during 3D layers ticket
                            // Commented out until 2D reversion with layers is complete
                            // Layers[LayerContainsMapObject(id)][id].IsActive = true;
                            obj.SetActive(true);
                        }
                    }
                    break;
                case EditorAction.ActionType.MoveMapObject:
                    // TODO: Implement
                    break;
                case EditorAction.ActionType.ResizeMapObject:
                    // TODO: Implement
                    break;
                case EditorAction.ActionType.RotateMapObject:
                    // TODO: Implement
                    break;
                case EditorAction.ActionType.CreateLayer:
                    Layering.DeleteLastLayer();
                    break;
                case EditorAction.ActionType.DeleteLayer:
                    // TODO: Implement
                    break;
                case EditorAction.ActionType.MoveLayer:
                    // TODO: Implement
                    break;
                case EditorAction.ActionType.RenameLayer:
                    // TODO: Implement
                    break;
            }

            if (_currentAction.Previous != null) {
                _currentAction = _currentAction.Previous;
            }
            else {
                _currentAction = null;
            }
        }
    }

    /// <summary>
    /// Adds a new <c>MapObject</c> to the list of all the MapObjects on the 2D map
    /// </summary>
    /// <param name="newGameObject">
    /// The 2D <c>GameObject</c> that has just been added to the map
    /// </param>
    /// <param name="name">
    /// <c>string</c> represents the type of the asset (fortress, tree, etc.)
    /// </param>
    /// <param name="parentGameObject">
    /// The parent container storing the new <c>GameObject</c>
    /// </param>
    public void AddNewMapObject(GameObject newGameObject, string name, 
                                GameObject parentGameObject, 
                                Dictionary<int, MapObject> mapObjectDictionary) {
        MapObject newMapObject = new MapObject(newGameObject.GetInstanceID(), name, 
            _currentButtonPressed,
            new Vector2(newGameObject.transform.localPosition.x, 
                        newGameObject.transform.localPosition.y),  
            new Vector2(parentGameObject.transform.localPosition.x, 
                        parentGameObject.transform.localPosition.y),
            new Vector3(parentGameObject.transform.localScale.x - Zoom.zoomFactor, 
                        parentGameObject.transform.localScale.y - Zoom.zoomFactor, 
                        parentGameObject.transform.localScale.z - Zoom.zoomFactor), 
            newGameObject.transform.rotation, true);
        mapObjectDictionary.Add(newMapObject.Id, newMapObject);
    }	

    /// <summary>
    /// Given the instance ID of a <c>MapObject</c>, returns the index corresponding to the layer
    /// that the <c>MapObject</c> can be found.
    /// </summary>
    /// <param name="obj">
    /// Instance ID of the desired <c>MapObject</c> to be located
    /// </param>
    /// <returns>
    /// <c>int</c> corresponding to the layer index
    /// </returns>
    public static int LayerContainsMapObject(int objId) {
        for (int i = 0; i < Layers.Count; i++) {
            if (Layers[i].ContainsKey(objId)) {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// Loads a map stored at the specified file path.
    /// </summary>
    public void LoadMap() {
        MapContainer = GameObject.Find("Map Container");
        MapData loadedMap = MapData.Deserialize(StartupScreen.FilePath);
        LoadMapFromMapData(loadedMap);
    }

    /// <summary>
    /// Loads a map from the specified <c>MapData</c> object.
    /// </summary>
    public void LoadMapFromMapData(MapData mapData) {
        SelectedBiome = mapData.Biome;
        SpawnPoint = mapData.SpawnPoint;
        GameObject.Find("Spawn Point").transform.localPosition = 
            new Vector3(SpawnPoint.x, SpawnPoint.y, -101);

        foreach (MapObject mapObject in mapData.MapObjects) {
            GameObject newParent = new GameObject();
            newParent.name = AssetPrefabs[mapObject.PrefabIndex].name + " Parent";
            newParent.transform.SetParent(MapContainer.transform, true);
            newParent.transform.localPosition = new Vector3(mapObject.MapOffset.x, 
                                                            mapObject.MapOffset.y, 0);
            GameObject newGameObject = (GameObject) Instantiate(
                AssetPrefabs[mapObject.PrefabIndex], new Vector3(newParent.transform.position.x, 
                                                                 newParent.transform.position.y, 
                                                                 88), Quaternion.identity, newParent.transform);
            // newGameObject.transform.localPosition = new Vector3(mapObject.MapPosition.x, 
            //                                                     mapObject.MapPosition.y, 0);
            newGameObject.transform.rotation = mapObject.Rotation;
            newGameObject.transform.localScale = 
                new Vector3(newGameObject.transform.localScale.x + Zoom.zoomFactor, 
                            newGameObject.transform.localScale.y + Zoom.zoomFactor, 
                            newGameObject.transform.localScale.z + Zoom.zoomFactor);
            MapObjects.Add(newGameObject.GetInstanceID(), mapObject);
        }
    }

    /// <summary>
    /// Converts from the 2D scene to the 3D scene.
    /// </summary>
    public void ConvertTo3D() {
        SpawnPoint = GameObject.Find("Spawn Point").transform.localPosition;
        SceneManager.LoadScene("3DMap", LoadSceneMode.Single);
    }

    /// <summary>
    /// Remakes the 2D scene upon it being reloaded from the 3D view.
    /// </summary>
    private void ReloadScene() {
        // Reloading Layers
        Layer.LayerIndex.Clear();
        Layer.LayerStatus.Clear();
        Layer.LayerNames.Clear();
        foreach (Dictionary<int, MapObject> layer in Layers){
            List<GameObject> tempLayerList = Layering.RemakeLayer(_layerPrefab);
        }
        CurrentLayer = Layers.Count - 1;

        
        // TODO: During 3D with layers, will need to nest Reloading MapObjects
        // within Reloading Layers to rebuild each layer.

        // Reloading MapObjects
        Dictionary<int, MapObject> newMapObjects = new Dictionary<int, MapObject>();
        Dictionary<int, GameObject> mapObjectsMapping = new Dictionary<int, GameObject>();
        MapContainer = GameObject.Find("Map Container");
        foreach (KeyValuePair <int, MapObject> mapObject in MapObjects) {
                GameObject newParent = new GameObject();
                newParent.name = AssetPrefabs[mapObject.Value.PrefabIndex].name + " Parent";
                newParent.transform.SetParent(MapContainer.transform, true);
                newParent.transform.localPosition = new Vector3(mapObject.Value.MapOffset.x, 
                                                                mapObject.Value.MapOffset.y, 0);
                newParent.transform.localScale = mapObject.Value.Scale;
                GameObject newGameObject = (GameObject) Instantiate(
                    AssetPrefabs[mapObject.Value.PrefabIndex], newParent.transform);
                newGameObject.transform.localPosition = new Vector3(mapObject.Value.MapPosition.x, 
                                                                    mapObject.Value.MapPosition.y, 0);
                newGameObject.transform.rotation = mapObject.Value.Rotation;
                newGameObject.transform.localScale = 
                    new Vector3(newGameObject.transform.localScale.x + Zoom.zoomFactor, 
                                newGameObject.transform.localScale.y + Zoom.zoomFactor, 
                                newGameObject.transform.localScale.z + Zoom.zoomFactor);
                mapObjectsMapping.Add(mapObject.Value.Id, newGameObject);
                AddNewMapObject(newGameObject, mapObject.Value.Name, 
                                newParent, newMapObjects);
                if(mapObject.Value.IsActive == false){
                    newMapObjects[newGameObject.GetInstanceID()].IsActive = false;
                    newGameObject.SetActive(false);
                }
        }

        // Swapping GameObject's in editor action linked list
        if(Actions != null){
            LinkedListNode<EditorAction> pointer = Actions.First;

            while (pointer != null){
                    for(int i = 0; i < pointer.Value.RelatedObjects.Count; i ++){
                        pointer.Value.RelatedObjects[i] = mapObjectsMapping[pointer.Value.RelatedObjects[i].GetInstanceID()];
                    }
                pointer = pointer.Next;
            }
        }

        // Resetting MapObjects Dictionary
        MapObjects = new Dictionary<int, MapObject>(newMapObjects);
    }
}