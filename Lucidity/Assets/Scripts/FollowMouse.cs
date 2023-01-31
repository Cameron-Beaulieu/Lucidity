using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using RaycastingLibrary;

public class FollowMouse : MonoBehaviour {
  private int _uiLayer = 5;

    // Update is called once per frame
    void Update() {
      Vector2 worldPosition = MapEditorManager.getMousePosition();
      transform.position = new Vector3(worldPosition.x, worldPosition.y, 90f);
      RayLibrary rayLib = new RayLibrary();
      if (rayLib.IsPointerOverLayer(_uiLayer)){
        gameObject.GetComponent<MeshRenderer>().enabled = false;
      }
      else if (!rayLib.IsPointerOverLayer(_uiLayer)) {
        gameObject.GetComponent<MeshRenderer>().enabled = true;
      }
    }
}
