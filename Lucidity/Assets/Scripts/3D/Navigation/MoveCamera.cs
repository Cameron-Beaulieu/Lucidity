using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour {

    public Transform CameraPosition;
    public Transform Orientation;
    public Transform AvatarBody;
    private Vector2 Sensitivity = new Vector2(200f, 200f);
    public Vector2 Rotation;

    private void OnEnable() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDisable() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Update is called once per frame
    private void Update() {
        Vector2 mouse = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * Sensitivity * Time.deltaTime;
        Rotation += new Vector2(-1f * mouse.y, mouse.x);
        Rotation.x = Mathf.Clamp(Rotation.x, -90f, 90f);

        CameraPosition.rotation = Quaternion.Euler(Rotation.x, Rotation.y, 0);
        Orientation.rotation = Quaternion.Euler(0, Rotation.y, 0);

        transform.position = CameraPosition.position;
        transform.rotation = CameraPosition.rotation;
        AvatarBody.rotation = Orientation.rotation;
    }
}
