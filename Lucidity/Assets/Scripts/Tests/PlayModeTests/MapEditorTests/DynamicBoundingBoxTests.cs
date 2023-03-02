using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

[TestFixture]
public class DynamicBoundingBoxTests : MapEditorTests {

    // asset hover with higher count
    [Test]
    public void DynamicBoundingBoxAssetHoverHasCorrectCount() {
        Button fortressButton = GameObject.Find("FortressButton").GetComponent<Button>();
        InputField countInput = GameObject.Find("CountInput").GetComponent<InputField>();
        countInput.text = "2";
        countInput.onEndEdit.Invoke(countInput.text);
        Assert.AreEqual(2, AssetOptions.AssetCount);
        fortressButton.onClick.Invoke();
        Assert.IsTrue(fortressButton.GetComponent<AssetController>().Clicked);

        // the dynamic bounding box parent on the hovering assets is also given the tag AssetImage
        Assert.AreEqual(3, GameObject.FindGameObjectsWithTag("AssetImage").Length);
    }

    [UnityTest]
    public IEnumerator DynamicBoundingBoxDeletesOnRelease() {
        // place the first asset
        Button fortressButton = GameObject.Find("FortressButton").GetComponent<Button>();
        fortressButton.onClick.Invoke();
        Assert.IsTrue(fortressButton.GetComponent<AssetController>().Clicked);
        Vector2 positionToPlace = new Vector2(-100, 150);
        MapEditorManager mapEditorManager = GameObject.Find("MapEditorManager")
            .GetComponent<MapEditorManager>();
        Assert.AreEqual(0, GameObject.FindGameObjectsWithTag("DynamicBoundingBox").Length);
        mapEditorManager.PaintAtPosition(positionToPlace);
        yield return null;
        Assert.AreEqual(1, GameObject.FindGameObjectsWithTag("DynamicBoundingBox").Length);

        // skip frame to allow for dynamic bounding boxes to be deleted
        yield return new WaitForFixedUpdate();
        Assert.AreEqual(0, GameObject.FindGameObjectsWithTag("DynamicBoundingBox").Length);
    }

    [UnityTest]
    public IEnumerator DynamicBoundingBoxPersistsOnHold() {
        // place the first asset
        Button fortressButton = GameObject.Find("FortressButton").GetComponent<Button>();
        fortressButton.onClick.Invoke();
        Assert.IsTrue(fortressButton.GetComponent<AssetController>().Clicked);
        Vector2 positionToPlace = new Vector2(-100, 150);
        MapEditorManager mapEditorManager = GameObject.Find("MapEditorManager")
            .GetComponent<MapEditorManager>();
        Assert.AreEqual(0, GameObject.FindGameObjectsWithTag("DynamicBoundingBox").Length);
        mapEditorManager.PaintAtPosition(positionToPlace);
        mapEditorManager.PaintAtPosition(positionToPlace + new Vector2(100f, 100f));
        yield return null;
        Assert.AreEqual(2, GameObject.FindGameObjectsWithTag("DynamicBoundingBox").Length);
    }
}