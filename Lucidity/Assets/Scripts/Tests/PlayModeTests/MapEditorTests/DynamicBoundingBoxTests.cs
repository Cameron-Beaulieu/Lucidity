using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

[TestFixture]
public class DynamicBoundingBoxTests : MapEditorTests {

    [UnitySetUp]
    public IEnumerator DynamicBoundingBoxSetUp() {
        Util.ResetStaticVariables();
        StartupScreen.FilePath = null;
        MapEditorManager.ReloadFlag = false;
        SceneManager.LoadScene("MapEditor");
        yield return null;
        Util.ResetAssetButtons();
    }

    [Test]
    public void DynamicBoundingBoxAssetHoverHasCorrectCount() {
        Button fortressButton = GameObject.Find("FortressButton").GetComponent<Button>();
        InputField countInput = GameObject.Find("CountInput").GetComponent<InputField>();
        countInput.text = "2";
        countInput.onEndEdit.Invoke(countInput.text);
        Assert.AreEqual(2, AssetOptions.AssetCount);
        fortressButton.onClick.Invoke();
        Assert.IsTrue(fortressButton.GetComponent<AssetController>().Clicked);

        // the dynamic bounding box parent on the hovering assets is also given the tag AssetImage
        Assert.AreEqual(3, GameObject.FindGameObjectsWithTag("AssetImage").Length);
    }

    [UnityTest]
    public IEnumerator DynamicBoundingBoxAssetHoverUpdatesOnBrushSizeChange() {
        Button fortressButton = GameObject.Find("FortressButton").GetComponent<Button>();
        Slider brushSizeSlider = GameObject.Find("BrushSizeContainer").transform.Find("Slider")
            .GetComponent<Slider>();
        fortressButton.onClick.Invoke();
        // the local scale of the hovering dynamic bounding box is equal to the slider input
        Assert.AreEqual(Vector3.one,
                        GameObject.Find("HoverDynamicBoundingBoxObject").transform.localScale);
        // change the slider input and ensure it is reflected in the hover object scale
        brushSizeSlider.value = 10f;
        yield return null;
        Assert.AreEqual(Vector3.one * 2,
                        GameObject.Find("HoverDynamicBoundingBoxObject").transform.localScale);
    }

    [UnityTest]
    public IEnumerator DynamicBoundingBoxAssetHoverUpdatesAfterReversion() {
        // 3D-ify
        GameObject.Find("3D-ify Button").GetComponent<Button>().onClick.Invoke();
        yield return null;
        Assert.AreEqual("3DMap", SceneManager.GetActiveScene().name);
        yield return new WaitForEndOfFrame();

        // Revert to 2D
        GameObject.Find("BackButton").GetComponent<Button>().onClick.Invoke();
        yield return null;
        Assert.AreEqual("MapEditor", SceneManager.GetActiveScene().name);
        yield return new WaitForEndOfFrame();
        
        Button fortressButton = GameObject.Find("FortressButton").GetComponent<Button>();
        Slider brushSizeSlider = GameObject.Find("BrushSizeContainer").transform.Find("Slider")
            .GetComponent<Slider>();
        fortressButton.onClick.Invoke();
        // the local scale of the hovering dynamic bounding box is equal to the slider input
        Assert.AreEqual(Vector3.one,
                        GameObject.Find("HoverDynamicBoundingBoxObject").transform.localScale);
        // change the slider input and ensure it is reflected in the hover object scale
        brushSizeSlider.value = 10f;
        yield return null;
        Assert.AreEqual(Vector3.one * 2,
                        GameObject.Find("HoverDynamicBoundingBoxObject").transform.localScale);
    }

    [UnityTest]
    public IEnumerator DynamicBoundingBoxDeletesOnRelease() {
        // place the first asset
        Button fortressButton = GameObject.Find("FortressButton").GetComponent<Button>();
        fortressButton.onClick.Invoke();
        Assert.IsTrue(fortressButton.GetComponent<AssetController>().Clicked);
        Vector2 positionToPlace = new Vector2(-100, 150);
        MapEditorManager mapEditorManager = GameObject.Find("MapEditorManager")
            .GetComponent<MapEditorManager>();
        Assert.AreEqual(0, GameObject.FindGameObjectsWithTag("DynamicBoundingBox").Length);
        mapEditorManager.PaintAtPosition(positionToPlace);
        Assert.AreEqual(1, GameObject.FindGameObjectsWithTag("DynamicBoundingBox").Length);

        // skip frame to allow for dynamic bounding boxes to be deleted
        yield return new WaitForFixedUpdate();
        yield return null;
        Assert.AreEqual(0, GameObject.FindGameObjectsWithTag("DynamicBoundingBox").Length);
    }

    [Test]
    public void DynamicBoundingBoxPersistsOnHold() {
        // place the first asset
        Button fortressButton = GameObject.Find("FortressButton").GetComponent<Button>();
        fortressButton.onClick.Invoke();
        Assert.IsTrue(fortressButton.GetComponent<AssetController>().Clicked);
        Vector2 positionToPlace = new Vector2(-100, 150);
        MapEditorManager mapEditorManager = GameObject.Find("MapEditorManager")
            .GetComponent<MapEditorManager>();
        Assert.AreEqual(0, GameObject.FindGameObjectsWithTag("DynamicBoundingBox").Length);
        mapEditorManager.PaintAtPosition(positionToPlace);
        mapEditorManager.PaintAtPosition(positionToPlace + new Vector2(100f, 100f));
        Assert.AreEqual(2, GameObject.FindGameObjectsWithTag("DynamicBoundingBox").Length);
    }
}