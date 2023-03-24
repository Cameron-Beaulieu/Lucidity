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

        // verify that the set of valid and invalid assets are disjoint, and complete the entire
        // grid
        Assert.AreEqual(2, DynamicBoundingBox.AssetArrangement.Count);

        HashSet<Vector2> validHoverAssets = new HashSet<Vector2>(DynamicBoundingBox.AssetArrangement);
        validHoverAssets.UnionWith(DynamicBoundingBox.InvalidArrangement);
        Assert.AreEqual(Mathf.Pow(DynamicBoundingBox.DynamicSideLength, 2), validHoverAssets.Count);

        validHoverAssets = new HashSet<Vector2>(DynamicBoundingBox.AssetArrangement);
        validHoverAssets.IntersectWith(DynamicBoundingBox.InvalidArrangement);
        Assert.AreEqual(0, validHoverAssets.Count);
    }

    [UnityTest]
    public IEnumerator DynamicBoundingBoxAssetHoverUpdatesOnSpreadChange() {
        Button fortressButton = GameObject.Find("FortressButton").GetComponent<Button>();
        Slider spreadSlider = GameObject.Find("SpreadContainer").transform.Find("Slider")
            .GetComponent<Slider>();
        fortressButton.onClick.Invoke();
        // the local scale of the hovering dynamic bounding box is equal to the slider input
        Assert.AreEqual(Vector3.one,
                        GameObject.Find("HoverDynamicBoundingBoxObject").transform.localScale);
        // change the slider input and ensure it is reflected in the hover object scale
        spreadSlider.value = 10f;
        yield return null;
        Assert.AreEqual(Vector3.one * 2,
                        GameObject.Find("HoverDynamicBoundingBoxObject").transform.localScale);
    }

    [Test]
    public void DynamicBoundingBoxRandomnessControlledByToggle() {
        Button fortressButton = GameObject.Find("FortressButton").GetComponent<Button>();
        Toggle randomToggle = GameObject.Find("RandomContainer").transform.Find("RandomToggle")
            .GetComponent<Toggle>();
        
        // the Random variable in AssetOptions should correspond to the toggle option
        randomToggle.isOn = true;
        Assert.AreEqual(randomToggle.isOn, AssetOptions.Random);

        // change the asset count to a larger value to verify uniform arrangement
        randomToggle.isOn = false;
        InputField countInput = GameObject.Find("CountInput").GetComponent<InputField>();
        countInput.text = "5";
        countInput.onEndEdit.Invoke(countInput.text);
        Assert.AreEqual(5, AssetOptions.AssetCount);

        fortressButton.onClick.Invoke();

        // simulate the uniform placement of 5 assets, which goes top-bottom and left-right
        HashSet<Vector2> fiveUniformAssets = new HashSet<Vector2>();
        fiveUniformAssets.Add(new Vector2(0, 2));
        fiveUniformAssets.Add(new Vector2(1, 2));
        fiveUniformAssets.Add(new Vector2(2, 2));
        fiveUniformAssets.Add(new Vector2(0, 1));
        fiveUniformAssets.Add(new Vector2(1, 1));
        Assert.AreEqual(DynamicBoundingBox.AssetArrangement, fiveUniformAssets);
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
        Slider spreadSlider = GameObject.Find("SpreadContainer").transform.Find("Slider")
            .GetComponent<Slider>();
        fortressButton.onClick.Invoke();
        // the local scale of the hovering dynamic bounding box is equal to the slider input
        Assert.AreEqual(Vector3.one,
                        GameObject.Find("HoverDynamicBoundingBoxObject").transform.localScale);
        // change the slider input and ensure it is reflected in the hover object scale
        spreadSlider.value = 10f;
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