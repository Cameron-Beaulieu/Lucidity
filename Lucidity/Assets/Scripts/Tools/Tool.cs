using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tool : MonoBehaviour {
    private MapEditorManager _editor;
    private string _name;
    private Color _unselected = new Color(48/255f, 49/255f, 52/255f);

    void Start() {
        _editor = GameObject.FindGameObjectWithTag("MapEditorManager")
            .GetComponent<MapEditorManager>();
        _name = gameObject.name;
    }

    void Update() {
        if (_editor.ToolStatus.ContainsKey(_name) && _editor.ToolStatus[_name] && gameObject.GetComponent<Image>().color != Color.black) {
            gameObject.GetComponent<Image>().color = Color.black;
        } else if (_editor.ToolStatus.ContainsKey(_name) && !_editor.ToolStatus[_name] && gameObject.GetComponent<Image>().color != _unselected) {
            gameObject.GetComponent<Image>().color = _unselected;
        }
    }

    public void ButtonClicked() {
        if (_name != "Brush Tool" && _editor.ToolStatus["Brush Tool"]) {
            _editor.StopPainting();
        }
        _editor.ChangeTools(_name);
    }
}
