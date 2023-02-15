using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarMovement : MonoBehaviour {

    private float _speed = 100f;
    private float _groundDrag = 5f;
    private float _avatarHeight = 2f;
    public LayerMask GroundLayer;
    private bool _isGrounded;
    public Transform Orientation;
    private float _horizontalInput;
    private float _verticalInput;
    private Vector3 _moveDirection;
    private Rigidbody _rb;
    private float _yRotation;
    private Vector3 _rotationSpeed = new Vector3(0,40f,0);

    void Start() {
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;
    }

    void Update()
    {
        // ground check
        _isGrounded = Physics.Raycast(transform.position, Vector3.down, _avatarHeight * 0.5f + 0.2f, GroundLayer);
        GetInput();
        ControlSpeed();

        // handle drag
        if (_isGrounded) {
            _rb.drag = _groundDrag;
        } else {
            _rb.drag = 0f;
        }
    }

    void FixedUpdate() {
        MoveAvatar();
    }

    void GetInput() {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");
    }

    void MoveAvatar() {
        Vector3 direction = new Vector3(_horizontalInput, 0f, _verticalInput).normalized;
        Orientation.Rotate(Vector3.up * direction.x * _rotationSpeed.y * Time.fixedDeltaTime);
        _rb.MovePosition(transform.position + Orientation.forward * _speed * direction.z * Time.fixedDeltaTime);
    }

    void ControlSpeed() {
        Vector3 flatVelocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);

        // limit velocity if necessary
        if (flatVelocity.magnitude > _speed) {
            Vector3 limitedVelocity = flatVelocity.normalized * _speed; // what max velocity should be
            _rb.velocity = new Vector3(limitedVelocity.x, _rb.velocity.y, limitedVelocity.z);
        }
    }
    
}
