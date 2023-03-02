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
        // the two MapObjects should be placed at any combination of a pair of x and y coordinates,
        // except on the same position
        Assert.IsTrue(!((MapEditorManager.MapObjects[keys[0]].MapOffset.x
                            == MapEditorManager.MapObjects[keys[1]].MapOffset.x)
                            && (MapEditorManager.MapObjects[keys[0]].MapOffset.y
                            == MapEditorManager.MapObjects[keys[1]].MapOffset.y)));
    }

    [UnityTest]
    public IEnumerator CanPlaceGroupsOfAssetsWithCollisionsHandled() {
        // make sure map is empty and brush tool is selected
        Assert.Zero(MapEditorManager.MapObjects.Count);
        Assert.IsTrue(Tool.ToolStatus["Brush Tool"]);

        // paint the asset group
        Button fortressButton = GameObject.Find("FortressButton").GetComponent<Button>();
        InputField countInput = GameObject.Find("CountInput").GetComponent<InputField>();
        countInput.text = "4";
        countInput.onEndEdit.Invoke(countInput.text);
        Assert.AreEqual(4, AssetOptions.AssetCount);
        fortressButton.onClick.Invoke();
        Assert.IsTrue(fortressButton.GetComponent<AssetController>().Clicked);
        Assert.AreEqual(0, MapEditorManager.MapObjects.Count);
        Vector2 positionToPlace = new Vector2(3f, 2.5f);
        MapEditorManager mapEditorManager = GameObject.Find("MapEditorManager")
            .GetComponent<MapEditorManager>();
        mapEditorManager.PaintAtPosition(positionToPlace);
        Assert.AreEqual(4, MapEditorManager.MapObjects.Count);
        yield return new WaitForFixedUpdate();
        yield return null;

        // place a group of four assets, wherein two assets overlap with the previous group;
        // must yield for the coroutine RevertMaterialAndDestroy()
        mapEditorManager.PaintAtPosition(positionToPlace + new Vector2(0, 1.5f));
        yield return new WaitForFixedUpdate();
        yield return new WaitForSeconds(0.5f);
        Assert.AreEqual(6, MapEditorManager.MapObjects.Count);
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
}
