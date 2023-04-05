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
    [SerializeField] private InputField _variationInput;
    [SerializeField] private Text _variationMaximumText;
    private static int _variation;

    public static int AssetCount {
        get { return _assetCount; }
        set { _assetCount = value; }
    }

    public static float Spread {
        get { return _spread; }
        set { _spread = value; }
    }

    public static int Variation {
        get { return _variation; }
        set { _variation = value; }
    }

    private void Awake() {
        _editor = gameObject.GetComponent<MapEditorManager>();
    }

    private void Start() {
        _countInput.onEndEdit.AddListener(delegate{ AssetCountInputHandler(_countInput.text); });
        _spreadSlider.onValueChanged.AddListener(delegate{ SpreadSliderHandler(); });
        _variationInput.onEndEdit.AddListener(delegate{ VariationInputHandler(_variationInput.text); });
        AssetCountInputHandler(_countInput.text);
        SpreadSliderHandler();
        VariationInputHandler(_variationInput.text);
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
        if (_assetCount <= 0) {   // Restrict input to only be positive
            _assetCount = 1;
            _countInput.text = _assetCount.ToString();
        }
        if (_assetCount > 9) {  // Restrict input to be a maximum of 9
            _assetCount = 9;
            _countInput.text = _assetCount.ToString();
        }
        DynamicBoundingBox.DynamicSideLength = (int)Mathf.Ceil(Mathf.Sqrt(_assetCount));
        VariationInputHandler("1");    // Change variation number if needed
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
    /// Parses and updates <c>Variation</c> and corresponding hover variation text value based on
    /// variation input.
    /// </summary>
    /// <param name="input">
    /// <c>string</c> corresponding to provided user input.
    /// </param>
    public void VariationInputHandler(string input) {
        if (_variation == int.Parse(input) - 1) {
            return;
        }
        _variation = int.Parse(input) - 1;
        // Restrict input to be the maximum number of variations
        if (_variation > DynamicBoundingBox.AssetArrangements.Count - 1) {
            _variation = DynamicBoundingBox.AssetArrangements.Count - 1;
        }
        if (_variation < 0) {   // Restrict input to only be positive
            _variation = 0;
        }
        _variationInput.text = (_variation + 1).ToString();
        UpdateAssetImage();
    }

    /// <summary>
    /// If an asset is selected, (re)generate an appropriate hover asset image.
    /// </summary>
    public void UpdateAssetImage() {
        if (_editor.AssetButtons[MapEditorManager.CurrentButtonPressed].Clicked
                && _editor.AssetPrefabs[MapEditorManager.CurrentButtonPressed] != null) {
            GameObject[] activeImages = GameObject.FindGameObjectsWithTag("AssetImage");
            // if there is an Image being shown on hover already, destroy it
            if (activeImages != null) {
                foreach (GameObject image in activeImages) {
                    Destroy(image);
                }
            }
            DynamicBoundingBox.CreateDynamicAssetImage(
                _editor.AssetImage[MapEditorManager.CurrentButtonPressed],
                Mouse.GetMousePosition());
        }
    }
}