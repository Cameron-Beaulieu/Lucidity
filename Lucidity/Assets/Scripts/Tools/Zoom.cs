using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zoom : MonoBehaviour
{
    private MapEditorManager _editor;

    // Start is called before the first frame update
    void Start()
    {
        _editor = GameObject.FindGameObjectWithTag("MapEditorManager")
            .GetComponent<MapEditorManager>();
    }

    void OnClick() {
        Debug.Log("Click");
        if (_editor.ToolStatus["Zoom In"]) {
            Debug.Log("Zooming in");
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z - 50);
        } else if (_editor.ToolStatus["Zoom Out"]) {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z + 50);
        }
    }
}
