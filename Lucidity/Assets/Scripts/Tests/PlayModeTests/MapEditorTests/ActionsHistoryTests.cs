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
        GameObject assetToDelete = GameObject.Find("FortressObject(Clone)");
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
        // Confirm current layer is tracking the base layer
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
        Assert.AreEqual(2, MapEditorManager.Layers.Count);
        Assert.AreEqual(0, editor.CurrentLayer);

        // redo the creation
        GameObject.Find("Redo").GetComponent<Button>().onClick.Invoke();
        yield return null;
        Assert.AreEqual(2, MapEditorManager.Layers.Count);
        Assert.AreEqual(1, editor.CurrentLayer);
    }

    [UnityTest]
    public IEnumerator CanUndoAndRedoLayerDeletion() {
        // Confirm current layer is tracking the base layer
        MapEditorManager editor = GameObject.Find("MapEditorManager")
            .GetComponent<MapEditorManager>();
        Assert.AreEqual(0, editor.CurrentLayer);

        // create a layer
        Assert.AreEqual(1, MapEditorManager.Layers.Count);
        GameObject.Find("Layer Tool").GetComponent<Button>().onClick.Invoke();
        yield return null;
        Assert.AreEqual(2, MapEditorManager.Layers.Count);
        GameObject layer = GameObject.Find("Layer1");
        
        // assert that the new layer is selected
        Assert.AreEqual(1, editor.CurrentLayer);

        // add an asset to the layer
        PlayModeTestUtil.PaintAnAsset(new Vector2(100, 150), "Fortress");
        Assert.AreEqual(1, MapEditorManager.Layers[1].Count);

        // delete layer
        layer.transform.GetChild(2).gameObject.GetComponent<Button>().onClick.Invoke();

        // undo deletion
        GameObject.Find("Undo").GetComponent<Button>().onClick.Invoke();
        yield return null;
        Assert.IsTrue(layer.activeSelf);
        Assert.AreEqual(1, editor.CurrentLayer);

        // redo the deletion
        GameObject.Find("Redo").GetComponent<Button>().onClick.Invoke();
        yield return null;
        Assert.IsFalse(layer.activeSelf);
        Assert.AreEqual(0, editor.CurrentLayer);
    }

    [UnityTest]
    public IEnumerator PermanentlyDeleteActionsThatCannotBeRedone() {
        // paint an asset
        PlayModeTestUtil.PaintAnAsset(new Vector2(-100, 150), "Fortress");
        int placedObjectId = new List<int>(MapEditorManager.MapObjects.Keys)[0];

        // undo the placement
        GameObject.Find("Undo").GetComponent<Button>().onClick.Invoke();

        // paint another asset
        PlayModeTestUtil.PaintAnAsset(new Vector2(100, 150), "Fortress");

        // check that the original asset painted is permanently deleted
        Assert.IsFalse(MapEditorManager.MapObjects.ContainsKey(placedObjectId));

        // create a layer
        Assert.AreEqual(1, MapEditorManager.Layers.Count);
        GameObject.Find("Layer Tool").GetComponent<Button>().onClick.Invoke();
        yield return null;
        Assert.AreEqual(2, MapEditorManager.Layers.Count);

        // undo the creation
        GameObject.Find("Undo").GetComponent<Button>().onClick.Invoke();
        Assert.AreEqual(2, MapEditorManager.Layers.Count);

        // create a new layer
        GameObject.Find("Layer Tool").GetComponent<Button>().onClick.Invoke();
        
        // check that the original new layer was permantently deleted
        Assert.AreEqual(2, MapEditorManager.Layers.Count);
    }
}
