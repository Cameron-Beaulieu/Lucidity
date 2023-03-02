using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Layering : MonoBehaviour{
    [SerializeField] private GameObject _layerPrefab; 
    private static GameObject _layerContainer;

    private void Start() {
        _layerContainer = GameObject.Find("LayerScrollContent");
        gameObject.GetComponent<Button>().onClick.AddListener(CreateNewLayer);
    }

    /// <summary>
    /// Adds a layer to the layer menu and a new dictionary to the MapEditorManager Layers list.
    /// </summary>
    /// <param name="layerPrefab">
    /// <c>GameObject</c> corresponding to the prefab of a layer.
    /// </param>
    /// <returns>
    /// <c>GameObject List</c> containing the new layer GameObject.
    /// </returns>
    public static List<GameObject> AddLayer(GameObject layerPrefab){
        MapEditorManager.Layers.Add(new Dictionary<int, MapObject>());
        Vector3 newPosition = new Vector3(150, 0, 0);
        GameObject newLayer = (GameObject) Instantiate(
            layerPrefab, _layerContainer.transform);
        newLayer.transform.localPosition = newPosition;
        List<GameObject> newLayerList = new List<GameObject> {newLayer};
        return newLayerList;
    }

    /// <summary>
    /// Adds a layer to the layer menu, but not to the MapEditorManager Layers list
    /// </summary>
    /// <param name="layerPrefab">
    /// <c>GameObject</c> corresponding to the prefab of a layer.
    /// </param>
    /// <returns>
    /// <c>GameObject List</c> containing the new layer GameObject.
    /// </returns>
    public static List<GameObject> RemakeLayer(GameObject layerPrefab){
        Vector3 newPosition = new Vector3(150, 0, 0);
        GameObject newLayer = (GameObject) Instantiate(
            layerPrefab, _layerContainer.transform);
        newLayer.transform.localPosition = newPosition;
        List<GameObject> newLayerList = new List<GameObject> {newLayer};
        return newLayerList;
    }

    /// <summary>
    /// Creates a new layer using the AddLayer method and updates the Undo/Redo linked list.
    /// </summary>
    private void CreateNewLayer(){
        List<GameObject> newLayerList = AddLayer(_layerPrefab);

        // Adding CreateLayerAction to Undo/Redo LinkedList
        if (MapEditorManager.Actions == null) {
            MapEditorManager.Actions = new LinkedList<EditorAction>();
            MapEditorManager.Actions.AddFirst(new CreateLayerAction(newLayerList));
            MapEditorManager.CurrentAction = MapEditorManager.Actions.First;
        } else {
            if (MapEditorManager.CurrentAction != null && 
                MapEditorManager.CurrentAction.Next != null) {
                // These actions can no longer be redone
                MapEditorManager.PermanentlyDeleteActions(MapEditorManager.CurrentAction.Next);
                LinkedListNode<EditorAction> actionToRemove = MapEditorManager.CurrentAction.Next;
                while (actionToRemove != null) {
                    MapEditorManager.Actions.Remove(actionToRemove);
                    actionToRemove = actionToRemove.Next;
                }
                MapEditorManager.Actions.AddAfter(MapEditorManager.CurrentAction, 
                    new CreateLayerAction(newLayerList));
                MapEditorManager.CurrentAction = MapEditorManager.CurrentAction.Next;
            } else if (MapEditorManager.CurrentAction != null) {
                MapEditorManager.Actions.AddAfter(MapEditorManager.CurrentAction, 
                    new CreateLayerAction(newLayerList));
                MapEditorManager.CurrentAction = MapEditorManager.CurrentAction.Next;
            } else if (MapEditorManager.CurrentAction == null && 
                MapEditorManager.Actions != null) {
                // There is only one action and it has been undone
                MapEditorManager.PermanentlyDeleteActions(MapEditorManager.Actions.First);
                MapEditorManager.Actions.Clear();
                MapEditorManager.Actions.AddFirst(new CreateLayerAction(newLayerList));
                MapEditorManager.CurrentAction = MapEditorManager.Actions.First;
            }
       }
    }

    /// <summary>
    /// Deletes the most recently added layer from all Lists and from the scene.
    /// </summary>
    public static void DeleteLastLayer(){
        Layer.LayerStatus.Remove(Layer.LayerNames.Last());
        Layer.LayerIndex.Remove(Layer.LayerNames.Last());
        MapEditorManager.Layers.RemoveAt(MapEditorManager.Layers.Count - 1);
        GameObject layerToBeRemoved = GameObject.Find(Layer.LayerNames.Last());
        Destroy(layerToBeRemoved);
        Layer.LayerNames.Remove(Layer.LayerNames.Last());
        Layer.SelectedChangeSelectedLayer(Layer.LayerNames.Last());

    }

}