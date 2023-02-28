using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
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
        CheckAllOtherToolsAreUnselected("Selection Tool");
        Assert.IsTrue(Tool.SelectionMenu.activeSelf);
        Assert.IsFalse(Tool.PaintingMenu.activeSelf);
    }

    [Test]
    public void CanSelectAsset() {
        // paint an object to select
        PaintAnAsset(new Vector2(-100, 150), "Fortress");
        GameObject.Find("Selection Tool").GetComponent<Button>().onClick.Invoke();

        // select the object
        Assert.IsFalse(Tool.SelectionOptions.activeSelf);
        GameObject placedAsset = GameObject.Find("TempFortressObject(Clone)");
        SelectMapObject.SelectedObject = placedAsset;
        placedAsset.GetComponent<SelectMapObject>()
            .OnPointerClick(new PointerEventData(EventSystem.current));
        
        // assert selection options is active and the object has an outline
        Assert.IsTrue(Tool.SelectionOptions.activeSelf);
        Assert.AreEqual("Editing TempFortressObject(Clone)", 
            GameObject.Find("SelectedObjectLabel").GetComponent<TMPro.TextMeshProUGUI>().text);
        Assert.AreEqual(Outline.Mode.OutlineAll, 
                        SelectMapObject.SelectedObject.GetComponent<Outline>().OutlineMode);
    }

    [Test]
    public void CanSelectSpawnPoint() {
        GameObject spawnPoint = GameObject.Find("Spawn Point");

        // select the spawn point
        GameObject.Find("Selection Tool").GetComponent<Button>().onClick.Invoke();
        Assert.IsFalse(Tool.SelectionOptions.activeSelf);
        Assert.IsFalse(Tool.SpawnPointOptions.activeSelf);
        SelectMapObject.SelectedObject = spawnPoint;
        spawnPoint.GetComponent<SelectMapObject>()
            .OnPointerClick(new PointerEventData(EventSystem.current));
        
        // assert spawn point selection options is active and the spawn point has an outline
        Assert.IsFalse(Tool.SelectionOptions.activeSelf);
        Assert.IsTrue(Tool.SpawnPointOptions.activeSelf);
        Assert.AreEqual("Editing Spawn Point",
            GameObject.Find("SelectedObjectLabel").GetComponent<TMPro.TextMeshProUGUI>().text);
        Assert.AreEqual(Outline.Mode.OutlineAll, 
                        SelectMapObject.SelectedObject.GetComponent<Outline>().OutlineMode);
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
        PaintAnAsset(new Vector2(-100, 150), "Fortress");
        
        // select the object
        GameObject.Find("Selection Tool").GetComponent<Button>().onClick.Invoke();
        GameObject placedAsset = GameObject.Find("TempFortressObject(Clone)");
        SelectMapObject.SelectedObject = placedAsset;
        placedAsset.GetComponent<SelectMapObject>()
            .OnPointerClick(new PointerEventData(EventSystem.current));

        // delete the object
        Button deleteButton = GameObject.Find("Delete Button").GetComponent<Button>();
        deleteButton.onClick.Invoke();
        int placedObjectId = new List<int>(MapEditorManager.MapObjects.Keys)[0];
        Assert.IsFalse(MapEditorManager.MapObjects[placedObjectId].IsActive);
    }
}
