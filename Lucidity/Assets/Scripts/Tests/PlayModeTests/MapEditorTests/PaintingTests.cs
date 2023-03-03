using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

[TestFixture]
public class PaintingTests : MapEditorTests {

    [Test]
    public void DefaultsToBrushTool() {
        Assert.IsTrue(Tool.ToolStatus["Brush Tool"]);
        PlayModeTestUtil.CheckAllOtherToolsAreUnselected("Brush Tool");
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
        PlayModeTestUtil.CheckAllOtherToolsAreUnselected("Brush Tool");
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
        // should be expanded by default
        GameObject menuBody = GameObject.Find("Painting Body");
        Assert.IsTrue(menuBody.activeSelf);

        // collapse the menu
        Button collapseButton = GameObject.Find("Painting Header (Expanded)")
            .GetComponent<Button>();
        collapseButton.onClick.Invoke();
        Assert.IsFalse(menuBody.activeSelf);
        Assert.IsFalse(collapseButton.gameObject.activeSelf);

        // expand it again
        Button expandButton = GameObject.Find("Painting Header (Collapsed)")
            .GetComponent<Button>();
        expandButton.onClick.Invoke();
        Assert.IsTrue(menuBody.activeSelf);
        Assert.IsFalse(expandButton.gameObject.activeSelf);
    }

    [Test]
    public void CanPlaceAssets() {
        // make sure map is empty and brush tool is selected
        Assert.Zero(MapEditorManager.MapObjects.Count);
        Assert.IsTrue(Tool.ToolStatus["Brush Tool"]);

        // paint the asset
        Button fortressButton = GameObject.Find("FortressButton").GetComponent<Button>();
        fortressButton.onClick.Invoke();
        Assert.IsTrue(fortressButton.GetComponent<AssetController>().Clicked);
        Vector2 positionToPlace = new Vector2(-100, 150);
        MapEditorManager mapEditorManager = GameObject.Find("MapEditorManager")
            .GetComponent<MapEditorManager>();
        mapEditorManager.PaintAtPosition(positionToPlace);

        // check that the asset was placed correctly
        GameObject placedParent = GameObject.Find("TempFortressObject Parent");
        Assert.IsNotNull(placedParent);
        Assert.AreEqual(1, placedParent.transform.childCount);
        Assert.AreEqual(1, MapEditorManager.MapObjects.Count);
        Assert.AreEqual(positionToPlace.x, placedParent.transform.position.x, PlayModeTestUtil.FloatTolerance);
        Assert.AreEqual(positionToPlace.y, placedParent.transform.position.y, PlayModeTestUtil.FloatTolerance);
        Assert.Zero(placedParent.transform.localPosition.z);
    }

    [Test]
    public void CanPlaceGroupsOfAssets() {
        // make sure map is empty and brush tool is selected
        Assert.Zero(MapEditorManager.MapObjects.Count);
        Assert.IsTrue(Tool.ToolStatus["Brush Tool"]);

        // paint the asset group
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

        // check that the asset group was placed correctly
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
        // make sure map is empty, brush tool is selected, and spawn point position is (0,0)
        Assert.Zero(MapEditorManager.MapObjects.Count);
        Assert.IsTrue(Tool.ToolStatus["Brush Tool"]);
        Assert.AreEqual(new Vector2(0,0), MapEditorManager.SpawnPoint);

        // try to place it on the spawn point
        Button fortressButton = GameObject.Find("FortressButton").GetComponent<Button>();
        fortressButton.onClick.Invoke();
        Assert.IsTrue(fortressButton.GetComponent<AssetController>().Clicked);
        Vector2 positionToPlace = new Vector2(0,0);
        MapEditorManager mapEditorManager = GameObject.Find("MapEditorManager")
            .GetComponent<MapEditorManager>();
        mapEditorManager.PaintAtPosition(positionToPlace);

        // check that the asset was not placed
        Assert.AreEqual(0, MapEditorManager.MapObjects.Count);
    }

