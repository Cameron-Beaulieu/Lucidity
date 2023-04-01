using RaycastingLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zoom : MonoBehaviour
{
    public static float zoomFactor;
    public static bool IsTesting = false;
    private MapEditorManager _editor;
    private RayLibrary _rayLib;
    private int _uiLayer = 5;
    private float zoomIncrement = 0.25f;

    private void Start() {
        _editor = GameObject.FindGameObjectWithTag("MapEditorManager")
            .GetComponent<MapEditorManager>();
        zoomFactor = 0f;
        _rayLib = new RayLibrary();
    }

    public void OnMouseDown () {
        if (IsTesting || (Input.GetMouseButtonDown(0) && !_rayLib.IsPointerOverLayer(_uiLayer))) {
            if (Tool.ToolStatus["Zoom In"] && gameObject.transform.localScale.x < 3) {
                gameObject.transform.localScale = 
                    new Vector3(gameObject.transform.localScale.x + zoomIncrement, 
                                gameObject.transform.localScale.y + zoomIncrement, 
                                gameObject.transform.localScale.z + zoomIncrement);
                zoomFactor += zoomIncrement;
            } else if (Tool.ToolStatus["Zoom Out"] && gameObject.transform.localScale.x > 0.25) {
                gameObject.transform.localScale =
                    new Vector3(gameObject.transform.localScale.x - zoomIncrement, 
                                gameObject.transform.localScale.y - zoomIncrement, 
                                gameObject.transform.localScale.z - zoomIncrement);
                zoomFactor -= zoomIncrement;
            }
        }
    }
}
