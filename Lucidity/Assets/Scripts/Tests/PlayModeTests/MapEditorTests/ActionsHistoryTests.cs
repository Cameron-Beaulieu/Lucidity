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
        Assert.Zero(MapEditorManager.Layers[MapEditorManager.CurrentLayer].Count);
        PlayModeTestUtil.PaintAnAsset(new Vector2(-100, 150), "Fortress");
        Assert.AreEqual(1, MapEditorManager.Layers[MapEditorManager.CurrentLayer].Count);
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
        Assert.AreEqual(0, MapEditorManager.Layers[MapEditorManager.CurrentLayer].Count);

        // undo the deletion
        GameObject.Find("Undo").GetComponent<Button>().onClick.Invoke();
        Assert.IsTrue(MapEditorManager.MapObjects[placedObjectId].IsActive);
        Assert.AreEqual(1, MapEditorManager.Layers[MapEditorManager.CurrentLayer].Count);

        // redo the deletion
        GameObject.Find("Redo").GetComponent<Button>().onClick.Invoke();
        Assert.IsFalse(MapEditorManager.MapObjects[placedObjectId].IsActive);
        Assert.AreEqual(0, MapEditorManager.Layers[MapEditorManager.CurrentLayer].Count);

        // reset testing var
        SelectMapObject.IsTesting = false;
    }

    [UnityTest]
    public IEnumerator CanUndoAndRedoLayerCreation() {
        // Confirm current layer is tracking the base layer
        Assert.AreEqual(0, MapEditorManager.CurrentLayer);

        // create a layer
        Assert.AreEqual(1, MapEditorManager.Layers.Count);
        GameObject.Find("Layer Tool").GetComponent<Button>().onClick.Invoke();
        Assert.AreEqual(2, MapEditorManager.Layers.Count);
        yield return null;
        
        // assert that the new layer is selected
        Assert.AreEqual(1, MapEditorManager.CurrentLayer);

        // undo the creation
        GameObject.Find("Undo").GetComponent<Button>().onClick.Invoke();
        yield return null;
        Assert.AreEqual(2, MapEditorManager.Layers.Count);
        Assert.AreEqual(0, MapEditorManager.CurrentLayer);

        // redo the creation
        GameObject.Find("Redo").GetComponent<Button>().onClick.Invoke();
        yield return null;
        Assert.AreEqual(2, MapEditorManager.Layers.Count);
        Assert.AreEqual(1, MapEditorManager.CurrentLayer);
    }

    [UnityTest]
    public IEnumerator CanUndoAndRedoLayerDeletion() {
        // Confirm current layer is tracking the base layer
        Assert.AreEqual(0, MapEditorManager.CurrentLayer);

        // create a layer
        Assert.AreEqual(1, MapEditorManager.Layers.Count);
        GameObject.Find("Layer Tool").GetComponent<Button>().onClick.Invoke();
        yield return null;
        Assert.AreEqual(2, MapEditorManager.Layers.Count);
        GameObject layer = GameObject.Find("Layer1");
        
        // assert that the new layer is selected
        Assert.AreEqual(1, MapEditorManager.CurrentLayer);

        // add an asset to the layer
        PlayModeTestUtil.PaintAnAsset(new Vector2(100, 150), "Fortress");
        Assert.AreEqual(1, MapEditorManager.Layers[1].Count);

        // delete layer
        layer.transform.GetChild(2).gameObject.GetComponent<Button>().onClick.Invoke();

        // undo deletion
        GameObject.Find("Undo").GetComponent<Button>().onClick.Invoke();
        yield return null;
        Assert.IsTrue(layer.activeSelf);
        Assert.AreEqual(1, MapEditorManager.CurrentLayer);

        // redo the deletion
        GameObject.Find("Redo").GetComponent<Button>().onClick.Invoke();
        yield return null;
        Assert.IsFalse(layer.activeSelf);
        Assert.AreEqual(0, MapEditorManager.CurrentLayer);
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

    [UnityTest]
    public IEnumerator CanUndoAndRedoAssetScaling() {
        SelectMapObject.IsTesting = true;
        Vector3 defaultScale = new Vector3(Util.ParentAssetDefaultScale,
                                           Util.ParentAssetDefaultScale, 
                                           Util.ParentAssetDefaultScale);
                    
        // paint an asset
        PlayModeTestUtil.PaintAnAsset(new Vector2(-100, 150), "Fortress");
        GameObject parentToScale = GameObject.Find("FortressObject Parent");
        GameObject child = parentToScale.transform.GetChild(0).gameObject;

        // select the asset
        GameObject.Find("Selection Tool").GetComponent<Button>().onClick.Invoke();
        SelectMapObject.SelectedObject = child;
        child.GetComponent<SelectMapObject>()
            .OnPointerClick(new PointerEventData(EventSystem.current));
        Assert.AreEqual(defaultScale, parentToScale.transform.localScale);
        yield return null;

        // scale the asset
        ResizeMapObject scaleScript = GameObject.Find("ScaleContainer/Slider")
            .GetComponent<ResizeMapObject>();
        scaleScript.OnValueChanged(2f);
        scaleScript.OnPointerUp(new PointerEventData(EventSystem.current));
        Assert.AreEqual(defaultScale * 2, parentToScale.transform.localScale);
        Assert.AreEqual(EditorAction.ActionType.ResizeMapObject, 
                        MapEditorManager.CurrentAction.Value.Type);

        // undo the scaling
        GameObject.Find("Undo").GetComponent<Button>().onClick.Invoke();
        yield return null;
        Assert.AreEqual(defaultScale, parentToScale.transform.localScale);

        // redo the scaling
        GameObject.Find("Redo").GetComponent<Button>().onClick.Invoke();
        yield return null;
        Assert.AreEqual(defaultScale * 2, parentToScale.transform.localScale);
        SelectMapObject.IsTesting = false;
    }

    [UnityTest]
    public IEnumerator CanUndoAndRedoAssetRotation() {
        SelectMapObject.IsTesting = true;

        // paint an asset
        PlayModeTestUtil.PaintAnAsset(new Vector2(-100, 150), "Fortress");
        GameObject parentToRotate = GameObject.Find("FortressObject Parent");
        GameObject child = parentToRotate.transform.GetChild(0).gameObject;
        Assert.AreEqual(0, parentToRotate.transform.rotation.eulerAngles.z);

        // select the asset
        GameObject.Find("Selection Tool").GetComponent<Button>().onClick.Invoke();
        SelectMapObject.SelectedObject = child;
        child.GetComponent<SelectMapObject>()
            .OnPointerClick(new PointerEventData(EventSystem.current));
        Assert.AreEqual(0, parentToRotate.transform.rotation.eulerAngles.z);
        yield return null;

        // rotate the asset
        Button clockwiseButton = GameObject.Find("CWButton").GetComponent<Button>();
        clockwiseButton.onClick.Invoke();
        yield return new WaitForFixedUpdate();
        Assert.AreEqual(270, parentToRotate.transform.rotation.eulerAngles.z);

        // undo the rotation
        GameObject.Find("Undo").GetComponent<Button>().onClick.Invoke();
        yield return new WaitForFixedUpdate();
        Assert.AreEqual(0, parentToRotate.transform.rotation.eulerAngles.z);

        // redo the rotation
        GameObject.Find("Redo").GetComponent<Button>().onClick.Invoke();
        yield return new WaitForFixedUpdate();
        Assert.AreEqual(270, parentToRotate.transform.rotation.eulerAngles.z);
        SelectMapObject.IsTesting = false;
    }
}
