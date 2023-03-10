using RaycastingLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Mouse : MonoBehaviour {
    private static Vector2 _lastMousePosition;
    private int _uiLayer = 5;

    public static Vector2 LastMousePosition {
        get { return _lastMousePosition; }
        set { _lastMousePosition = value; }
    }

    private void Update() {
        Vector2 worldPosition = GetMousePosition();
        transform.position = new Vector3(worldPosition.x, worldPosition.y, 0f);
        RayLibrary rayLib = new RayLibrary();
        if (gameObject.GetComponent<SpriteRenderer>() != null) {
            if (rayLib.IsPointerOverLayer(_uiLayer)) {
                gameObject.GetComponent<SpriteRenderer>().enabled = false;
                foreach (Transform child in transform) {
                    child.gameObject.GetComponent<SpriteRenderer>().enabled = false;
                }
            } else {
                gameObject.GetComponent<SpriteRenderer>().enabled = true;
                foreach (Transform child in transform) {
                    child.gameObject.GetComponent<SpriteRenderer>().enabled = true;
                }
            }
        }
    }

    /// <summary>
    /// Static method to retrieve current mouse position. relative to world point.
    /// </summary>
    /// <returns>
    /// <c>Vector2</c> corresponding to the position of the mouse cursor.
    /// </returns>
    public static Vector2 GetMousePosition() {
        Vector2 screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        return Camera.main.ScreenToWorldPoint(screenPosition);
    }
}
