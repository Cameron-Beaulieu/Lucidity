using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarMovement : MonoBehaviour {

    public LayerMask GroundLayer;
    public Transform Orientation;
    public static bool IsTesting = false;
    public static float HorizontalTestingInput;
    public static float VerticalTestingInput;
    public static bool AscendTestingInput;
    public static bool DescendTestingInput;
    private float _avatarHeight;
    private float _groundDrag = 5f;
    private float _airDrag = 0.5f;
    private bool _isGrounded;
    private float _speed;
    private float _horizontalInput;
    private float _verticalInput;
    private bool _ascendInput;
    private bool _descendInput;
    private bool _noclip;
    private GameObject _map;
    private Rigidbody _rb;
    [SerializeField] private Slider _speedSlider;
    [SerializeField] private Text _speedText;
    [SerializeField] private Toggle _noclipToggle;

    public float Speed {
        get { return _speed; }
        set { _speed = value; }
    }

    public bool Noclip {
        get { return _noclip; }
        set { _noclip = value; }
    }

    private void Start() {
        _map = GameObject.FindGameObjectWithTag("3DMap");
        
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;

        _avatarHeight = transform.localScale.y;

        _speedSlider.onValueChanged.AddListener(delegate{ SpeedSliderHandler(); });
        _speedSlider.value = PlayerPrefs.GetFloat("speed", 1f) * 10f;
        SpeedSliderHandler();

        _noclipToggle.isOn = PlayerPrefs.GetInt("noclip", 0) == 1;
        NoclipToggleHandler();
    }

    private void Update() {
        // verify that the avatar is in bounds; if it is out of bounds, force its position to stay
        // within the bounds of the map
        Vector3 positionInsideBounds = transform.position;
        Vector3 mapSize = new Vector3(_map.transform.localScale.x, 0, _map.transform.localScale.z);
        if (Mathf.Abs(transform.position.x) > (5 * mapSize.x)) {
            positionInsideBounds.x = Mathf.Sign(transform.position.x) * (5 * mapSize.x);
        }
        if (transform.position.y < _avatarHeight) {
            positionInsideBounds.y = _avatarHeight;
        }
        if (Mathf.Abs(transform.position.z) > (5 * mapSize.z)) {
            positionInsideBounds.z = Mathf.Sign(transform.position.z) * (5 * mapSize.z);
        }
        transform.SetPositionAndRotation(positionInsideBounds, Quaternion.identity);

        // check if object is grounded
        _isGrounded = Physics.Raycast(transform.position, Vector3.down, 
                                      _avatarHeight + 0.2f, GroundLayer);
        GetInput();

        // prevents the avatar from slipping around
        if (_isGrounded && !_noclip) {
            _rb.drag = _groundDrag;
        } else {
            _rb.drag = _airDrag;
        }
    }

    private void FixedUpdate() {
        MoveAvatar();
    }

    /// <summary>
    /// Updates <c>_speed</c> and corresponding speed text value based on slider.
    /// </summary>
    public void SpeedSliderHandler() {
        _speed = _speedSlider.value * 10;
        string sliderMessage = (_speed / 100f).ToString("0.0") + " x";
        _speedText.text = sliderMessage;
    }

    /// <summary>
    /// Updates <c>_noclip</c> based on toggle.
    /// </summary>
    public void NoclipToggleHandler() {
        _noclip = _noclipToggle.isOn;
        GameObject.Find("AvatarBody").GetComponent<CapsuleCollider>().enabled = !_noclip;
        SetGravity(!_noclip);
    }

    /// <summary>
    /// Sets the gravity on or off based on user-provided boolean value.
    /// </summary>
    /// <param name="active">
    /// <c>bool</c> corresponding to whether gravity should be set on or off
    /// </param>
    public void SetGravity(bool active) {
        if (active) {
            Physics.gravity = new Vector3(0, -1000f, 0);
        } else {
            Physics.gravity = Vector3.zero;
            _rb.velocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
        }
    }

    /// <summary>
    /// Gets the user's input.
    /// </summary>
    private void GetInput() {
        if (IsTesting) {
            _horizontalInput = HorizontalTestingInput;
            _verticalInput = VerticalTestingInput;
            _ascendInput = AscendTestingInput;
            _descendInput = DescendTestingInput;
        } else {
            _horizontalInput = Input.GetAxisRaw("Horizontal");
            _verticalInput = Input.GetAxisRaw("Vertical");
            _ascendInput = Input.GetKey(KeyCode.Space);
            _descendInput = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        }
    }

    /// <summary>
    /// Moves and rotates the Avatar in the direction of the user's input.
    /// </summary>
    private void MoveAvatar() {
        if (_ascendInput && _noclip) {
            Ascend();
        } else if (_descendInput && _noclip) {
            Descend();
        }

        Vector3 direction = new Vector3(_horizontalInput, 0f, _verticalInput).normalized;
        _rb.velocity = (Orientation.forward * direction.z + Orientation.right * direction.x)
                        * _speed;
    }

    /// <summary>
    /// Applies a force upwards, allowing the player to ascend.
    /// </summary>
    private void Ascend() {
        _rb.velocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
        _rb.AddForce(Orientation.up * _speed, ForceMode.Impulse);
    }

    /// <summary>
    /// Applies a force downwards, allowing the player to descend.
    /// </summary>
    private void Descend() {
        _rb.velocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
        _rb.AddForce(-1f * Orientation.up * _speed, ForceMode.Impulse);
    }

    private void OnCollisionEnter() {
        // Stop moving Avatar when it collides with an object
        _rb.velocity = Vector3.zero;
    }
}
