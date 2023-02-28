using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

[TestFixture]
public class PaintingTests : MapEditorTests {

    [Test]
    public void DefaultsToBrushTool() {
        Assert.IsTrue(Tool.ToolStatus["Brush Tool"]);
        Util.CheckAllOtherToolsAreUnselected("Brush Tool");
        Assert.IsTrue(Tool.PaintingMenu.activeSelf);
    }

    [Test]
    public void CanSwitchToBrushTool() {
        // switch to another tool first since brush tool is default
        Button selectionToolButton = GameObject.Find("Selection Tool").GetComponent<Button>();
        selectionToolButton.onClick.Invoke();
        Assert.IsFalse(Tool.ToolStatus["Brush Tool"]);

        Button brushToolButton = GameObject.Find("Brush Tool").GetComponent<Button>();
        brushToolButton.onClick.Invoke();
        Assert.IsTrue(Tool.ToolStatus["Brush Tool"]);
        Util.CheckAllOtherToolsAreUnselected("Brush Tool");
        Assert.IsFalse(Tool.SelectionMenu.activeSelf);
        Assert.IsTrue(Tool.PaintingMenu.activeSelf);
    }

    [Test]
    public void CanSwitchBetweenPaintButtons() {
        Assert.IsTrue(Tool.ToolStatus["Brush Tool"]);
        GameObject[] paintButtons = GameObject.FindGameObjectsWithTag("PaintButton");
        foreach (GameObject paintButton1 in paintButtons) {
            paintButton1.GetComponent<Button>().onClick.Invoke();
            Assert.IsTrue(paintButton1.GetComponent<AssetController>().Clicked);
            foreach (GameObject paintButton2 in paintButtons) {
                if (paintButton1 != paintButton2) {
                    Assert.IsFalse(paintButton2.GetComponent<AssetController>().Clicked);
                }
            }
        } 
    }

    [Test]
    public void CanCollapseAndExpandPaintingMenu() {
        GameObject menuBody = GameObject.Find("Painting Body");
        Assert.IsTrue(menuBody.activeSelf);
        Button collapseButton = GameObject.Find("Painting Header (Expanded)")
            .GetComponent<Button>();
        collapseButton.onClick.Invoke();
        Assert.IsFalse(menuBody.activeSelf);
        Assert.IsFalse(collapseButton.gameObject.activeSelf);
        Button expandButton = GameObject.Find("Painting Header (Collapsed)")
            .GetComponent<Button>();
        expandButton.onClick.Invoke();
        Assert.IsTrue(menuBody.activeSelf);
        Assert.IsFalse(expandButton.gameObject.activeSelf);
    }

    [Test]
    public void CanPlaceAssets() {
    Assert.Zero(MapEditorManager.MapObjects.Count);
    Assert.IsTrue(Tool.ToolStatus["Brush Tool"]);
    Button fortressButton = GameObject.Find("FortressButton").GetComponent<Button>();
    fortressButton.onClick.Invoke();
    Assert.IsTrue(fortressButton.GetComponent<AssetController>().Clicked);

    Vector2 positionToPlace = new Vector2(-100, 150);
    MapEditorManager mapEditorManager = GameObject.Find("MapEditorManager")
        .GetComponent<MapEditorManager>();
    mapEditorManager.PaintAtPosition(positionToPlace);

    GameObject placedParent = GameObject.Find("TempFortressObject Parent");
    Assert.IsNotNull(placedParent);
    Assert.AreEqual(1, placedParent.transform.childCount);
    Assert.AreEqual(1, MapEditorManager.MapObjects.Count);
    Assert.AreEqual(positionToPlace.x, placedParent.transform.position.x, 0.005f);
    Assert.AreEqual(positionToPlace.y, placedParent.transform.position.y, 0.005f);
    Assert.Zero(placedParent.transform.localPosition.z);
    }

    [Test]
    public void CanPlaceGroupsOfAssets() {
        Assert.Zero(MapEditorManager.MapObjects.Count);
        Assert.IsTrue(Tool.ToolStatus["Brush Tool"]);
        Button fortressButton = GameObject.Find("FortressButton").GetComponent<Button>();
        InputField countInput = GameObject.Find("CountInput").GetComponent<InputField>();
        countInput.text = "2";
        countInput.onEndEdit.Invoke(countInput.text);
        Assert.AreEqual(2, AssetOptions.AssetCount);
        fortressButton.onClick.Invoke();
        Assert.IsTrue(fortressButton.GetComponent<AssetController>().Clicked);

        Vector2 positionToPlace = new Vector2(-100, 150);
        MapEditorManager mapEditorManager = GameObject.Find("MapEditorManager")
            .GetComponent<MapEditorManager>();
        mapEditorManager.PaintAtPosition(positionToPlace);

        Assert.AreEqual(2, MapEditorManager.MapObjects.Count);
        List<int> keys = new List<int>(MapEditorManager.MapObjects.Keys);

        // the two MapObjects should be placed at the same y position, but different x positions 
        // because they are side by side
        Assert.AreNotEqual(MapEditorManager.MapObjects[keys[0]].MapOffset.x, 
                           MapEditorManager.MapObjects[keys[1]].MapOffset.x);
        Assert.AreEqual(MapEditorManager.MapObjects[keys[0]].MapOffset.y, 
                        MapEditorManager.MapObjects[keys[1]].MapOffset.y);
    }

    [Test]
    public void CannotPlaceAssetOnTopOfSpawnPoint() {
        Assert.Zero(MapEditorManager.MapObjects.Count);
        Assert.IsTrue(Tool.ToolStatus["Brush Tool"]);
        Button fortressButton = GameObject.Find("FortressButton").GetComponent<Button>();
        fortressButton.onClick.Invoke();
        Assert.IsTrue(fortressButton.GetComponent<AssetController>().Clicked);

        Vector2 positionToPlace = new Vector2(0,0);
        MapEditorManager mapEditorManager = GameObject.Find("MapEditorManager")
            .GetComponent<MapEditorManager>();
        mapEditorManager.PaintAtPosition(positionToPlace);

        Assert.AreEqual(0, MapEditorManager.MapObjects.Count);
    }

    [Test]
    public void CannotPlaceAssetOnTopOfAnotherAsset() {
        Assert.Zero(MapEditorManager.MapObjects.Count);
        Assert.IsTrue(Tool.ToolStatus["Brush Tool"]);
        Button fortressButton = GameObject.Find("FortressButton").GetComponent<Button>();
        fortressButton.onClick.Invoke();
        Assert.IsTrue(fortressButton.GetComponent<AssetController>().Clicked);

        Vector2 positionToPlace = new Vector2(-100, 150);
        MapEditorManager mapEditorManager = GameObject.Find("MapEditorManager")
            .GetComponent<MapEditorManager>();
        mapEditorManager.PaintAtPosition(positionToPlace);
        Assert.AreEqual(1, MapEditorManager.MapObjects.Count);

        // try to place another asset on top of the first one
        mapEditorManager.PaintAtPosition(positionToPlace + new Vector2(0.5f, 0.5f));
        Assert.AreEqual(1, MapEditorManager.MapObjects.Count);
    }
}
