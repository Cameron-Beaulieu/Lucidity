using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

[TestFixture]
public class LayerTests : MapEditorTests {

    [Test]
    public void EmptyMapHasOneLayer() {
        // Check for starting layer
        GameObject layerScrollContent = GameObject.Find("LayerScrollContent");
        Assert.AreEqual(layerScrollContent.transform.childCount, 1);
    }

    [Test]
    public void CanCollapseAndExpandLayersMenu() {
        // check already expanded
        GameObject menuBody = GameObject.Find("Layers Body");
        Assert.IsTrue(menuBody.activeSelf);
        Assert.IsNotNull(GameObject.Find("Layer Tool")); // layer tool exists only on expanded menu
        
        // collapse the menu
        Button collapseButton = GameObject.Find("Layers Header (Expanded)").GetComponent<Button>();
        collapseButton.onClick.Invoke();
        Assert.IsFalse(menuBody.activeSelf);
        Assert.IsFalse(collapseButton.gameObject.activeSelf);
        Assert.IsNull(GameObject.Find("Layer Tool"));
        
        // expand it again
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

    [UnityTest]
    public IEnumerator CanSwitchBetweenLayers() {
        // add a layer in addition to the base layer
        MapEditorManager editor = GameObject.Find("MapEditorManager")
            .GetComponent<MapEditorManager>();
        GameObject layerScrollContent = GameObject.Find("LayerScrollContent");
        GameObject baseLayer = layerScrollContent.transform.GetChild(0).gameObject;
        GameObject.Find("Layer Tool").GetComponent<Button>().onClick.Invoke();
        yield return null;
        Assert.AreEqual(2, MapEditorManager.Layers.Count);
        GameObject newLayer = layerScrollContent.transform.GetChild(1).gameObject;

        // check that the new layer is the selected layer
        Assert.AreEqual(1, editor.CurrentLayer);
        Assert.IsTrue(Layer.LayerStatus[newLayer.name]);
        Assert.IsFalse(Layer.LayerStatus[baseLayer.name]);

        // switch to the base layer
        baseLayer.GetComponent<Button>().onClick.Invoke();
        yield return null;
        Assert.AreEqual(0, editor.CurrentLayer);
        Assert.IsTrue(Layer.LayerStatus[baseLayer.name]);
        Assert.IsFalse(Layer.LayerStatus[newLayer.name]);
    }

    [Test]
    public void EditButtonChangesReadOnlyStatusOfLayerLabel() {
        // Getting layer name input
        GameObject baseLayer = GameObject.Find("LayerScrollContent").transform.GetChild(0)
            .gameObject;
        TMP_InputField layerNameInput = baseLayer.transform.Find("InputField (TMP)")
            .GetComponent<TMP_InputField>();

        // Checking readonly status functionality
        Assert.IsTrue(layerNameInput.readOnly);
        baseLayer.transform.Find("Edit").GetComponent<Button>().onClick.Invoke();
        Assert.IsFalse(layerNameInput.readOnly);
    }

    [Test]
    public void CanRenameLayers() {
        // get the default layer name
        GameObject baseLayer = GameObject.Find("LayerScrollContent").transform.GetChild(0)
            .gameObject;
        TMP_InputField layerNameInput = baseLayer.transform.Find("InputField (TMP)")
            .GetComponent<TMP_InputField>();

        // Setting LayerName.CurrentText to the current layer name
        string originalName = layerNameInput.text;
        layerNameInput.onSelect.Invoke(originalName);
        Assert.AreEqual(originalName, layerNameInput.GetComponent<LayerName>().CurrentText);

        // change the name
        layerNameInput.onSubmit.Invoke("New Layer Name");
        Assert.AreEqual("New Layer Name", layerNameInput.text);
        Assert.AreEqual("New Layer Name", layerNameInput.GetComponent<LayerName>().CurrentText);
    }

    [Test]
    public void ResetsToOriginalNameOnEmptyInput() {
        // get the default layer name
        GameObject baseLayer = GameObject.Find("LayerScrollContent").transform.GetChild(0)
            .gameObject;
        TMP_InputField layerNameInput = baseLayer.transform.Find("InputField (TMP)")
            .GetComponent<TMP_InputField>();

        // Setting LayerName.CurrentText to the current layer name
        string originalName = layerNameInput.text;
        layerNameInput.onSelect.Invoke(originalName);

        // Confirm layer name cannot be empty
        Assert.AreNotEqual("", originalName);
        layerNameInput.onSubmit.Invoke("");
        Assert.AreEqual(originalName, layerNameInput.text);
    }

    [Test]
    public void TruncatesLongNames() {
        // Get starting layer input
        GameObject baseLayer = GameObject.Find("LayerScrollContent").transform.GetChild(0)
            .gameObject;
        TMP_InputField layerNameInput = baseLayer.transform.Find("InputField (TMP)")
            .GetComponent<TMP_InputField>();
        string originalName = layerNameInput.text;
        layerNameInput.onSelect.Invoke(originalName);

        // Changing layer name
        string longName = "This is a very long layer name that should be truncated";
        // setting .text through code alone doesn't change the rect transform's width, so we have 
        // to set the sizeDelta manually
        layerNameInput.GetComponent<RectTransform>().sizeDelta = new Vector2(165, 0);
        layerNameInput.onSubmit.Invoke(longName);
        Assert.AreEqual(longName.Substring(0,10) + "...", 
                        layerNameInput.GetComponent<LayerName>().CurrentText);
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

    [UnityTest]
    public IEnumerator LayerNamesCannotBeTheSame() {
        LayerName.IsTesting = true;
        // Get the base layer
        MapEditorManager editor = GameObject.Find("MapEditorManager")
            .GetComponent<MapEditorManager>();
        GameObject layerScrollContent = GameObject.Find("LayerScrollContent");
        GameObject baseLayer = layerScrollContent.transform.GetChild(0).gameObject;

        // Rename base layer
        TMP_InputField baseLayerInput = baseLayer.transform.Find("InputField (TMP)")
            .GetComponent<TMP_InputField>();
        baseLayerInput.onSubmit.Invoke("abc");

        // Create a new layer
        GameObject.Find("Layer Tool").GetComponent<Button>().onClick.Invoke();
        yield return null;
        Assert.AreEqual(2, MapEditorManager.Layers.Count);
        GameObject newLayer = layerScrollContent.transform.GetChild(1).gameObject;

        // Give the new layer the same name
        TMP_InputField newLayerInput = newLayer.transform.Find("InputField (TMP)")
            .GetComponent<TMP_InputField>();
        newLayerInput.onSubmit.Invoke("abc");

        Assert.AreEqual("abc", baseLayerInput.text);
        Assert.AreEqual("abc2", newLayerInput.text);
    }

    [UnityTest]
    public IEnumerator LayerNamesWithTruncationCannotBeTheSame() {
        LayerName.IsTesting = true;
        // Get the base layer
        MapEditorManager editor = GameObject.Find("MapEditorManager")
            .GetComponent<MapEditorManager>();
        GameObject layerScrollContent = GameObject.Find("LayerScrollContent");
        GameObject baseLayer = layerScrollContent.transform.GetChild(0).gameObject;

        string longText = "A really cool long name";

        // Rename base layer
        TMP_InputField baseLayerInput = baseLayer.transform.Find("InputField (TMP)")
            .GetComponent<TMP_InputField>();
        baseLayerInput.GetComponent<RectTransform>().sizeDelta = new Vector2(165, 0);
        baseLayerInput.onSubmit.Invoke(longText);

        // Create a new layer
        GameObject.Find("Layer Tool").GetComponent<Button>().onClick.Invoke();
        yield return null;
        Assert.AreEqual(2, MapEditorManager.Layers.Count);
        GameObject newLayer = layerScrollContent.transform.GetChild(1).gameObject;

        // Give the new layer the same name
        TMP_InputField newLayerInput = newLayer.transform.Find("InputField (TMP)")
            .GetComponent<TMP_InputField>();
        newLayerInput.GetComponent<RectTransform>().sizeDelta = new Vector2(165, 0);
        newLayerInput.onSubmit.Invoke(longText);

        Assert.AreEqual(longText.Substring(0, 10) + "...", baseLayerInput.text);
        Assert.AreEqual(longText.Substring(0, 9) + "2...", newLayerInput.text);
    }

    [UnityTest]
    public IEnumerator CanStackTreeOnMountain() {
        // paint the mountain on base layer
        Button mountainButton = GameObject.Find("MountainButton").GetComponent<Button>();
        mountainButton.onClick.Invoke();
        Assert.IsTrue(mountainButton.GetComponent<AssetController>().Clicked);
        Vector2 positionToPlace = new Vector2(3, 3);
        MapEditorManager mapEditorManager = GameObject.Find("MapEditorManager")
            .GetComponent<MapEditorManager>();
        mapEditorManager.PaintAtPosition(positionToPlace);
        Assert.AreEqual(1, MapEditorManager.Layers[0].Count);

        // add new layer
        GameObject.Find("Layer Tool").GetComponent<Button>().onClick.Invoke();
        yield return null;
        Assert.AreEqual(2, MapEditorManager.Layers.Count);

        // paint the tree on mountain
        Button treeButton = GameObject.Find("TreeButton").GetComponent<Button>();
        treeButton.onClick.Invoke();
        Assert.IsTrue(treeButton.GetComponent<AssetController>().Clicked);
        mapEditorManager.PaintAtPosition(positionToPlace + new Vector2(0.2f,0.2f));
        yield return null;
        yield return new WaitForSeconds(0.5f);
        Assert.AreEqual(1, MapEditorManager.Layers[1].Count);
    }

    [UnityTest]
    public IEnumerator CannotStackTreePartiallyOnMountain() {
        // paint the mountain on base layer
        Button mountainButton = GameObject.Find("MountainButton").GetComponent<Button>();
        mountainButton.onClick.Invoke();
        Assert.IsTrue(mountainButton.GetComponent<AssetController>().Clicked);
        Vector2 positionToPlace = new Vector2(3, 3);
        MapEditorManager mapEditorManager = GameObject.Find("MapEditorManager")
            .GetComponent<MapEditorManager>();
        mapEditorManager.PaintAtPosition(positionToPlace);
        Assert.AreEqual(1, MapEditorManager.Layers[0].Count);

        // add new layer
        GameObject.Find("Layer Tool").GetComponent<Button>().onClick.Invoke();
        yield return null;
        Assert.AreEqual(2, MapEditorManager.Layers.Count);
        Assert.AreEqual(1, mapEditorManager.CurrentLayer);

        // paint the tree partially on mountain
        Button treeButton = GameObject.Find("TreeButton").GetComponent<Button>();
        treeButton.onClick.Invoke();
        Assert.IsTrue(treeButton.GetComponent<AssetController>().Clicked);
        mapEditorManager.PaintAtPosition(positionToPlace + new Vector2(0.7f,0.7f));
        yield return null;
        yield return new WaitForSeconds(0.5f);
        Assert.AreEqual(0, MapEditorManager.Layers[1].Count);
    }
}
