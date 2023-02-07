using RaycastingLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zoom : MonoBehaviour
{
    private MapEditorManager _editor;
    public static float zoomFactor;
    private RayLibrary _rayLib;
    private int _uiLayer = 5;
    private float zoomIncrement = 0.5f;

    void Start()
    {
        _editor = GameObject.FindGameObjectWithTag("MapEditorManager")
            .GetComponent<MapEditorManager>();
        zoomFactor = 0f;
        _rayLib = new RayLibrary();
    }

    void OnMouseDown () {
        if (Input.GetMouseButtonDown(0) && !_rayLib.IsPointerOverLayer(_uiLayer)) {
            if (Tool.ToolStatus["Zoom In"] && gameObject.transform.localScale.x < 3) {
                gameObject.transform.localScale = 
                    new Vector3(gameObject.transform.localScale.x + zoomIncrement, 
                        gameObject.transform.localScale.y + zoomIncrement, 
                            gameObject.transform.localScale.z + zoomIncrement);
                zoomFactor += zoomIncrement;
            } else if (Tool.ToolStatus["Zoom Out"] && gameObject.transform.localScale.x > 1) {
                gameObject.transform.localScale =
                    new Vector3(gameObject.transform.localScale.x - zoomIncrement, 
                        gameObject.transform.localScale.y - zoomIncrement, 
                            gameObject.transform.localScale.z - zoomIncrement);
                zoomFactor -= zoomIncrement;
            }
        }
    }
}

// TODO: need to add scale factor to asset placement if zoomed in
