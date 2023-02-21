using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssetOptions : MonoBehaviour {
    [SerializeField] private Slider _brushSizeSlider;
    [SerializeField] private Text _brushSizeText;
    private static float _brushSize;
    [SerializeField] private InputField _countInput;
    private static int _assetCount;

    public static int AssetCount {
        get { return _assetCount; }
        set { _assetCount = value; }
    }

    public static float BrushSize {
        get { return _brushSize; }
        set { _brushSize = value; }
    }

    private void Start() {
        _countInput.onEndEdit.AddListener(delegate{AssetCountInputHandler(_countInput.text);});
        _brushSizeSlider.onValueChanged.AddListener(delegate{BrushSizeSliderHandler();});
        _assetCount = 1;
        BrushSizeSliderHandler();
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
    }

    /// <summary>
    /// Updates <c>_brushSize</c> and corresponding brush size text value based on slider.
    /// </summary>
    public void BrushSizeSliderHandler() {
        _brushSize = _brushSizeSlider.value;
        string sliderMessage = _brushSize + " px";
        _brushSizeText.text = sliderMessage;
    }
}
