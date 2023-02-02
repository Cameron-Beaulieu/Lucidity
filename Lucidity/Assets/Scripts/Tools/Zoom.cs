using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zoom : MonoBehaviour
{
    private MapEditorManager _editor;

    void Start()
    {
        _editor = GameObject.FindGameObjectWithTag("MapEditorManager")
            .GetComponent<MapEditorManager>();
    }

    void OnMouseDown () {
        if (Input.GetKey ("mouse 0")) {
            if (_editor.ToolStatus["Zoom In"] && gameObject.transform.localScale.x < 5) {
                gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x + 1f, gameObject.transform.localScale.y + 1f, gameObject.transform.localScale.z + 1f);
            } else if (_editor.ToolStatus["Zoom Out"] && gameObject.transform.localScale.x > 1) {
                gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x - 1f, gameObject.transform.localScale.y - 1f, gameObject.transform.localScale.z - 1f);
            }
        }
    }
}

// TODO: need to add scale factor to asset placement if zoomed in
