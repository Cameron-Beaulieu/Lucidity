using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LayerName : MonoBehaviour{
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
    public void UpdateCurrentText(string newName){
        CurrentText = newName;
    }

    /// <summary>
    /// Checks for invalid text input and deactivates the input field.
    /// </summary>
    /// <param name="newName">
    /// <c>string</c> corresponding to the new layer name inputted by the user.
    /// </param>
    public void HandleSubmission(string newName){
        _layerText.text = newName;
        if(String.IsNullOrWhiteSpace(newName)){
            _layerText.text = CurrentText;
        } else if(_layerText.GetComponent<RectTransform>().rect.width >= 165){
            string oldName = CurrentText;
            newName = newName.Substring(0,10) + "...";
            // If the layer name is a duplicate, append a number before the ...
            while (Layer.LayerNames.Contains(newName) && !newName.Equals(oldName)) {
                if (!Layer.LayerNames.Contains(newName.Substring(0,10) + 
                    Layer.DuplicateIndex + "...")) {
                    newName = newName.Substring(0,10) + Layer.DuplicateIndex + "..."; 
                } 
                Layer.DuplicateIndex++;
            }
            _layerText.text = newName;
            CurrentText = newName;
            UpdateLayerName(oldName, newName);
        } else {
            string oldName = CurrentText;
            // if the layer name is a duplicate, append a number to the end
            while (Layer.LayerNames.Contains(newName) && !newName.Equals(oldName)) {
                if (!Layer.LayerNames.Contains(newName + Layer.DuplicateIndex)) {
                    newName += Layer.DuplicateIndex;
                    _layerText.text = newName;
                } 
                Layer.DuplicateIndex++;
            }
            CurrentText = newName;
            UpdateLayerName(oldName, newName);
        }
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
        Layer.LayerNames.Remove(oldName);
        Layer.LayerNames.Add(newName);
    }
}