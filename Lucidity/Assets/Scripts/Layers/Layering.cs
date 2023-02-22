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
        gameObject.GetComponent<Button>().onClick.AddListener(AddLayer);
    }

    /// <summary>
    /// Adds a layer to the layer menu and a new dictionary to the MapEditorManager Layers list.
    /// </summary>
    private void AddLayer(){
        MapEditorManager.Layers.Add(new Dictionary<int, MapObject>());
        float layerHeight = _layerPrefab.GetComponent<RectTransform>().rect.height;
        Vector3 newPosition = new Vector3(150, -30 - (layerHeight * MapEditorManager.Layers.Count), 0);
        GameObject newLayer = (GameObject) Instantiate(
            _layerPrefab, _layerContainer.transform);
        newLayer.transform.localPosition = newPosition;
    }

}