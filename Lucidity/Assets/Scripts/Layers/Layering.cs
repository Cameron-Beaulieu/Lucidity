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
    public static void AddLayer(GameObject layerPrefab){
        MapEditorManager.Layers.Add(new Dictionary<int, MapObject>());
        Vector3 newPosition = new Vector3(150, 0, 0);
        GameObject newLayer = (GameObject) Instantiate(
            layerPrefab, _layerContainer.transform);
        newLayer.transform.localPosition = newPosition;
    }

    private void CreateNewLayer(){
        AddLayer(_layerPrefab);
    }

    public static void DeleteLastLayer(){
        Debug.Log("Deleting Last Layer");
        Layer.LayerStatus.Remove(Layer.LayerNames.Last());
        Layer.LayerIndex.Remove(Layer.LayerNames.Last());
        MapEditorManager.Layers.RemoveAt(MapEditorManager.Layers.Count - 1);
        GameObject layerToBeRemoved = GameObject.Find(Layer.LayerNames.Last());
        Debug.Log(layerToBeRemoved.name);
        Destroy(layerToBeRemoved);
        Layer.LayerNames.Remove(Layer.LayerNames.Last());
    }

}