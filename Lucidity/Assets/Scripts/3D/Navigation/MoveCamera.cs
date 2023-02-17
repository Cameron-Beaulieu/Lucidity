using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour {

    public Transform CameraPosition;
    public Transform Orientation;

    // Update is called once per frame
    void Update() {
        transform.position = CameraPosition.position;
        transform.rotation = Orientation.rotation;
    }
}
