using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TestTools;
using UnityEngine.UI;

[TestFixture] 
public class ActionsHistoryTests : MapEditorTests {

    [Test]
    public void CanUndoAndRedoAssetPlacement() {
        // paint an asset
        Assert.Zero(MapEditorManager.MapObjects.Count);
        PlayModeTestUtil.PaintAnAsset(new Vector2(-100, 150), "Fortress");
        Assert.AreEqual(1, MapEditorManager.MapObjects.Count);
        int placedObjectId = new List<int>(MapEditorManager.MapObjects.Keys)[0];
        Assert.IsTrue(MapEditorManager.MapObjects[placedObjectId].IsActive);

        // undo the placement
        Button undoButton = GameObject.Find("Undo").GetComponent<Button>();
        undoButton.onClick.Invoke();
        Assert.IsFalse(MapEditorManager.MapObjects[placedObjectId].IsActive);

        // redo the placement
        Button redoButton = GameObject.Find("Redo").GetComponent<Button>();
        redoButton.onClick.Invoke();
        Assert.IsTrue(MapEditorManager.MapObjects[placedObjectId].IsActive);
    }

    [Test]
    public void CanUndoAndRedoAssetDeletion() {
        // paint an asset
        Assert.Zero(MapEditorManager.MapObjects.Count);
        PlayModeTestUtil.PaintAnAsset(new Vector2(-100, 150), "Fortress");
        Assert.AreEqual(1, MapEditorManager.MapObjects.Count);
        int placedObjectId = new List<int>(MapEditorManager.MapObjects.Keys)[0];
        Assert.IsTrue(MapEditorManager.MapObjects[placedObjectId].IsActive);

        // select the asset and delete it
        GameObject.Find("Selection Tool").GetComponent<Button>().onClick.Invoke();
        GameObject assetToDelete = GameObject.Find("TempFortressObject(Clone)");
        SelectMapObject.SelectedObject = assetToDelete;
        SelectMapObject.IsTesting = true;
        assetToDelete.GetComponent<SelectMapObject>()
            .OnPointerClick(new PointerEventData(EventSystem.current));
        Button deleteButton = GameObject.Find("Delete Button").GetComponent<Button>();
        deleteButton.onClick.Invoke();
        Assert.IsFalse(MapEditorManager.MapObjects[placedObjectId].IsActive);

        // undo the deletion
        GameObject.Find("Undo").GetComponent<Button>().onClick.Invoke();
        Assert.IsTrue(MapEditorManager.MapObjects[placedObjectId].IsActive);

        // redo the deletion
        GameObject.Find("Redo").GetComponent<Button>().onClick.Invoke();
        Assert.IsFalse(MapEditorManager.MapObjects[placedObjectId].IsActive);

        // reset testing var
        SelectMapObject.IsTesting = false;
    }

    [UnityTest]
    public IEnumerator CanUndoAndRedoLayerCreation() {
        MapEditorManager editor = GameObject.Find("MapEditorManager")
            .GetComponent<MapEditorManager>();
        Assert.AreEqual(0, editor.CurrentLayer);

        // create a layer
        Assert.AreEqual(1, MapEditorManager.Layers.Count);
        GameObject.Find("Layer Tool").GetComponent<Button>().onClick.Invoke();
        Assert.AreEqual(2, MapEditorManager.Layers.Count);
        yield return null;
        
        // assert that the new layer is selected
        Assert.AreEqual(1, editor.CurrentLayer);

        // undo the creation
        GameObject.Find("Undo").GetComponent<Button>().onClick.Invoke();
        yield return null;
        Assert.AreEqual(1, MapEditorManager.Layers.Count);
        Assert.AreEqual(0, editor.CurrentLayer);

        // redo the creation
        GameObject.Find("Redo").GetComponent<Button>().onClick.Invoke();
        yield return null;
        Assert.AreEqual(2, MapEditorManager.Layers.Count);
        Assert.AreEqual(1, editor.CurrentLayer);
    }

    [Test]
    public void PermanentlyDeleteActionsThatCannotBeRedone() {
        // paint an asset
        PlayModeTestUtil.PaintAnAsset(new Vector2(-100, 150), "Fortress");
        int placedObjectId = new List<int>(MapEditorManager.MapObjects.Keys)[0];

        // undo the placement
        GameObject.Find("Undo").GetComponent<Button>().onClick.Invoke();

        // paint another asset
        PlayModeTestUtil.PaintAnAsset(new Vector2(100, 150), "Fortress");

        // check that the original asset painted is permanently deleted
        Assert.IsFalse(MapEditorManager.MapObjects.ContainsKey(placedObjectId));
    }


}
