using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TestTools;
using UnityEngine.UI;

[TestFixture]
public class SelectionTests : MapEditorTests {
    [Test]
    public void CanCollapseAndExpandSelectionMenu() {
        Button selectionButton = GameObject.Find("Selection Tool").GetComponent<Button>();
        selectionButton.onClick.Invoke();
        GameObject menuBody = GameObject.Find("Selection Body");
        Assert.IsTrue(menuBody.activeSelf);
        Button collapseButton = GameObject.Find("Selection Header (Expanded)")
            .GetComponent<Button>();
        collapseButton.onClick.Invoke();
        Assert.IsFalse(menuBody.activeSelf);
        Assert.IsFalse(collapseButton.gameObject.activeSelf);
        Button expandButton = GameObject.Find("Selection Header (Collapsed)")
            .GetComponent<Button>();
        expandButton.onClick.Invoke();
        Assert.IsTrue(menuBody.activeSelf);
        Assert.IsFalse(expandButton.gameObject.activeSelf);
    }

    [Test]
    public void CanDeleteAsset() {
        Button fortressButton = GameObject.Find("FortressButton").GetComponent<Button>();
        fortressButton.onClick.Invoke();
        Assert.IsTrue(fortressButton.GetComponent<AssetController>().Clicked);

        Vector2 positionToPlace = new Vector2(-100, 150);
        MapEditorManager mapEditorManager = GameObject.Find("MapEditorManager")
            .GetComponent<MapEditorManager>();
        mapEditorManager.PaintAtPosition(positionToPlace);
        Assert.AreEqual(1, MapEditorManager.MapObjects.Count);

        GameObject.Find("Selection Tool").GetComponent<Button>().onClick.Invoke();
        Assert.IsTrue(Tool.ToolStatus["Selection Tool"]);

        GameObject placedAsset = GameObject.Find("TempFortressObject(Clone)");
        SelectMapObject.IsTesting = true;
        SelectMapObject.SelectedObject = placedAsset;
        placedAsset.GetComponent<SelectMapObject>().OnPointerClick(new PointerEventData(EventSystem.current));
        Assert.IsTrue(Tool.SelectionOptions.activeSelf);

        Button deleteButton = GameObject.Find("Delete Button").GetComponent<Button>();
        deleteButton.onClick.Invoke();
        int placedObjectId = new List<int>(MapEditorManager.MapObjects.Keys)[0];
        Assert.IsFalse(MapEditorManager.MapObjects[placedObjectId].IsActive);

        SelectMapObject.IsTesting = false;
    }
}
