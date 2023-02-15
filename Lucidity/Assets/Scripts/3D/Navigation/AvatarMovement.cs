using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarMovement : MonoBehaviour {

    [Header("Movement")]
    private float _speed = 5f;

    [Header("Ground Check")]
    private float _groundDrag = 5f;
    private float _avatarHeight = 2f;
    public LayerMask WhatIsGround;
    private bool _isGrounded;


    public Transform Orientation;
    private float _horizontalInput;
    private float _verticalInput;
    private Vector3 _moveDirection;
    private Rigidbody _rb;

    void Start() {
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;
    }

    void Update()
    {
        // ground check
        _isGrounded = Physics.Raycast(transform.position, Vector3.down, _avatarHeight * 0.5f + 0.2f, WhatIsGround);
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
        _moveDirection = (Orientation.forward * _verticalInput) + (Orientation.right * _horizontalInput);
        _rb.AddForce(_moveDirection.normalized * _speed * 10f, ForceMode.Force);
        // _rb.MovePosition(transform.position + _moveDirection * _speed * Time.deltaTime);
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
