using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Layer : MonoBehaviour {
    public static Dictionary<string, bool> LayerStatus = new Dictionary<string, bool>();
    public static Dictionary<string, int> LayerIndex = new Dictionary<string, int>();
    public static Dictionary<string, bool> LayerVisibility = new Dictionary<string, bool>();
    public static List<string> LayerNames = new List<string>();
    public static int LayerToBeNamed = -1;
    private static GameObject _layerContainer;
    private GameObject _layerTrashCan;
    private TMP_InputField _layerText;
    private GameObject _layerEdit;
    private GameObject _layerVisibility;
    private MapEditorManager _editor;
    private string _name;
    private Color _unselected = new Color(48/255f, 49/255f, 52/255f);
    public Sprite Eye;
    public Sprite EyeSlash;

    public static GameObject LayerContainer {
        get {return _layerContainer;}
        set {_layerContainer = value;}
    }

    private void Start() {
        _layerContainer = GameObject.Find("LayerScrollContent");
        _editor = GameObject.FindGameObjectWithTag("MapEditorManager")
            .GetComponent<MapEditorManager>();
        gameObject.name = "Layer" + (LayerStatus.Count).ToString();
        _name = gameObject.name;
        _layerText = gameObject.transform.GetChild(0).gameObject.GetComponent<TMP_InputField>();
        _layerText.readOnly = true;
        _layerVisibility = gameObject.transform.GetChild(1).gameObject;
        _layerVisibility.GetComponent<Button>().onClick.AddListener(ToggleVisibility);
        _layerTrashCan = gameObject.transform.GetChild(2).gameObject;
        _layerTrashCan.SetActive(false);
        _layerEdit = gameObject.transform.GetChild(3).gameObject;
        _layerEdit.GetComponent<Button>().onClick.AddListener(ChangeLayerName);
        _layerEdit.SetActive(false);
        gameObject.GetComponent<Button>().onClick.AddListener(ChangeSelectedLayer);
        // These are updated in the MapEditorManager if loaded from a file (LayerToBeNamed > -1)
        if (LayerToBeNamed == -1) {
            LayerStatus.Add(_name, false);
            LayerIndex.Add(_name, LayerIndex.Count);
            LayerNames.Add(_name);
        }
        // Names are applied to the layers after they have been loaded in the MapEditorManager
        // This ensures that layers are given the proper names if loaded from a file
        if (LayerToBeNamed >= 0 && LayerToBeNamed < LayerNames.Count) {
            _layerText.text = LayerNames[LayerToBeNamed];
            // this is the case where the last layer has been named, so LayerToBeNamed is reset
            if (LayerToBeNamed + 1 == LayerNames.Count) {
                LayerToBeNamed = -1;
            } else {
                LayerToBeNamed++;
            }
        } else {
            _layerText.text = _name;
        }
        // to prevent adding the same keys to LayerVisibility
        if (!LayerVisibility.ContainsKey(_name)) {
            LayerVisibility.Add(_name, true);
        }
        ChangeSelectedLayer();
    }

    private void Update() {
        _name = _layerText.text;
        if (LayerStatus.ContainsKey(_name)
            && LayerStatus[_name]
            && gameObject.GetComponent<Image>().color != Color.black) {
            gameObject.GetComponent<Image>().color = Color.black;
            _layerTrashCan.SetActive(true);
            _layerEdit.SetActive(true);
            _editor.CurrentLayer = LayerIndex[_name];
        } else if (LayerStatus.ContainsKey(_name)
            && !LayerStatus[_name]
            && gameObject.GetComponent<Image>().color != _unselected) {
            gameObject.GetComponent<Image>().color = _unselected;
            _layerTrashCan.SetActive(false);
            _layerEdit.SetActive(false);
        }
    }

    /// <summary>
    /// Changes the layer currently selected from the layer menu.
    /// </summary>
    public static void SelectedChangeSelectedLayer(string layerName) {
        SelectMapObject.UnselectMapObject();
        foreach (string layerKey in new List<string>(LayerStatus.Keys)) {
            if (layerKey != layerName) {
                LayerStatus[layerKey] = false;
            } else {
                LayerStatus[layerKey] = true;
            }
        }  
    }

        
    private void ChangeSelectedLayer() {
        _name = _layerText.text;
        SelectedChangeSelectedLayer(_name);
    }

    /// <summary>
    /// Reactivates the layer input field for the user to supply a new layer name.
    /// </summary>
    private void ChangeLayerName() {
        _layerText.readOnly = false;
        _layerText.ActivateInputField();
    }

    /// <summary>
    /// Toggles visibility for a selected layer.
    /// </summary>
    private void ToggleVisibility() {
        bool isLayerVisible = LayerVisibility[_name];
        LayerVisibility[_name] = !LayerVisibility[_name];
        if (isLayerVisible) {
            _layerVisibility.GetComponent<Image>().sprite = EyeSlash;
        }
        else {
            _layerVisibility.GetComponent<Image>().sprite = Eye;
        }

        // Get the index of the selected layer
        int layerIndex = LayerIndex[_name];

        // Update the visibility of all MapObjects on the selected layer
        foreach (KeyValuePair<int, MapObject> obj in MapEditorManager.Layers[layerIndex]) {
            obj.Value.IsVisible = !obj.Value.IsVisible;
            obj.Value.GameObject.gameObject.GetComponentInChildren<Image>().enabled = !obj.Value.GameObject.gameObject.GetComponentInChildren<Image>().enabled;
        }
    }
}