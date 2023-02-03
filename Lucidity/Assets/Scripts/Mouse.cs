using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Mouse : MonoBehaviour {
	private int _uiLayer = 5;

	void Update() {
		Vector2 worldPosition = getMousePosition();
		transform.position = new Vector3(worldPosition.x, worldPosition.y, 90f);
		RaycastingLibrary.RayLibrary rayLib = new RaycastingLibrary.RayLibrary();
		if (rayLib.IsPointerOverLayer(_uiLayer)) {
			gameObject.GetComponent<MeshRenderer>().enabled = false;
		}
		else {
			gameObject.GetComponent<MeshRenderer>().enabled = true;
		}
	}

	/// <summary>
	/// Static method to retrieve current mouse position. relative to world point.
	/// </summary>
	/// <returns>
	/// 2D vector corresponding to the position of the mouse cursor.
	/// </returns>
	public static Vector2 getMousePosition() {
		Vector2 screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		return Camera.main.ScreenToWorldPoint(screenPosition);
	}
}
