using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

[TestFixture]
public class DynamicBoundingBoxTests : MapEditorTests {

    [SetUp]
    public void DynamicBoundingBoxSetUp() {
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

    /// <summary>
    /// Calculates the factorial of a given number.
    /// </summary>
    /// <param name="i">
    /// <c>int</c> to calculate the factorial for
    /// </param>
    /// <returns>
    /// The computed factorial of the input number
    /// </returns>
    private int factorial(int i) {
        int result = 1;
        while (i > 0) {
            result *= i;
            i--;
        }
        return result;
    }

    [Test]
    public void DynamicBoundingBoxVariationHasCorrectNumber() {
        Button fortressButton = GameObject.Find("FortressButton").GetComponent<Button>();
        fortressButton.onClick.Invoke();

        // set the number of assets to place equal to 7
        InputField countInput = GameObject.Find("CountInput").GetComponent<InputField>();
        countInput.text = "7";
        countInput.onEndEdit.Invoke(countInput.text);
        Assert.AreEqual(7, AssetOptions.AssetCount);        
        Assert.AreEqual(DynamicBoundingBox.DynamicSideLength, 3);

        // verify the correct number of variations are available: 9 choose 7
        int variations = (factorial(9)) / (factorial(7) * factorial(9 - 7));
        Assert.AreEqual(DynamicBoundingBox.AssetArrangements.Count, variations);
        Text maximumText = GameObject.Find("VariationMaximumText").GetComponent<Text>();
        Assert.AreEqual(int.Parse(maximumText.text.Replace("Max: ", "")), variations);
    }

    [Test]
    public void DynamicBoundingBoxVariationInputChangesHover() {
        Button fortressButton = GameObject.Find("FortressButton").GetComponent<Button>();
        fortressButton.onClick.Invoke();
        
        // set the number of assets to place equal to 2
        InputField countInput = GameObject.Find("CountInput").GetComponent<InputField>();
        countInput.text = "2";
        countInput.onEndEdit.Invoke(countInput.text);
        Assert.AreEqual(2, AssetOptions.AssetCount); 

        // save the original variation
        List<Vector2> var1 = DynamicBoundingBox.AssetArrangements[AssetOptions.Variation];

        // change to a new variation
        InputField variationInput = GameObject.Find("VariationInput").GetComponent<InputField>();
        variationInput.text = "2";
        variationInput.onEndEdit.Invoke(variationInput.text);
        Assert.AreEqual(1, AssetOptions.Variation);

        // verify that the old variation and the newly selected one are not the same
        Assert.AreNotEqual(var1, DynamicBoundingBox.AssetArrangements[AssetOptions.Variation]);

        // revert the variation and verify that the variation is the same
        variationInput.text = "1";
        variationInput.onEndEdit.Invoke(variationInput.text);
        Assert.AreEqual(0, AssetOptions.Variation);
        Assert.AreEqual(var1, DynamicBoundingBox.AssetArrangements[AssetOptions.Variation]);
    }

    [UnityTest]
    public IEnumerator DynamicBoundingBoxAssetHoverUpdatesAfterReversion() {
        // 3D-ify
        GameObject.Find("3D-ify Button").GetComponent<Button>().onClick.Invoke();
        yield return null;
        Assert.AreEqual("3DMap", SceneManager.GetActiveScene().name);
        yield return new WaitForEndOfFrame();

        // open up options menu
        Render3DScene.EscapeTestingInput = true;
        yield return null;

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