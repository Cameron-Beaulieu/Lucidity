using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarMovement : MonoBehaviour {

    public LayerMask GroundLayer;
    public Transform Orientation;
    private float _avatarHeight = 2f;
    private float _groundDrag = 5f;
    private bool _isGrounded;
    private float _speed = 100f;
    private float _rotationSpeed = 40f;
    private float _horizontalInput;
    private float _verticalInput;
    private Rigidbody _rb;

    private void Start() {
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;
    }

    private void Update() {
        // check if object is grounded
        _isGrounded = Physics.Raycast(transform.position, Vector3.down, 
                                    _avatarHeight * 0.5f + 0.2f, GroundLayer);
        GetInput();

        // prevents the avatar from slipping around
        if (_isGrounded) {
            _rb.drag = _groundDrag;
        } else {
            _rb.drag = 0f;
        }
    }

    private void FixedUpdate() {
        MoveAvatar();
    }

    /// <summary>
    /// Gets the user's input.
    /// </summary>
    private void GetInput() {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");
    }

    /// <summary>
    /// Moves and rotates the Avatar in the direction of the user's input.
    /// </summary>
    private void MoveAvatar() {
        Vector3 direction = new Vector3(_horizontalInput, 0f, _verticalInput).normalized;
        Orientation.Rotate(Vector3.up * direction.x * _rotationSpeed * Time.fixedDeltaTime);
        _rb.velocity = Orientation.forward * direction.z * _speed;
    }

    private void OnCollisionEnter() {
        // Stop moving Avatar when it collides with an object
        _rb.velocity = Vector3.zero;
    }
    
}
