using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetController : MonoBehaviour {
    public int Id;
    public bool Clicked = false;
    private MapEditorManager Editor;

    void Start() {
        GameObject MapEditorManager = GameObject.FindGameObjectWithTag("MapEditorManager");
        Editor = MapEditorManager.GetComponent<MapEditorManager>();
    }

    public void ButtonClicked() {
        Clicked = true;
        Editor.CurrentButtonPressed = Id;
    }
}
