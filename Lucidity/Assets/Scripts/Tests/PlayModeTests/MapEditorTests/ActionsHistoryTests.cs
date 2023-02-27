using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

[TestFixture] 
[Category("Map Editor Tests")]
public class ActionsHistoryTests : MapEditorTests {
    [Test]
    public void CanUndoAndRedoAssetPlacement() {
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

        Button undoButton = GameObject.Find("Undo").GetComponent<Button>();
        undoButton.onClick.Invoke();
        int placedObjectId = new List<int>(MapEditorManager.MapObjects.Keys)[0];
        Assert.IsFalse(MapEditorManager.MapObjects[placedObjectId].IsActive);

        Button redoButton = GameObject.Find("Redo").GetComponent<Button>();
        redoButton.onClick.Invoke();
        Assert.IsTrue(MapEditorManager.MapObjects[placedObjectId].IsActive);
    }
}
