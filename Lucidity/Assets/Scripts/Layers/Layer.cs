using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Layer : MonoBehaviour{
    public static Dictionary<string, bool> LayerStatus = new Dictionary<string, bool>();
    public static Dictionary<string, int> LayerIndex = new Dictionary<string, int>();
    public static List<string> LayerNames = new List<string>();
    private static GameObject _layerContainer;
    private GameObject _layerTrashCan;
    private TMP_InputField _layerText;
    private GameObject _layerEdit;
    private MapEditorManager _editor;
    private string _name;
    private Color _unselected = new Color(48/255f, 49/255f, 52/255f);

    private void Start() {
        _layerContainer = GameObject.Find("LayerScrollContent");
        _editor = GameObject.FindGameObjectWithTag("MapEditorManager")
            .GetComponent<MapEditorManager>();
        gameObject.name = "Layer" + (LayerStatus.Count).ToString();
        _name = gameObject.name;
        _layerText = gameObject.transform.GetChild(0).gameObject.GetComponent<TMP_InputField>();
        _layerText.text = _name;
        _layerText.readOnly = true;
        _layerTrashCan = gameObject.transform.GetChild(2).gameObject;
        _layerTrashCan.SetActive(false);
        _layerEdit = gameObject.transform.GetChild(3).gameObject;
        _layerEdit.GetComponent<Button>().onClick.AddListener(ChangeLayerName);
        gameObject.GetComponent<Button>().onClick.AddListener(ChangeSelectedLayer);
        LayerStatus.Add(_name, false);
        LayerIndex.Add(_name, LayerIndex.Count);
        LayerNames.Add(_name);
        ChangeSelectedLayer();
    }

    private void Update() {
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
    private void ChangeSelectedLayer(){
        foreach (string layerKey in new List<string>(LayerStatus.Keys)) {
            if (layerKey != _name) {
                LayerStatus[layerKey] = false;
            } else {
                LayerStatus[layerKey] = true;
            }
        }
    }

    /// <summary>
    /// Reactivates the layer input field for the user to supply a new layer name.
    /// </summary>
    private void ChangeLayerName(){
        _layerText.readOnly = false;
        _layerText.ActivateInputField();
    }

}