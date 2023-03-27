using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LayerName : MonoBehaviour {
    public TMP_InputField LayerText;
    public string CurrentText;
    public static bool IsTesting = false;

    private void Awake() {
       LayerText = gameObject.GetComponent<TMP_InputField>();
       LayerText.onSubmit.AddListener(HandleSubmission);
       LayerText.onDeselect.AddListener(HandleSubmission);
       LayerText.onSelect.AddListener(UpdateCurrentText);
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
        if (IsTesting) { CurrentText = LayerText.text; }
        LayerText.text = newName;
        string oldName = CurrentText;
        int duplicateIndex = 2;
        if(String.IsNullOrWhiteSpace(newName)) {
            LayerText.text = CurrentText;
            LayerText.readOnly = true;
            return;
        } else if(LayerText.GetComponent<RectTransform>().rect.width >= 165) {
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
        LayerText.text = newName;
        CurrentText = newName;
        UpdateLayerName(oldName, newName);
        LayerText.readOnly = true;
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
    public void UpdateLayerName(string oldName, string newName) {
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
        Layer.LayerNames.Insert(oldIndex, newName);

        GameObject layer = GameObject.Find(oldName);
        layer.name = newName;
    }
}