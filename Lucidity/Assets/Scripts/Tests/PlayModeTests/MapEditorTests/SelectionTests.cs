using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
        // Check CurrentLayer is tracking base layer and that it is empty
        Assert.AreEqual(0, MapEditorManager.CurrentLayer);
        Assert.Zero(MapEditorManager.MapObjects.Count);

        // paint an object on base layer
        PlayModeTestUtil.PaintAnAsset(new Vector2(-100, 150), "Fortress");
        GameObject placedAsset = GameObject.Find("FortressObject(Clone)");
        Assert.IsTrue(MapEditorManager.Layers[0].ContainsKey(placedAsset.GetInstanceID()));

        // select the object while on base layer
        GameObject.Find("Selection Tool").GetComponent<Button>().onClick.Invoke();
        Assert.AreEqual(0, MapEditorManager.CurrentLayer);
        Assert.IsFalse(Tool.SelectionOptions.activeSelf);
        SelectMapObject.SelectedObject = placedAsset;
        placedAsset.GetComponent<SelectMapObject>()
            .OnPointerClick(new PointerEventData(EventSystem.current));
        Assert.IsTrue(Tool.SelectionOptions.activeSelf);
        Assert.AreEqual("Editing FortressObject(Clone)", 
            GameObject.Find("SelectedObjectLabel").GetComponent<TMPro.TextMeshProUGUI>().text);
        Assert.AreEqual(((Color) new Color32(73, 48, 150, 255)), 
                         placedAsset.GetComponent<Image>().color);

        // create a new layer (should switch to it automatically)
        GameObject.Find("Layer Tool").GetComponent<Button>().onClick.Invoke();      
        yield return null;
        Assert.AreEqual(1, MapEditorManager.CurrentLayer);

        // select the object while on a new layer
        Assert.IsFalse(Tool.SelectionOptions.activeSelf);
        SelectMapObject.SelectedObject = placedAsset;
        placedAsset.GetComponent<SelectMapObject>()
            .OnPointerClick(new PointerEventData(EventSystem.current));
        Assert.IsFalse(Tool.SelectionOptions.activeSelf);
        Assert.AreEqual(Color.white, placedAsset.GetComponent<Image>().color);
    }

    [UnityTest]
    public IEnumerator CanSelectSpawnPointRegardlessOfLayer() {
        // Check CurrentLayer is tracking base layer and that spawnSpoint is not selected
        GameObject spawnPoint = GameObject.Find("Spawn Point");
        Assert.AreEqual(0, MapEditorManager.CurrentLayer);
        Assert.AreEqual(Color.white, spawnPoint.GetComponent<Image>().color);

        // select the spawn point while on base layer
        GameObject.Find("Selection Tool").GetComponent<Button>().onClick.Invoke();
        Assert.IsFalse(Tool.SelectionOptions.activeSelf);
        Assert.IsFalse(Tool.SpawnPointOptions.activeSelf);
        SelectMapObject.SelectedObject = spawnPoint;
        spawnPoint.GetComponent<SelectMapObject>()
            .OnPointerClick(new PointerEventData(EventSystem.current));
        
        // assert spawn point selection options is active and the spawn point is colored purple
        Assert.IsFalse(Tool.SelectionOptions.activeSelf);
        Assert.IsTrue(Tool.SpawnPointOptions.activeSelf);
        Assert.AreEqual("Editing Spawn Point",
            GameObject.Find("SelectedObjectLabel").GetComponent<TMPro.TextMeshProUGUI>().text);
        Assert.AreEqual(((Color) new Color32(73, 48, 150, 255)),
                        spawnPoint.GetComponent<Image>().color);
        
        // add new layer (switches to it automatically)
        GameObject.Find("Layer Tool").GetComponent<Button>().onClick.Invoke();
        yield return null;
        Assert.AreEqual(1, MapEditorManager.CurrentLayer);
        Assert.IsFalse(Tool.SpawnPointOptions.activeSelf);
        Assert.AreEqual(Color.white, 
                         spawnPoint.GetComponent<Image>().color);

        // select the spawn point while on a new layer
        SelectMapObject.SelectedObject = spawnPoint;
        spawnPoint.GetComponent<SelectMapObject>()
            .OnPointerClick(new PointerEventData(EventSystem.current));
        Assert.IsTrue(Tool.SpawnPointOptions.activeSelf);
        Assert.AreEqual("Editing Spawn Point",
            GameObject.Find("SelectedObjectLabel").GetComponent<TMPro.TextMeshProUGUI>().text);
        Assert.AreEqual(((Color) new Color32(73, 48, 150, 255)), 
                        spawnPoint.GetComponent<Image>().color);
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

        // check asset is in <c>Layers</c>
        Assert.AreEqual(1, MapEditorManager.Layers[MapEditorManager.CurrentLayer].Count);
        
        // select the object
        GameObject.Find("Selection Tool").GetComponent<Button>().onClick.Invoke();
        GameObject placedAsset = GameObject.Find("FortressObject(Clone)");
        SelectMapObject.SelectedObject = placedAsset;
        placedAsset.GetComponent<SelectMapObject>()
            .OnPointerClick(new PointerEventData(EventSystem.current));

        // delete the object
        Button deleteButton = GameObject.Find("Delete Button").GetComponent<Button>();
        deleteButton.onClick.Invoke();
        int placedObjectId = new List<int>(MapEditorManager.MapObjects.Keys)[0];
        // check asset is in <c>Layers</c>
        Assert.AreEqual(0, MapEditorManager.Layers[MapEditorManager.CurrentLayer].Count);
        Assert.IsFalse(MapEditorManager.MapObjects[placedObjectId].IsActive);
    }

    [UnityTest]
    public IEnumerator CanRotateAsset() {
        // paint an object to rotate
        PlayModeTestUtil.PaintAnAsset(new Vector2(-100, 150), "Fortress");

        // select the object
        GameObject.Find("Selection Tool").GetComponent<Button>().onClick.Invoke();
        GameObject placedAssetParent = GameObject.Find("FortressObject Parent");
        GameObject placedAsset = placedAssetParent.transform.GetChild(0).gameObject;
        SelectMapObject.SelectedObject = placedAsset;
        placedAsset.GetComponent<SelectMapObject>()
            .OnPointerClick(new PointerEventData(EventSystem.current));
        Assert.AreEqual(0, placedAssetParent.transform.rotation.eulerAngles.z);
        
        // rotate the object clockwise
        Button clockwiseButton = GameObject.Find("CWButton").GetComponent<Button>();
        clockwiseButton.onClick.Invoke();
        yield return null;
        Assert.AreEqual(270, placedAssetParent.transform.rotation.eulerAngles.z);

        // rotate the object counter-clockwise
        Button counterClockwiseButton = GameObject.Find("CCWButton").GetComponent<Button>();
        counterClockwiseButton.onClick.Invoke();
        yield return null;
        Assert.AreEqual(0, placedAssetParent.transform.rotation.eulerAngles.z);
    }

    [UnityTest]
    public IEnumerator CanScaleAsset() {
        Vector3 defaultScale = new Vector3(Util.ParentAssetDefaultScale, 
                                           Util.ParentAssetDefaultScale, 
                                           Util.ParentAssetDefaultScale);
        // paint an object to scale
        PlayModeTestUtil.PaintAnAsset(new Vector2(-100, 150), "Fortress");

        // select the object
        GameObject.Find("Selection Tool").GetComponent<Button>().onClick.Invoke();
        GameObject placedAssetParent = GameObject.Find("FortressObject Parent");
        GameObject placedAsset = placedAssetParent.transform.GetChild(0).gameObject;
        SelectMapObject.SelectedObject = placedAsset;
        placedAsset.GetComponent<SelectMapObject>()
            .OnPointerClick(new PointerEventData(EventSystem.current));
        Assert.AreEqual(defaultScale, placedAssetParent.transform.localScale);
        yield return null;

        // check that Asset Options displays the default scale
        TMP_Text scaleValueText = GameObject.Find("ValueText").GetComponent<TMP_Text>();
        Assert.AreEqual("1x", scaleValueText.text);

        // scale the object up
        ResizeMapObject scaleScript = GameObject.Find("ScaleContainer/Slider")
            .GetComponent<ResizeMapObject>();
        scaleScript.OnValueChanged(2f);
        Assert.AreEqual("2x", scaleValueText.text);
        Assert.AreEqual(defaultScale * 2, placedAssetParent.transform.localScale);
    }
}
