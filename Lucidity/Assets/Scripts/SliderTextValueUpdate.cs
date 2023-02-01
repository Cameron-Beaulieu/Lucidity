using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderTextValueUpdate : MonoBehaviour
{
    public Slider _slider;
    public TextMeshProUGUI _sliderText;


    // Start is called before the first frame update
    private void Start() {
        
    }

    void Update() {
        _sliderText.text = _slider.value.ToString("0.0" + "x");
    }
}
