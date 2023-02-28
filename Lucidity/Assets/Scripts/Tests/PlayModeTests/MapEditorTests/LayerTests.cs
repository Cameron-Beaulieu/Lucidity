using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

[TestFixture]
public class LayerTests : MapEditorTests {
    [Test]
    public void EmptyMapHasOneLayer() {
        Assert.AreEqual("MapEditor", SceneManager.GetActiveScene().name);
        GameObject layerScrollContent = GameObject.Find("LayerScrollContent");
        Assert.AreEqual(layerScrollContent.transform.childCount, 1);
    }

    [Test]
    public void CanCollapseAndExpandLayersMenu() {
        GameObject menuBody = GameObject.Find("Layers Body");
        Assert.IsTrue(menuBody.activeSelf);
        Assert.IsNotNull(GameObject.Find("Layer Tool")); // layer tool exists only on expanded menu
        Button collapseButton = GameObject.Find("Layers Header (Expanded)").GetComponent<Button>();
        collapseButton.onClick.Invoke();
        Assert.IsFalse(menuBody.activeSelf);
        Assert.IsFalse(collapseButton.gameObject.activeSelf);
        Assert.IsNull(GameObject.Find("Layer Tool"));
        Button expandButton = GameObject.Find("Layers Header (Collapsed)").GetComponent<Button>();
        expandButton.onClick.Invoke();
        Assert.IsTrue(menuBody.activeSelf);
        Assert.IsFalse(expandButton.gameObject.activeSelf);
    }

    [UnityTest]
    public IEnumerator CanAddLayers() {
        // should start with only the base layer
        MapEditorManager editor = GameObject.Find("MapEditorManager")
            .GetComponent<MapEditorManager>();
        GameObject layerScrollContent = GameObject.Find("LayerScrollContent");
        GameObject baseLayer = layerScrollContent.transform.GetChild(0).gameObject;
        Assert.AreEqual(1, MapEditorManager.Layers.Count);
        Assert.AreEqual(layerScrollContent.transform.childCount, 1);

        Assert.AreEqual(0, editor.CurrentLayer);
        Assert.IsTrue(Layer.LayerStatus[baseLayer.name]);

        // add a layer
        GameObject.Find("Layer Tool").GetComponent<Button>().onClick.Invoke();
        Assert.AreEqual(2, MapEditorManager.Layers.Count);
        Assert.AreEqual(layerScrollContent.transform.childCount, 2);
        GameObject newLayer = layerScrollContent.transform.GetChild(1).gameObject;
        yield return null;

        // check that the new layer is the selected layer
        Assert.AreEqual(1, editor.CurrentLayer);
        Assert.IsTrue(Layer.LayerStatus[newLayer.name]);
    }

    [Test]
    public void EditButtonChangesReadOnlyStatus() {
        GameObject baseLayer = GameObject.Find("LayerScrollContent").transform.GetChild(0)
            .gameObject;
        TMP_InputField layerNameInput = baseLayer.transform.Find("InputField (TMP)")
            .GetComponent<TMP_InputField>();
        Assert.IsTrue(layerNameInput.readOnly);
        baseLayer.transform.Find("Edit").GetComponent<Button>().onClick.Invoke();
        Assert.IsFalse(layerNameInput.readOnly);
    }

    [Test]
    public void CanRenameLayers() {
        GameObject baseLayer = GameObject.Find("LayerScrollContent").transform.GetChild(0)
            .gameObject;
        TMP_InputField layerNameInput = baseLayer.transform.Find("InputField (TMP)")
            .GetComponent<TMP_InputField>();
        string originalName = layerNameInput.text;
        layerNameInput.onSelect.Invoke(originalName);
        Assert.AreEqual(originalName, layerNameInput.GetComponent<LayerName>().CurrentText);

        // actually change the name
        layerNameInput.onSubmit.Invoke("New Layer Name");
        Assert.AreEqual("New Layer Name", layerNameInput.text);
        Assert.AreEqual("New Layer Name", layerNameInput.GetComponent<LayerName>().CurrentText);
    }

    [Test]
    public void ResetsToOriginalNameOnEmptyInput() {
        GameObject baseLayer = GameObject.Find("LayerScrollContent").transform.GetChild(0)
            .gameObject;
        TMP_InputField layerNameInput = baseLayer.transform.Find("InputField (TMP)")
            .GetComponent<TMP_InputField>();
        string originalName = layerNameInput.text;
        layerNameInput.onSelect.Invoke(originalName);
        Assert.AreNotEqual("", originalName);
        layerNameInput.onSubmit.Invoke("");
        Assert.AreEqual(originalName, layerNameInput.text);
    }

    [Test]
    public void TruncatesLongNames() {
        GameObject baseLayer = GameObject.Find("LayerScrollContent").transform.GetChild(0)
            .gameObject;
        TMP_InputField layerNameInput = baseLayer.transform.Find("InputField (TMP)")
            .GetComponent<TMP_InputField>();
        string originalName = layerNameInput.text;
        layerNameInput.onSelect.Invoke(originalName);
        string longName = "This is a very long layer name that should be truncated";
        layerNameInput.GetComponent<RectTransform>().sizeDelta = new Vector2(165, 0);
        layerNameInput.onSubmit.Invoke(longName);

        Assert.AreEqual(longName, layerNameInput.GetComponent<LayerName>().CurrentText);
        Assert.AreEqual(longName.Substring(0,10) + "...", layerNameInput.text);
    }

    [UnityTest]
    public IEnumerator CanOnlyEditAndDeleteSelectedLayer() {
        // check edit and delete on base layer
        MapEditorManager editor = GameObject.Find("MapEditorManager")
            .GetComponent<MapEditorManager>();
        GameObject layerScrollContent = GameObject.Find("LayerScrollContent");
        GameObject baseLayer = layerScrollContent.transform.GetChild(0).gameObject;
        Assert.AreEqual(0, editor.CurrentLayer);
        Assert.IsTrue(baseLayer.transform.Find("Edit").gameObject.activeSelf);
        Assert.IsTrue(baseLayer.transform.Find("TrashCan").gameObject.activeSelf);

        // create a new layer and switch to it
        GameObject.Find("Layer Tool").GetComponent<Button>().onClick.Invoke();
        yield return null;
        GameObject newLayer = layerScrollContent.transform.GetChild(1).gameObject;
        Assert.AreEqual(1, editor.CurrentLayer);

        // check that you can edit and delete
        Assert.IsTrue(newLayer.transform.Find("Edit").gameObject.activeSelf);
        Assert.IsTrue(newLayer.transform.Find("TrashCan").gameObject.activeSelf);

        // check that the other layer can't be edited or deleted
        Assert.IsFalse(baseLayer.transform.Find("Edit").gameObject.activeSelf);
        Assert.IsFalse(baseLayer.transform.Find("TrashCan").gameObject.activeSelf);
    }
}
