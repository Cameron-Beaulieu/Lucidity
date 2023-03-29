using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssetOptions : MonoBehaviour {
    private MapEditorManager _editor;
    [SerializeField] private Slider _spreadSlider;
    [SerializeField] private Text _spreadText;
    private static float _spread;
    [SerializeField] private InputField _countInput;
    private static int _assetCount;
    [SerializeField] private Toggle _randomToggle;
    private static bool _random;

    public static int AssetCount {
        get { return _assetCount; }
        set { _assetCount = value; }
    }

    public static float Spread {
        get { return _spread; }
        set { _spread = value; }
    }

    public static bool Random {
        get { return _random; }
        set { _random = value; }
    }

    void Start() {
        _editor = GameObject.FindGameObjectWithTag("MapEditorManager")
            .GetComponent<MapEditorManager>();
        _countInput.onEndEdit.AddListener(delegate{ AssetCountInputHandler(_countInput.text); });
        _spreadSlider.onValueChanged.AddListener(delegate{ SpreadSliderHandler(); });
        _randomToggle.onValueChanged.AddListener(delegate{ RandomToggleHandler(); });
        AssetCountInputHandler(_countInput.text);
        SpreadSliderHandler();
        RandomToggleHandler();
    }

    /// <summary>
    /// Parses and updates <c>Count</c> and corresponding asset count text value based on asset
    /// count input.
    /// </summary>
    /// <param name="input">
    /// <c>string</c> corresponding to provided user input.
    /// </param>
    public void AssetCountInputHandler(string input) {
        _assetCount = int.Parse(input);
        if (_assetCount < 0) {   // Restrict input to only be positive
            _assetCount *= -1;
            _countInput.text = _assetCount.ToString();
        }
        DynamicBoundingBox.DynamicSideLength = (int)Mathf.Ceil(Mathf.Sqrt(_assetCount));
        UpdateAssetImage();
    }

    /// <summary>
    /// Updates <c>_spread</c> and corresponding spread text value based on slider.
    /// </summary>
    public void SpreadSliderHandler() {
        _spread = 1 + (_spreadSlider.value) / 10;
        string sliderMessage = _spread.ToString("0.0") + " x";
        _spreadText.text = sliderMessage;
        UpdateAssetImage();
    }

    /// <summary>
    /// Updates <c>_random<c> and the asset image to match the setting.
    /// </summary>
    public void RandomToggleHandler() {
        _random = _randomToggle.isOn;
        UpdateAssetImage();
    }

    /// <summary>
    /// If an asset is selected, (re)generate an appropriate hover asset image.
    /// </summary>
    public void UpdateAssetImage() {
        if (_editor.AssetButtons[MapEditorManager.CurrentButtonPressed].Clicked
                && _editor.AssetPrefabs[MapEditorManager.CurrentButtonPressed] != null) {
            GameObject activeImage = GameObject.FindGameObjectWithTag("AssetImage");
            // if there is an Image being shown on hover already, destroy it
            if (activeImage != null) {
                Destroy(activeImage);
            }
            DynamicBoundingBox.CreateDynamicAssetImage(
                _editor.AssetImage[MapEditorManager.CurrentButtonPressed],
                Mouse.GetMousePosition());
        }
    }
}