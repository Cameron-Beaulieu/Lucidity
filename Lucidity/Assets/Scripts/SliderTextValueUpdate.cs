using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderTextValueUpdate : MonoBehaviour
{
    public Slider _slider;
    public TextMeshProUGUI _sliderText;
    public SelectMapObject MapObject;


    // Start is called before the first frame update
    private void Start() {
        // TODO: Needs to fix this so it changes dynamically
        // when you click on a SelectMapObject Object
        _slider.value = 1.0f;
    }

    void Update() {
        _sliderText.text = _slider.value.ToString("0.0" + "x");
    }
}
