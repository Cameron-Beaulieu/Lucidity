using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderTextValueUpdate : MonoBehaviour
{
    public Slider Slider;
    public TextMeshProUGUI SliderText;
    public SelectMapObject MapObject;


    // Start is called before the first frame update
    private void Start() {
        // TODO: Needs to fix this so it changes dynamically
        // when you click on a SelectMapObject Object
        Slider.value = 1.0f;
    }

    void Update() {
        SliderText.text = Slider.value.ToString("0.0" + "x");
    }
}
