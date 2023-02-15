using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarCam : MonoBehaviour {

    public float XSensitivity;
    public float ZSensitivity;
    public Transform Orientation;
    private float _xRotation;
    private float _zRotation;

    void Start() {
        // Cursor.lockState = CursorLockMode.Locked;

    }

    void Update() {
    }
}
