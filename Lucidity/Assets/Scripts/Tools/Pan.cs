using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pan : MonoBehaviour {
    private MapEditorManager _editor;
    private bool _isDragging = false;
    private Vector3 _offset;

    void Start() {
        _editor = GameObject.FindGameObjectWithTag("MapEditorManager")
            .GetComponent<MapEditorManager>();
    }
    
    void Update() {
        if (_isDragging && Tool.ToolStatus["Panning Tool"]) {
            Vector2 mousePosition = Input.mousePosition;
            gameObject.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) 
                + _offset;
        }

    }

    void OnMouseDown() {
        if (Tool.ToolStatus["Panning Tool"]) {
            _offset = gameObject.transform.position - 
                Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _isDragging = true;
        }
    }

    void OnMouseUp() {
        if (Tool.ToolStatus["Panning Tool"]) {
            _isDragging = false;
        }
    }
}