    [Test]
    public void CannotPlaceAssetOnTopOfAnotherAsset() {
        // make sure map is empty and brush tool is selected
        Assert.Zero(MapEditorManager.MapObjects.Count);
        Assert.IsTrue(Tool.ToolStatus["Brush Tool"]);

        // place the first asset
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

    [UnityTest]
    public IEnumerator CanPaintOnDifferentLayers() {
        MapEditorManager editor = GameObject.Find("MapEditorManager")
            .GetComponent<MapEditorManager>();
        Assert.AreEqual(0, editor.CurrentLayer);

        // place an asset on base layer
        GameObject.Find("FortressButton").GetComponent<Button>().onClick.Invoke();
        editor.PaintAtPosition(new Vector2(-100,150));
        Assert.AreEqual(1, MapEditorManager.MapObjects.Count);
        GameObject placedFortress = GameObject.Find("TempFortressObject(Clone)");
        Assert.IsTrue(MapEditorManager.Layers[0].ContainsKey(placedFortress.GetInstanceID()));
        
        // add and switch to layer 1
        GameObject.Find("Layer Tool").GetComponent<Button>().onClick.Invoke();
        yield return null;
        Assert.AreEqual(1, editor.CurrentLayer);
        
        // place an asset on layer 1
        GameObject.Find("HouseButton").GetComponent<Button>().onClick.Invoke();
        editor.PaintAtPosition(new Vector2(100,150));
        Assert.AreEqual(2, MapEditorManager.MapObjects.Count);
        GameObject placedHouse = GameObject.Find("TempHouseObject(Clone)");
        Assert.IsTrue(MapEditorManager.Layers[1].ContainsKey(placedHouse.GetInstanceID()));

        Assert.AreEqual(1, MapEditorManager.Layers[0].Count);
        Assert.AreEqual(1, MapEditorManager.Layers[1].Count);
    }

    [Test]
    public void CanPaintTheAssetBeforeAndAfterSwitchingTools() {
        // this is a regression test; a bug was found where if a user selected an asset and painted 
        // it, then switched tools, then switched back to paint and tried to paint the same asset,
        // the user could not paint with that asset

        Assert.IsTrue(Tool.ToolStatus["Brush Tool"]);
        MapEditorManager editor = GameObject.Find("MapEditorManager")
            .GetComponent<MapEditorManager>();

        // place the first instance of the fortress asset
        GameObject.Find("FortressButton").GetComponent<Button>().onClick.Invoke();
        editor.PaintAtPosition(new Vector2(-100,150));
        Assert.AreEqual(1, MapEditorManager.MapObjects.Count);
        
        // switch to some other tool
        GameObject.Find("Panning Tool").GetComponent<Button>().onClick.Invoke();
        Assert.IsTrue(Tool.ToolStatus["Panning Tool"]);
        Assert.IsFalse(Tool.ToolStatus["Brush Tool"]);

        // switch back to the brush tool
        GameObject.Find("Brush Tool").GetComponent<Button>().onClick.Invoke();
        Assert.IsTrue(Tool.ToolStatus["Brush Tool"]);
        Assert.IsFalse(Tool.ToolStatus["Panning Tool"]);
        
        // all asset buttons should be unclicked
        GameObject[] paintButtons = GameObject.FindGameObjectsWithTag("PaintButton");
        foreach (GameObject button in paintButtons) {
            Assert.IsFalse(button.GetComponent<AssetController>().Clicked);
        }

        // click the fortress button again
        GameObject.Find("FortressButton").GetComponent<Button>().onClick.Invoke();
        Assert.IsTrue(GameObject.Find("FortressButton").GetComponent<AssetController>().Clicked);

        // try to paint
        editor.PaintAtPosition(new Vector2(100,150));
        Assert.AreEqual(2, MapEditorManager.MapObjects.Count);
    }

}
