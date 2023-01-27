using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetController : MonoBehaviour {
    public int Id;
    public bool Clicked = false;
    private MapEditorManager _editor;

    void Start() {
        _editor = GameObject.FindGameObjectWithTag("MapEditorManager").GetComponent<MapEditorManager>();
    }

    public void ButtonClicked() {
        Clicked = true;
        _editor.CurrentButtonPressed = Id;
    }
}
