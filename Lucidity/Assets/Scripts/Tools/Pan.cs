using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pan : MonoBehaviour {
    public Texture2D PanCursor;
    public Texture2D PanCursorMouseDown;
    private static MapEditorManager _editor;
    private static bool _isDragging = false;
    private static Vector3 _offset;

    private void Start() {
        _editor = GameObject.FindGameObjectWithTag("MapEditorManager")
            .GetComponent<MapEditorManager>();
    }
    
    private void Update() {
        if (_isDragging && Tool.ToolStatus["Panning Tool"]) {
            Vector2 mousePosition = Input.mousePosition;
            gameObject.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) 
                + _offset;
        }
    }

    public void OnMouseDown() {
        if (Tool.ToolStatus["Panning Tool"]) {
            if (PanCursorMouseDown != null) {
                Cursor.SetCursor(PanCursorMouseDown, new Vector2(16f,16f), CursorMode.Auto);
            }
            _offset = gameObject.transform.position - 
                Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _isDragging = true;
        }
    }

    public void OnMouseUp() {
        if (Tool.ToolStatus["Panning Tool"]) {
            if (PanCursor != null) {
                Cursor.SetCursor(PanCursor, new Vector2(16f,16f), CursorMode.Auto);
            }
            _isDragging = false;
        }
    }
}
