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

        // switch back to brush tool
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
        // Test switching to every paint button
        foreach (GameObject paintButton1 in paintButtons) {
            paintButton1.GetComponent<Button>().onClick.Invoke();
            Assert.IsTrue(paintButton1.GetComponent<AssetController>().Clicked);
            // Check all other paintButtons are false
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
        GameObject placedParent = GameObject.Find("FortressObject Parent");
        Assert.IsNotNull(placedParent);
        Assert.AreEqual(1, placedParent.transform.childCount);
        Assert.AreEqual(1, MapEditorManager.MapObjects.Count);
        Assert.AreEqual(positionToPlace.x, placedParent.transform.position.x, PlayModeTestUtil.FloatTolerance);
        Assert.AreEqual(positionToPlace.y, placedParent.transform.position.y, PlayModeTestUtil.FloatTolerance);
        Assert.AreEqual(0, placedParent.transform.localPosition.z, PlayModeTestUtil.FloatTolerance);
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
    public IEnumerator DestroysPaintedAssetIfItCausesCollision() {
        // place the first asset
        Button fortressButton = GameObject.Find("FortressButton").GetComponent<Button>();
        fortressButton.onClick.Invoke();
        MapEditorManager mapEditorManager = GameObject.Find("MapEditorManager")
            .GetComponent<MapEditorManager>();
        mapEditorManager.PaintAtPosition(new Vector2(-100, 150));
        GameObject placedFortress = GameObject.Find("FortressObject(Clone)");
        Assert.AreEqual(1, MapEditorManager.MapObjects.Count);

        // place another asset far away (shouldn't touch the first one)
        // this is done so that the third placement can cause a collision
        Button treeButton = GameObject.Find("TreeButton").GetComponent<Button>();
        treeButton.onClick.Invoke();
        mapEditorManager.PaintAtPosition(new Vector2(100, -150));
        Assert.AreEqual(2, MapEditorManager.MapObjects.Count);

        // place another asset, but this time it should collide with the first one 
        mapEditorManager.PaintAtPosition(new Vector2(-100, 150));
        
        // check collision handling is done (colliding assets turn red and the asset causing the 
        // collision should be destroyed)
        GameObject collidingTree = GameObject.Find("TreeObject(Clone)");
        Debug.Break();
        yield return null;
        Assert.AreEqual(Color.red, collidingTree.GetComponent<Image>().color);
        Assert.AreEqual(Color.red, placedFortress.GetComponent<Image>().color);
        yield return new WaitForSecondsRealtime(0.5f);
        Assert.AreEqual(Color.white, collidingTree.GetComponent<Image>().color);
        Assert.AreEqual(Color.white, placedFortress.GetComponent<Image>().color);
        yield return new WaitForEndOfFrame();
        Assert.AreEqual(2, MapEditorManager.MapObjects.Count);
        Assert.IsTrue(collidingTree == null);

        // check that the first asset placed (the one that was collided with) is still there
        Assert.IsNotNull(placedFortress);
    }

    [UnityTest]
    public IEnumerator CanPaintOnDifferentLayers() {
        // Check CurrentLayer is tracking base layer
        MapEditorManager editor = GameObject.Find("MapEditorManager")
            .GetComponent<MapEditorManager>();
        Assert.AreEqual(0, editor.CurrentLayer);

        // place an asset on base layer
        GameObject.Find("FortressButton").GetComponent<Button>().onClick.Invoke();
        editor.PaintAtPosition(new Vector2(-100,150));
        Assert.AreEqual(1, MapEditorManager.MapObjects.Count);
        GameObject placedFortress = GameObject.Find("FortressObject(Clone)");
        Assert.IsTrue(MapEditorManager.Layers[0].ContainsKey(placedFortress.GetInstanceID()));
        
        // add and switch to layer 1
        GameObject.Find("Layer Tool").GetComponent<Button>().onClick.Invoke();
        yield return null;
        Assert.AreEqual(1, editor.CurrentLayer);
        
        // place an asset on layer 1
        GameObject.Find("HouseButton").GetComponent<Button>().onClick.Invoke();
        editor.PaintAtPosition(new Vector2(100,150));
        Assert.AreEqual(2, MapEditorManager.MapObjects.Count);
        GameObject placedHouse = GameObject.Find("HouseObject(Clone)");
        Assert.IsTrue(MapEditorManager.Layers[1].ContainsKey(placedHouse.GetInstanceID()));

        // Check number of assets on each layer
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
