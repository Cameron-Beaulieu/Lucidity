using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LayerName : MonoBehaviour{
    private TMP_InputField _layerText;
    private string _currentText;

    private void Start() {
       _layerText = gameObject.GetComponent<TMP_InputField>();
       _layerText.onSubmit.AddListener(HandleSubmission);
       _layerText.onDeselect.AddListener(HandleSubmission);
       _layerText.onSelect.AddListener(UpdateCurrentText);
    }

    /// <summary>
    /// Saves the current text before it is changed incase of invalid input.
    /// </summary>
    /// <param name="newName">
    /// <c>string</c> corresponding to the new layer name inputted by the user.
    /// </param>
    public void UpdateCurrentText(string newName){
        _currentText = _layerText.text;
    }

    /// <summary>
    /// Checks for invalid text input and deactivates the input field.
    /// </summary>
    /// <param name="newName">
    /// <c>string</c> corresponding to the new layer name inputted by the user.
    /// </param>
    public void HandleSubmission(string newName){
        if(String.IsNullOrWhiteSpace(newName)){
            _layerText.text = _currentText;
        }
        if(_layerText.GetComponent<RectTransform>().rect.width >= 165){
            _currentText = _layerText.text;
            _layerText.text = _currentText.Substring(0,10) + "...";
        }
        _layerText.readOnly = true;
    }
}