using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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
    public static Dictionary<int, GameObject> IdToGameObjectMapping 
        = new Dictionary<int, GameObject>();
    public static int CurrentLayer;
    [SerializeField] private GameObject _layerPrefab;
    public static LinkedList<EditorAction> Actions;
    public static Dictionary<string, Texture2D> ToolToCursorMap =
        new Dictionary<string, Texture2D>();
    private static LinkedListNode<EditorAction> _currentAction;
    public static GameObject Map;
    public static GameObject MapContainer;
    public static Vector2 SpawnPoint;
    public static bool Reversion;
    public static bool ReloadFlag;
    public static bool LoadFlag = false;
    private static int _currentButtonPressed;

    public static LinkedListNode<EditorAction> CurrentAction {
        get { return _currentAction; }
        set { _currentAction = value; }
    }

    public static int CurrentButtonPressed {
        get { return _currentButtonPressed; }
        set { _currentButtonPressed = value; }
    }

    private void Awake() {
        if (StartupScreen.FilePath != null && MapData.FileName != null && !ReloadFlag) {
            // Case 1: Map loaded from Map Editor via "Open File"
            Util.ResetStaticVariables();
            Util.ResetAssetButtons();
            LoadMap();
            MapData.FileName = StartupScreen.FilePath;
        } else if (StartupScreen.FilePath != null && !ReloadFlag) {
            // Case 2: Map loaded from Startup Screen via "Load existing map"
            LoadMap();
            MapData.FileName = StartupScreen.FilePath;
        } else if (MapData.FileName != null && !ReloadFlag) {
            // Case 3: Map created from MapEditor via "New File"
            Util.ResetStaticVariables();
            Util.ResetAssetButtons();
            SelectedBiome = CreateNewMap.ChosenBiome;
        } else {
            // Case 4: Map created from Startup Screen via "Create new map"
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

        if(!ReloadFlag || LoadFlag) {
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
            if (ToolToCursorMap["Brush Tool"]) {
                Cursor.SetCursor(ToolToCursorMap["Brush Tool"], new Vector2(16f, 16f), 
                                 CursorMode.Auto);
            } else {
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            }
            CurrentLayer = 0;
        } 
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start() {
        if (!ReloadFlag && StartupScreen.FilePath == null) {
            List<(int, GameObject)> tempLayerList = Layering.AddLayer(_layerPrefab);
        } else if (ReloadFlag) {
            ReloadScene();
            Tool.ChangeTools("Brush Tool");
            Util.ResetAssetButtons();
            ReloadFlag = false;
        }
        // destroy all other instances of MapEditorManager (keeps the newest one)
        GameObject[] instances = GameObject.FindGameObjectsWithTag("MapEditorManager");
        if (instances.Length > 1) {
            foreach (GameObject instance in instances) {
                if (instance != gameObject) {
                    Destroy(instance);
                }
            }
        }
    }

    private void Update() {
        Vector2 worldPosition = Mouse.GetMousePosition();
        if (Input.GetMouseButton(0) && AssetButtons[_currentButtonPressed].Clicked 
            && Tool.ToolStatus["Brush Tool"]) {
            PaintAtPosition(worldPosition);
        } else if (!Input.GetMouseButton(0) && AssetButtons[_currentButtonPressed].Clicked 
            && Tool.ToolStatus["Brush Tool"]) {
                DynamicBoundingBox.DeleteDynamicBoundingBoxes();
        }
        // TODO: Implement other actions here
    }

    /// <summary>
    /// Paints the asset at the given position.
    /// </summary>
    public void PaintAtPosition(Vector2 worldPosition) {
        Reversion = false;
        LoadFlag = false;
        GameObject activeImage = GameObject.FindGameObjectWithTag("AssetImage");
        if (activeImage == null) {
            DynamicBoundingBox.CreateDynamicAssetImage(AssetImage[_currentButtonPressed],
                                                        worldPosition);
            activeImage = GameObject.FindGameObjectWithTag("AssetImage");
        }
        if (Mouse.LastMousePosition != worldPosition) {
            List<GameObject> newMapObjects = new List<GameObject>();
            GameObject dynamicBoundingBox = DynamicBoundingBox.CreateDynamicBoundingBox(
                AssetPrefabs[_currentButtonPressed],
                worldPosition);
            if (dynamicBoundingBox != null
                    && !dynamicBoundingBox.GetComponent<AssetCollision>().IsInvalidPlacement()
                    && dynamicBoundingBox.GetComponent<AssetCollision>()
                        .GetDynamicCollision() == false) {
                List<GameObject> newGameObjects =
                    DynamicBoundingBox.CreateAssets(AssetPrefabs[_currentButtonPressed],
                                                    dynamicBoundingBox);
                foreach (GameObject newGameObject in newGameObjects) {
                    if (newGameObject != null 
                        && !MapObjects.ContainsKey(newGameObject.GetInstanceID())) {
                        newMapObjects.Add(newGameObject);
                        AddNewMapObject(newGameObject, AssetNames[_currentButtonPressed],
                                        newGameObject.transform.parent.gameObject, MapObjects,
                                        _currentButtonPressed);
                        AddNewMapObject(newGameObject,
                                        AssetNames[_currentButtonPressed],
                                        newGameObject.transform.parent.gameObject,
                                        Layers[CurrentLayer], _currentButtonPressed);
                    }
                }
            } else {
                Destroy(dynamicBoundingBox);
            }
            List<(int, GameObject)> actionRelatedObjects = new List<(int, GameObject)>();
            foreach (GameObject newMapObject in newMapObjects) {
                actionRelatedObjects.Add((newMapObject.GetInstanceID(), newMapObject));
            }
            if (newMapObjects.Count == 0) {
                // Don't add action to history if there are no objects attached to it
            } else if (Actions == null) {
                Actions = new LinkedList<EditorAction>();
                Actions.AddFirst(new PaintAction(actionRelatedObjects));
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
                    Actions.AddAfter(_currentAction, new PaintAction(actionRelatedObjects));
                    _currentAction = _currentAction.Next;
                } else if (_currentAction != null) {
                    Actions.AddAfter(_currentAction, new PaintAction(actionRelatedObjects));
                    _currentAction = _currentAction.Next;
                } else if (_currentAction == null && Actions != null) {
                    // There is only one action and it has been undone
                    PermanentlyDeleteActions(Actions.First);
                    Actions.Clear();
                    Actions.AddFirst(new PaintAction(actionRelatedObjects));
                    _currentAction = Actions.First;
                }
            }
            Mouse.LastMousePosition = worldPosition;
        }
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
                foreach ((int id, GameObject obj) in actionToDelete.Value.RelatedObjects) {
                    MapObjects.Remove(id);
                    // Remove the related object from whichever layer it was on
                    if (LayerContainsMapObject(id) >= 0) {
                        Layers[LayerContainsMapObject(id)].Remove(id);
                    }
                    Destroy(obj);
                }
            } else if (actionToDelete.Value.Type == EditorAction.ActionType.CreateLayer) {
                actionToDelete.Value.RelatedObjects[0].Item2.GetComponent<Layer>()
                    .PermanentlyDeleteLayer();
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
                    foreach ((int id, GameObject obj) in actionToRedo.RelatedObjects) {
                        if (obj != null) {
                            MapObjects[id].IsActive = true;
                            Layers[LayerContainsMapObject(id)][id].IsActive = true;
                            obj.SetActive(true);
                        }
                    }
                    break;
                case EditorAction.ActionType.DeleteMapObject:
                    foreach ((int id, GameObject obj) in actionToRedo.RelatedObjects) {
                        if (obj != null) {
                            MapObjects[id].IsActive = false;
                            Layers[((DeleteMapObjectAction) actionToRedo).LayerID][id].IsActive
                                 = false;
                            Layers[((DeleteMapObjectAction) actionToRedo).LayerID].Remove(id);
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
                    actionToRedo.RelatedObjects[0].Item2.SetActive(true);
                    CurrentLayer = Layer.LayerIndex[actionToRedo.RelatedObjects[0].Item2.name];
                    Layer.SelectedChangeSelectedLayer(Layer.LayerNames[Layer.LayerIndex
                        [actionToRedo.RelatedObjects[0].Item2.name]]);
                    break;
                case EditorAction.ActionType.DeleteLayer:
                    foreach ((int id, GameObject obj) in actionToRedo.RelatedObjects) {
                        if (obj != null && obj != actionToRedo.RelatedObjects.Last().Item2) {
                            Layers[LayerContainsMapObject(id)][id].IsActive = false;
                            obj.SetActive(false);
                        } else if (obj == actionToRedo.RelatedObjects.Last().Item2) {
                            CurrentLayer = Layer.LayerIndex[obj.name] - 1;
                            Layer.SelectedChangeSelectedLayer(Layer.LayerNames
                                [Layer.LayerIndex[obj.name] - 1]);
                            Layer.LayerDeletions[obj.name] = true;
                            obj.SetActive(false);
                        }
                    }
                    Layer.NumberOfActiveLayers--;
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
                    foreach ((int id, GameObject obj) in actionToUndo.RelatedObjects) {
                        if (obj != null) {
                            MapObjects[id].IsActive = false;
                            Layers[LayerContainsMapObject(id)][id].IsActive = false;
                            obj.SetActive(false);
                        }
                    }
                    break;
                case EditorAction.ActionType.DeleteMapObject:
                    foreach ((int id, GameObject obj) in actionToUndo.RelatedObjects) {
                        if (obj != null) {
                            MapObjects[id].IsActive = true;
                            Layers[((DeleteMapObjectAction) actionToUndo).LayerID].Add
                                (id, MapObjects[id]);
                            Layers[((DeleteMapObjectAction) actionToUndo).LayerID][id].IsActive
                                 = true;
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
                    CurrentLayer = Layer.LayerIndex[actionToUndo.RelatedObjects[0].Item2.name] - 1;
                    Layer.SelectedChangeSelectedLayer(Layer.LayerNames[Layer.LayerIndex
                        [actionToUndo.RelatedObjects[0].Item2.name] - 1]);
                    actionToUndo.RelatedObjects[0].Item2.SetActive(false);
                    break;
                case EditorAction.ActionType.DeleteLayer:
                    foreach ((int id, GameObject obj) in actionToUndo.RelatedObjects) {
                        if (obj != null && obj != actionToUndo.RelatedObjects.Last().Item2) {
                            Layers[LayerContainsMapObject(id)][id].IsActive = true;
                            obj.SetActive(true);
                        } else if (obj == actionToUndo.RelatedObjects.Last().Item2) {
                            obj.SetActive(true);
                            CurrentLayer = Layer.LayerIndex[obj.name];
                            Layer.SelectedChangeSelectedLayer(Layer.LayerNames[Layer
                                .LayerIndex[obj.name]]);
                            Layer.LayerDeletions[obj.name] = false;
                        }
                    }
                    Layer.NumberOfActiveLayers++;
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
                                Dictionary<int, MapObject> mapObjectDictionary,
                                int assetPrefabIndex) {
        MapObject newMapObject = new MapObject(newGameObject.GetInstanceID(), name, 
            assetPrefabIndex, new Vector2(newGameObject.transform.localPosition.x, 
                                          newGameObject.transform.localPosition.y),  
            new Vector2(parentGameObject.transform.localPosition.x, 
                        parentGameObject.transform.localPosition.y),
            new Vector3(parentGameObject.transform.localScale.x - Zoom.zoomFactor, 
                        parentGameObject.transform.localScale.y - Zoom.zoomFactor, 
                        parentGameObject.transform.localScale.z - Zoom.zoomFactor), 
            newGameObject.transform.rotation, true);
        mapObjectDictionary.Add(newMapObject.Id, newMapObject);
        if (!IdToGameObjectMapping.ContainsKey(newGameObject.GetInstanceID())) {
            IdToGameObjectMapping.Add(newGameObject.GetInstanceID(), newGameObject);
        }
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
        LoadFlag = true;
        AssetCollision.LayerCollisions.Clear();
        MapContainer = GameObject.Find("Map Container");
        MapData loadedMap = MapData.Deserialize(StartupScreen.FilePath);
        LoadMapFromMapData(loadedMap);
    }

    /// <summary>
    /// Loads a map from the specified <c>MapData</c> object.
    /// </summary>
    /// <param name="mapData">
    /// The <c>MapData</c> object to load the map from.
    /// </param>
    public void LoadMapFromMapData(MapData mapData) {
        SelectedBiome = mapData.Biome;
        SpawnPoint = mapData.SpawnPoint;
        GameObject.Find("Spawn Point").transform.localPosition = 
            new Vector3(SpawnPoint.x, SpawnPoint.y, 0);
        GameObject.Find("Spawn Point").GetComponent<CircleCollider2D>().enabled = false;
        Layer.LayerToBeNamed = 0;

        Layer.LayerIndex.Clear();
        Layer.LayerStatus.Clear();
        Layer.LayerNames.Clear();
        Layer.LayerDeletions.Clear();

        for (int i = 0; i < mapData.LayerNames.Count; i++) {            
            // fill in LayerIndex and LayerStatus dictionaries
            Layer.LayerIndex.Add(mapData.LayerNames[i], i);
            Layer.LayerStatus.Add(mapData.LayerNames[i], false);
            Layer.LayerNames.Add(mapData.LayerNames[i]);
            Layer.LayerDeletions.Add(mapData.LayerNames[i], false);
        }

        for (int i = 0; i < mapData.LayerNames.Count; i++) {
            // Create a list for each layer
            List<(int, GameObject)> tempLayerList = Layering.AddLayer(_layerPrefab);
            tempLayerList[0].Item2.name = mapData.LayerNames[i];
        }

        // Select the layer with index 0
        Layer.SelectedChangeSelectedLayer(mapData.LayerNames[0]);

        Dictionary<int, MapObject> newMapObjects = new Dictionary<int, MapObject>();
        Dictionary<int, GameObject> newIdMapping = new Dictionary<int, GameObject>();

        foreach (MapObject mapObject in mapData.MapObjects) {
            GameObject  newGameObject = RebuildMapObject(mapObject, newMapObjects);
            newIdMapping.Add(newGameObject.GetInstanceID(), newGameObject);
            Layers[Layer.LayerIndex[mapObject.LayerName]].Add(
                newGameObject.GetInstanceID(), newMapObjects[newGameObject.GetInstanceID()]);
        }
        MapObjects = newMapObjects;
        IdToGameObjectMapping = newIdMapping;
        GameObject.Find("Spawn Point").GetComponent<CircleCollider2D>().enabled = true;
    }

    /// <summary>
    /// Converts from the 2D scene to the 3D scene.
    /// </summary>
    public void ConvertTo3D() {
        Reversion = false;
        LoadFlag = false;
        SpawnPoint = GameObject.Find("Spawn Point").transform.localPosition;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        SceneManager.LoadScene("3DMap", LoadSceneMode.Single);
    }

    /// <summary>
    /// Remakes the 2D scene upon it being reloaded from the 3D view.
    /// </summary>
    private void ReloadScene() {
        Reversion = true;
        LoadFlag = false;
        
        List<string> tempLayerNames = new List<string>(Layer.LayerNames);
        List<bool> tempDeletions = Layer.LayerDeletions.Values.ToList();

        Layer.LayerIndex.Clear();
        Layer.LayerStatus.Clear();
        Layer.LayerNames.Clear();
        Layer.LayerDeletions.Clear();
        Layer.NumberOfActiveLayers = 0;

        int tempCurrentLayer = CurrentLayer;

        for (int i = 0; i < tempLayerNames.Count; i++) {
            Layer.LayerToBeNamed = 0;
            // fill in LayerIndex and LayerStatus dictionaries
            Layer.LayerIndex.Add(tempLayerNames[i], i);
            if (i == CurrentLayer) {
                Layer.LayerStatus.Add(tempLayerNames[i], true);
            } else {
                Layer.LayerStatus.Add(tempLayerNames[i], false);
            }
            Layer.LayerNames.Add(tempLayerNames[i]);
            Layer.LayerDeletions.Add(tempLayerNames[i], tempDeletions[i]);
        }

        for (int i = 0; i <tempLayerNames.Count; i++) {
            // Create a list for each layer
            List<(int, GameObject)> tempLayerList = Layering.RemakeLayer(_layerPrefab);
            tempLayerList[0].Item2.transform.GetChild(0).gameObject.GetComponent<LayerName>()
                .LayerText.text = Layer.LayerNames[i];
            tempLayerList[0].Item2.name = Layer.LayerNames[i];
        }

        Layer.SelectedChangeSelectedLayer(Layer.LayerNames[tempCurrentLayer]);
        CurrentLayer = tempCurrentLayer;

        // Reloading MapObjects
        Dictionary<int, MapObject> newMapObjects = new Dictionary<int, MapObject>();
        Dictionary<int, GameObject> mapObjectsMapping = new Dictionary<int, GameObject>();
        Dictionary<int, GameObject> newIdMapping = new Dictionary<int, GameObject>();
        List<Dictionary<int, MapObject>> newLayers = new List<Dictionary<int, MapObject>>();

        MapContainer = GameObject.Find("Map Container");
        foreach (Dictionary<int, MapObject> layer in Layers) {
            Dictionary<int, MapObject> currentLayer = new Dictionary<int, MapObject>();
            foreach (KeyValuePair <int, MapObject> mapObject in layer) {
               GameObject newGameObject = RebuildMapObject(mapObject.Value, newMapObjects);
                newIdMapping.Add(newGameObject.GetInstanceID(), newGameObject);
                mapObjectsMapping.Add(mapObject.Value.Id, newGameObject);
                currentLayer.Add(newGameObject.GetInstanceID(), 
                             newMapObjects[newGameObject.GetInstanceID()]);
                if (!mapObject.Value.IsActive) {
                    currentLayer[newGameObject.GetInstanceID()].IsActive = false;
                    newGameObject.SetActive(false);
                }
            }
            newLayers.Add(currentLayer);
        }

        // Swapping GameObject's in editor action linked list
        if (Actions != null) {
            LinkedListNode<EditorAction> pointer = Actions.First;

            while (pointer != null) {
                for (int i = 0; i < pointer.Value.RelatedObjects.Count; i++) {
                    if (pointer.Value.Type == EditorAction.ActionType.DeleteMapObject) {
                        MapObject mapObject = ((DeleteMapObjectAction) pointer.Value).MapObject;
                        GameObject newGameObject = RebuildMapObject(mapObject, newMapObjects);
                        newIdMapping.Add(newGameObject.GetInstanceID(), newGameObject);
                        mapObjectsMapping.Add(mapObject.Id, newGameObject);
                        if (!mapObject.IsActive) {
                                newGameObject.SetActive(false);
                        }
                    } else if (pointer.Value.Type == EditorAction.ActionType.DeleteLayer 
                        && i == pointer.Value.RelatedObjects.Count - 1) {
                        GameObject layer = GameObject.Find(((DeleteLayerAction) pointer
                            .Value).LayerName);
                        pointer.Value.RelatedObjects[i] = (layer.GetInstanceID(), layer);
                    }
                    int pointerId = pointer.Value.RelatedObjects[i].Item1;
                    // ensure the object is a MapObject
                    if (mapObjectsMapping.ContainsKey(pointerId)) {
                        pointer.Value.RelatedObjects[i] = (
                            mapObjectsMapping[pointerId]
                                .GetInstanceID(), 
                            mapObjectsMapping[pointerId]
                        );
                    }
                }
                pointer = pointer.Next;
            }
        }

        // Resetting MapObjects  and Layers Dictionaries
        MapObjects = new Dictionary<int, MapObject>(newMapObjects);
        Layers = new List<Dictionary<int, MapObject>>(newLayers);
        IdToGameObjectMapping = new Dictionary<int, GameObject>(newIdMapping);

        // Swapping MapObjects in LayerCollisions List
        foreach (List<MapObject> mapObjects in AssetCollision.LayerCollisions) {
            mapObjects[0] = MapObjects[mapObjectsMapping[mapObjects[0].Id].GetInstanceID()];
            mapObjects[1] = MapObjects[mapObjectsMapping[mapObjects[1].Id].GetInstanceID()];
        }

        foreach (string name in Layer.LayerNames) {
            if (Layer.LayerDeletions[name] == true) {
                foreach ((int id, MapObject mapObject) in Layers[Layer.LayerIndex[name]]) {
                    IdToGameObjectMapping[mapObject.Id].SetActive(false);
                }
                Layer.NumberOfActiveLayers--;
                GameObject.Find(name).SetActive(false);
            }
        }
    }

    /// <summary>
    /// Rebuilds a <c>GameObject</c> and <c>MapObject</c> based on a current <c>MapObject</c> and then adds the new
    /// <c>MapObject</c> to a dictionary used to track it.
    /// </summary>
    /// <param name="mapObject">
    /// The <c>MapObject</c> that will be used as the template for the new one being built
    /// </param>
    /// <param name="newMapObjects">
    /// The dictionary being used to track the new <c>MapObject</c> that has been added
    /// </param>
    /// <returns>
    /// The <c>GameObject</c> that was created as part of creating the new MapObject
    /// </returns>
    private GameObject RebuildMapObject(MapObject mapObject, 
                                        Dictionary<int, MapObject> newMapObjects) {
        GameObject newParent = new GameObject();
        newParent.name = AssetPrefabs[mapObject.PrefabIndex].name + " Parent";
        newParent.transform.SetParent(MapContainer.transform, true);
        newParent.transform.localPosition = new Vector3(mapObject.MapOffset.x, 
                                                        mapObject.MapOffset.y, 0);
        newParent.transform.localScale = mapObject.Scale;
        GameObject newGameObject = (GameObject) Instantiate(
            AssetPrefabs[mapObject.PrefabIndex], newParent.transform);
        newGameObject.transform.localPosition = new Vector3(mapObject.MapPosition.x, 
            mapObject.MapPosition.y, 0);
        newGameObject.transform.rotation = mapObject.Rotation;
        newGameObject.transform.localScale = 
            new Vector3(newGameObject.transform.localScale.x + Zoom.zoomFactor, 
                        newGameObject.transform.localScale.y + Zoom.zoomFactor, 
                        newGameObject.transform.localScale.z + Zoom.zoomFactor);
        AddNewMapObject(newGameObject, mapObject.Name, 
                        newParent, newMapObjects, mapObject.PrefabIndex);
        if (!mapObject.IsActive) {
            newMapObjects[newGameObject.GetInstanceID()].IsActive = false;
        }
        return newGameObject;
    }
}