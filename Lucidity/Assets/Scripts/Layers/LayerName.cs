using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LayerName : MonoBehaviour {
    private TMP_InputField _layerText;
    public string CurrentText;
    public static bool IsTesting = false;

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
        if (IsTesting) { CurrentText = _layerText.text; }
        _layerText.text = newName;
        string oldName = CurrentText;
        int duplicateIndex = 2;
        if(String.IsNullOrWhiteSpace(newName)) {
            _layerText.text = CurrentText;
            _layerText.readOnly = true;
            return;
        } else if(_layerText.GetComponent<RectTransform>().rect.width >= 165) {
            newName = newName.Substring(0,10) + "...";
            while (Layer.LayerNames.Contains(newName) && !newName.Equals(oldName)) {
                if (duplicateIndex == 2) {
                    newName = newName.Substring(0, newName.Length - 4) + duplicateIndex + "...";
                } else {
                    newName = newName.Substring(0, newName.Length - (int) Math.Floor(
                        Math.Log10(duplicateIndex - 1) + 4)) + duplicateIndex + "...";
                }
                duplicateIndex++;
            }
        } else {
            // if the layer name is a duplicate, append a number to the end
            while (Layer.LayerNames.Contains(newName) && !newName.Equals(oldName)) {
                if (duplicateIndex == 2) {
                    newName += duplicateIndex;
                } else {
                    // The math part is to handle the case where duplicateIndex is >1 digit
                    newName = newName.Substring(0, newName.Length - 
                        (int) Math.Floor(Math.Log10(duplicateIndex - 1) + 1)) + duplicateIndex;
                }
                duplicateIndex++;
            }
        }
        _layerText.text = newName;
        CurrentText = newName;
        UpdateLayerName(oldName, newName);
        _layerText.readOnly = true;
    }

    /// <summary>
    /// Updates the layer name in the <c>Layer</c> class fields: <c>LayerIndex</c>, 
    /// <c>LayerStatus</c>, and <c>LayerNames</c>.
    /// <param name="oldName">
    /// <c>string</c> corresponding to the previous name of the layer.
    /// </param>
    /// <param name="newName">
    /// <c>string</c> corresponding to the new layer name inputted by the user.
    /// </param>
    /// </summary>
    private void UpdateLayerName(string oldName, string newName) {
        if (newName.Equals(oldName)) {return;}
        int oldIndex = Layer.LayerIndex[oldName];
        bool oldStatus = Layer.LayerStatus[oldName];
        // Update LayerIndex dictionary to use the newName
        Layer.LayerIndex.Remove(oldName);
        Layer.LayerIndex.Add(newName, oldIndex);
        // Update LayerStatus to use the NewName
        Layer.LayerStatus.Remove(oldName);
        Layer.LayerStatus.Add(newName, oldStatus);
        // Update LayerNames
        int index = Layer.LayerNames.FindIndex(s => s == oldName);
        Layer.LayerNames[index] = newName;
    }
}