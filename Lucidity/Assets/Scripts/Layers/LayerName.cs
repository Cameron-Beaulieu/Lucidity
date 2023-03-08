using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class LayerName : MonoBehaviour {
    private TMP_InputField _layerText;
    public string CurrentText;

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
    public void UpdateCurrentText(string newName) {
        CurrentText = newName;
    }

    /// <summary>
    /// Checks for invalid text input and deactivates the input field.
    /// </summary>
    /// <param name="newName">
    /// <c>string</c> corresponding to the new layer name inputted by the user.
    /// </param>
    public void HandleSubmission(string newName) {
        _layerText.text = newName;
        if (String.IsNullOrWhiteSpace(newName)) {
            _layerText.text = CurrentText;
        } else if (_layerText.GetComponent<RectTransform>().rect.width >= 165) {
            CurrentText = newName;
            _layerText.text = CurrentText.Substring(0,10) + "...";
        } else {
            CurrentText = newName;
        }
        _layerText.readOnly = true;
    }
}