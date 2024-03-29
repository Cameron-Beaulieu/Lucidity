using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

[TestFixture]
public class TwoDReversionTests {

    [UnitySetUp]
    public IEnumerator SetUp() {
        Util.ResetStaticVariables();
        StartupScreen.FilePath = null;
        MapEditorManager.ReloadFlag = false;
        SceneManager.LoadScene("MapEditor");
        yield return null;
    }

    [UnityTest]
    public IEnumerator ReversionTo2DMapObjectAccuracy() {
        // Paint assets
        PlayModeTestUtil.PaintAnAsset(new Vector3(-100, 150, 0), "Fortress");
        PlayModeTestUtil.PaintAnAsset(new Vector3(100, 150, 0), "House");
        GameObject fortressParent = GameObject.Find("FortressObject Parent");
        GameObject houseParent = GameObject.Find("HouseObject Parent");
        GameObject newFortress = GameObject.Find("FortressObject(Clone)");
        GameObject newHouse = GameObject.Find("HouseObject(Clone)");
        Vector3 fortressPosition = fortressParent.transform.localPosition;
        Vector3 fortressScale = fortressParent.transform.localScale;
        Vector3 housePosition = houseParent.transform.localPosition;
        Vector3 houseScale = houseParent.transform.localScale;
        int fortressId = newFortress.GetInstanceID();
        int houseId = newHouse.GetInstanceID();

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

        // Find new MapObjects
        GameObject newFortressParent = GameObject.Find("FortressObject Parent");
        GameObject newHouseParent = GameObject.Find("HouseObject Parent");

        // Check that the assets are in the right place
        Assert.AreEqual(newFortressParent.transform.localPosition, fortressPosition);
        Assert.AreEqual(newHouseParent.transform.localPosition, housePosition);

        // Check that the assets have the right scale
        Assert.AreEqual(newFortressParent.transform.localScale, fortressScale);
        Assert.AreEqual(newHouseParent.transform.localScale, houseScale); 

        // Check MapObjects Dictionary has correct number of MapObjects in it
        Assert.AreEqual(MapEditorManager.MapObjects.Count, 2);

        // Check if the GameObjects in MapObjects have been replaced
        GameObject revertedFortress = GameObject.Find("FortressObject(Clone)");
        Assert.AreNotEqual(fortressId, revertedFortress.GetInstanceID());
        GameObject revertedHouse = GameObject.Find("HouseObject(Clone)");
        Assert.AreNotEqual(houseId, revertedHouse.GetInstanceID());
    }

    [UnityTest]
    public IEnumerator CanUndoAndRedoAssetDeletionPostReversion() {
        // Paint an asset
        PlayModeTestUtil.PaintAnAsset(new Vector2(-100, 150), "Fortress");
        yield return null;

        // Select the asset and delete it
        GameObject.Find("Selection Tool").GetComponent<Button>().onClick.Invoke();
        GameObject assetToDelete = GameObject.Find("FortressObject(Clone)");
        SelectMapObject.SelectedObject = assetToDelete;
        SelectMapObject.IsTesting = true;
        assetToDelete.GetComponent<SelectMapObject>()
            .OnPointerClick(new PointerEventData(EventSystem.current));
        Button deleteButton = GameObject.Find("Delete Button").GetComponent<Button>();
        deleteButton.onClick.Invoke();

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

        // Undo the deletion
        GameObject revertedGameObject = GameObject.Find("FortressObject(Clone)");
        Assert.Null(revertedGameObject);
        GameObject.Find("Undo").GetComponent<Button>().onClick.Invoke();
        yield return null;
        revertedGameObject = GameObject.Find("FortressObject(Clone)");
        int revertedGameObjectId = revertedGameObject.GetInstanceID();
        Assert.IsTrue(MapEditorManager.MapObjects[revertedGameObjectId].IsActive);

        // Redo the deletion
        GameObject.Find("Redo").GetComponent<Button>().onClick.Invoke();
        Assert.IsFalse(MapEditorManager.MapObjects[revertedGameObjectId].IsActive);

        // Reset testing var
        SelectMapObject.IsTesting = false;
    }

