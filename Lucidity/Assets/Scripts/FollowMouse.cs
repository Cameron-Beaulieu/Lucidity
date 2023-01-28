using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        Vector2 worldPosition = MapEditorManager.getMousePosition();
        transform.position = worldPosition;
    }
}
