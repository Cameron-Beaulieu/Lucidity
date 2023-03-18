using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveCamera : MonoBehaviour {

    public Transform CameraPosition;
    public Transform Orientation;
    public Transform AvatarBody;
    private Vector2 _rotation;
    private float _sensitivity;
    [SerializeField] private Slider _sensitivitySlider;
    [SerializeField] private Text _sensitivityText;

    private void Start() {
        _sensitivitySlider.onValueChanged.AddListener(delegate{ SensitivitySliderHandler(); });
        SensitivitySliderHandler();
    }

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
        Vector2 mouse = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"))
                        * _sensitivity * Time.deltaTime;
        _rotation += new Vector2(-1f * mouse.y, mouse.x);
        _rotation.x = Mathf.Clamp(_rotation.x, -90f, 90f);

        CameraPosition.rotation = Quaternion.Euler(_rotation.x, _rotation.y, 0);
        Orientation.rotation = Quaternion.Euler(0, _rotation.y, 0);

        transform.position = CameraPosition.position;
        transform.rotation = CameraPosition.rotation;
        AvatarBody.rotation = Orientation.rotation;
    }

    /// <summary>
    /// Updates <c>_sensitivity</c> and corresponding sensitivity text value based on slider.
    /// </summary>
    public void SensitivitySliderHandler() {
        _sensitivity = (_sensitivitySlider.value / 10) * 100;
        string sliderMessage = (_sensitivity / 100f).ToString("0.0") + " x";
        _sensitivityText.text = sliderMessage;
    }
}