    [UnityTest]
    public IEnumerator CanSwitchToolsAfterReversion() {
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

        // Check if user can change to panning tool
        Button panningToolButton = GameObject.Find("Panning Tool").GetComponent<Button>();
        panningToolButton.onClick.Invoke();
        Assert.IsTrue(Tool.ToolStatus["Panning Tool"]);
        PlayModeTestUtil.CheckAllOtherToolsAreUnselected("Panning Tool");
    }

    [UnityTest]
    public IEnumerator CorrectMapObjectTypesAfterDoubleReversion() {
        // This is a regression test; A bug previously existed where the AddMapObject method was
        // based on the current asset selected in the painting menu. This resulted in reverting
        // twice causing all MapObjects to be changed to the selected type.

        // Paint assets
        PlayModeTestUtil.PaintAnAsset(new Vector3(-100, 150, 0), "Fortress");
        GameObject treeButton = GameObject.Find("TreeButton");
        treeButton.GetComponent<Button>().onClick.Invoke();

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

        // Find new MapObject
        GameObject newFortress = GameObject.Find("FortressObject Parent");

        // Check that the correct asset type has been placed
        Assert.IsNotNull(newFortress);
    }

    [UnityTest]
    public IEnumerator CorrectNumberOfLayersAfterReversion() {
        // This is a regression test, previously after reverting, an extra layer was added
        // that wasn't visible on the UI

        // Paint an asset
        PlayModeTestUtil.PaintAnAsset(new Vector3(-100, 150, 0), "Fortress");

        // add new layer
        GameObject.Find("Layer Tool").GetComponent<Button>().onClick.Invoke();
        yield return null;

        // Paint on the new layer
        PlayModeTestUtil.PaintAnAsset(new Vector3(100, 150, 0), "House");

        // Make sure everything agrees that there are two layers
        Assert.AreEqual(2, MapEditorManager.Layers.Count);
        Assert.AreEqual(2, Layer.LayerNames.Count);
        Assert.AreEqual(2, Layer.LayerStatus.Count);
        Assert.AreEqual(2, Layer.LayerIndex.Count);

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

        // Make sure there are still only two layers
        Assert.AreEqual(2, MapEditorManager.Layers.Count);
        Assert.AreEqual(2, Layer.LayerNames.Count);
        Assert.AreEqual(2, Layer.LayerStatus.Count);
        Assert.AreEqual(2, Layer.LayerIndex.Count);
    }

    [UnityTest]
    public IEnumerator CanToggleLayerVisibilityPostReversion() {
        // confirm current layer is tracking the base layer
        Assert.AreEqual(0, MapEditorManager.CurrentLayer);

        // add an asset to the layer
        PlayModeTestUtil.PaintAnAsset(new Vector2(100, 150), "Fortress");
        // this is the eye with no slash
        Button visibilityOffButton = GameObject.Find("VisibilityEye").GetComponent<Button>();

        // toggle visibility
        visibilityOffButton.onClick.Invoke();

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

        // get new gameobject
        GameObject newFortress = GameObject.Find("FortressObject(Clone)");

        // get new button
        Button visibilityOnButton = GameObject.Find("SlashEye").GetComponent<Button>();

        // check layer and asset are still not visible
        Assert.IsFalse(Layer.LayerVisibility[Layer.LayerNames[0]]);
        Assert.IsFalse(newFortress.GetComponent<Image>().enabled);
        Assert.IsTrue(visibilityOnButton.transform.parent.gameObject.activeSelf);

        // toggle visibility
        visibilityOnButton.onClick.Invoke();

        // get new button
        visibilityOffButton = GameObject.Find("VisibilityEye").GetComponent<Button>();

        // check layer and asset are visible
        Assert.IsTrue(Layer.LayerVisibility[Layer.LayerNames[0]]);
        Assert.IsTrue(newFortress.GetComponent<Image>().enabled);
        Assert.IsTrue(visibilityOffButton.transform.parent.gameObject.activeSelf);
    }
}