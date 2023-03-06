using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TestTools;
using UnityEngine.UI;

[TestFixture]
public class SelectionTests : MapEditorTests {

    [OneTimeSetUp]
    public void SelectionSetUp() {
        SelectMapObject.IsTesting = true;
    }

    [OneTimeTearDown]
    public void SelectionTearDown() {
        SelectMapObject.IsTesting = false;
    }

    [Test]
    public void CanSwitchToSelectionTool() {
        Button selectionToolButton = GameObject.Find("Selection Tool").GetComponent<Button>();
        selectionToolButton.onClick.Invoke();
        Assert.IsTrue(Tool.ToolStatus["Selection Tool"]);
        PlayModeTestUtil.CheckAllOtherToolsAreUnselected("Selection Tool");
        Assert.IsTrue(Tool.SelectionMenu.activeSelf);
        Assert.IsFalse(Tool.PaintingMenu.activeSelf);
    }

    [UnityTest]
    public IEnumerator CanOnlySelectObjectsOnCurrentLayer() {
        MapEditorManager editor = GameObject.Find("MapEditorManager")
            .GetComponent<MapEditorManager>();
        Assert.AreEqual(0, editor.CurrentLayer);
        Assert.Zero(MapEditorManager.MapObjects.Count);

        //  paint an object on base layer
        PlayModeTestUtil.PaintAnAsset(new Vector2(-100, 150), "Fortress");
        GameObject placedAsset = GameObject.Find("TempFortressObject(Clone)");
        Assert.IsTrue(MapEditorManager.Layers[0].ContainsKey(placedAsset.GetInstanceID()));

        // select the object while on base layer
        GameObject.Find("Selection Tool").GetComponent<Button>().onClick.Invoke();
        Assert.AreEqual(0, editor.CurrentLayer);
        Assert.IsFalse(Tool.SelectionOptions.activeSelf);
        SelectMapObject.SelectedObject = placedAsset;
        placedAsset.GetComponent<SelectMapObject>()
            .OnMouseDown();
        Assert.IsTrue(Tool.SelectionOptions.activeSelf);
        Assert.AreEqual("Editing TempFortressObject(Clone)", 
            GameObject.Find("SelectedObjectLabel").GetComponent<TMPro.TextMeshProUGUI>().text);
        Assert.AreEqual(Outline.Mode.OutlineAll, 
                        placedAsset.GetComponent<Outline>().OutlineMode);

        // create a new layer (should switch to it automatically)
        GameObject.Find("Layer Tool").GetComponent<Button>().onClick.Invoke();      
        yield return null;
        Assert.AreEqual(1, editor.CurrentLayer);

        // select the object while on a new layer
        Assert.IsFalse(Tool.SelectionOptions.activeSelf);
        SelectMapObject.SelectedObject = placedAsset;
        placedAsset.GetComponent<SelectMapObject>()
            .OnMouseDown();
        Assert.IsFalse(Tool.SelectionOptions.activeSelf);
        Assert.IsNull(placedAsset.GetComponent<Outline>());
    }

    [UnityTest]
    public IEnumerator CanSelectSpawnPointRegardlessOfLayer() {
        GameObject spawnPoint = GameObject.Find("Spawn Point");
        MapEditorManager editor = GameObject.Find("MapEditorManager")
            .GetComponent<MapEditorManager>();
        Assert.AreEqual(0, editor.CurrentLayer);
        Assert.IsNull(spawnPoint.GetComponent<Outline>());

        // select the spawn point while on base layer
        GameObject.Find("Selection Tool").GetComponent<Button>().onClick.Invoke();
        Assert.IsFalse(Tool.SelectionOptions.activeSelf);
        Assert.IsFalse(Tool.SpawnPointOptions.activeSelf);
        SelectMapObject.SelectedObject = spawnPoint;
        spawnPoint.GetComponent<SelectMapObject>()
            .OnMouseDown();
        
        // assert spawn point selection options is active and the spawn point has an outline
        Assert.IsFalse(Tool.SelectionOptions.activeSelf);
        Assert.IsTrue(Tool.SpawnPointOptions.activeSelf);
        Assert.AreEqual("Editing Spawn Point",
            GameObject.Find("SelectedObjectLabel").GetComponent<TMPro.TextMeshProUGUI>().text);
        Assert.AreEqual(Outline.Mode.OutlineAll, 
                        spawnPoint.GetComponent<Outline>().OutlineMode);
        
        // add new layer (switches to it automatically)
        GameObject.Find("Layer Tool").GetComponent<Button>().onClick.Invoke();
        yield return null;
        Assert.AreEqual(1, editor.CurrentLayer);
        Assert.IsFalse(Tool.SpawnPointOptions.activeSelf);
        Assert.IsNull(spawnPoint.GetComponent<Outline>());

        // select the spawn point while on a new layer
        SelectMapObject.SelectedObject = spawnPoint;
        spawnPoint.GetComponent<SelectMapObject>()
            .OnMouseDown();
        Assert.IsTrue(Tool.SpawnPointOptions.activeSelf);
        Assert.AreEqual("Editing Spawn Point",
            GameObject.Find("SelectedObjectLabel").GetComponent<TMPro.TextMeshProUGUI>().text);
        Assert.AreEqual(Outline.Mode.OutlineAll, 
                        spawnPoint.GetComponent<Outline>().OutlineMode);
    }

    [Test]
    public void CanCollapseAndExpandSelectionMenu() {
        // open the selection menu
        Button selectionButton = GameObject.Find("Selection Tool").GetComponent<Button>();
        selectionButton.onClick.Invoke();
        GameObject menuBody = GameObject.Find("Selection Body");
        Assert.IsTrue(menuBody.activeSelf);

        // collapse the menu
        Button collapseButton = GameObject.Find("Selection Header (Expanded)")
            .GetComponent<Button>();
        collapseButton.onClick.Invoke();
        Assert.IsFalse(menuBody.activeSelf);
        Assert.IsFalse(collapseButton.gameObject.activeSelf);

        // expand the menu again
        Button expandButton = GameObject.Find("Selection Header (Collapsed)")
            .GetComponent<Button>();
        expandButton.onClick.Invoke();
        Assert.IsTrue(menuBody.activeSelf);
        Assert.IsFalse(expandButton.gameObject.activeSelf);
    }

    [Test]
    public void CanDeleteAsset() {
        // paint an object to delete
        PlayModeTestUtil.PaintAnAsset(new Vector2(-100, 150), "Fortress");
        
        // select the object
        GameObject.Find("Selection Tool").GetComponent<Button>().onClick.Invoke();
        GameObject placedAsset = GameObject.Find("TempFortressObject(Clone)");
        SelectMapObject.SelectedObject = placedAsset;
        placedAsset.GetComponent<SelectMapObject>()
            .OnMouseDown();

        // delete the object
        Button deleteButton = GameObject.Find("Delete Button").GetComponent<Button>();
        deleteButton.onClick.Invoke();
        int placedObjectId = new List<int>(MapEditorManager.MapObjects.Keys)[0];
        Assert.IsFalse(MapEditorManager.MapObjects[placedObjectId].IsActive);
    }
}
