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

    [TearDown]
    public void TearDown() {
        Util.ResetStaticVariables();
    }

    [UnityTest]
    public IEnumerator ProperAssetPlacementDuringReversion() {
        // Paint assets
        PlayModeTestUtil.PaintAnAsset(new Vector3(-100, 150, 0), "Fortress");
        PlayModeTestUtil.PaintAnAsset(new Vector3(100, 150, 0), "House");
        GameObject fortressParent = GameObject.Find("TempFortressObject Parent");
        GameObject houseParent = GameObject.Find("TempHouseObject Parent");
        Vector3 fortressPosition = fortressParent.transform.localPosition;
        Vector3 fortressScale = fortressParent.transform.localScale;
        Vector3 housePosition = houseParent.transform.localPosition;
        Vector3 houseScale = houseParent.transform.localScale;

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

        // Find new MapObjects
        GameObject newFortressParent = GameObject.Find("TempFortressObject Parent");
        GameObject newHouseParent = GameObject.Find("TempHouseObject Parent");

        // Check that the assets are in the right place
        Assert.AreEqual(newFortressParent.transform.localPosition, fortressPosition);
        Assert.AreEqual(newHouseParent.transform.localPosition, housePosition);

        // Check that the assets have the right scale
        Assert.AreEqual(newFortressParent.transform.localScale, fortressScale);
        Assert.AreEqual(newHouseParent.transform.localScale, houseScale); 
    }

    public IEnumerator ProperMapObjectDictionaryRebuilding(){
        // Paint assets
        PlayModeTestUtil.PaintAnAsset(new Vector2(-100, 150), "Fortress");
        GameObject newFortress = GameObject.Find("TempFortressObject(Clone)");
        int fortressId = newFortress.GetInstanceID();

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

        // Check MapObjects Dictionary has correct number of MapObjects in it
        Assert.AreEqual(MapEditorManager.MapObjects.Count, 1);

        // Check if the GameObjects in MapObjects have been replaced
        GameObject revertedGameObject = GameObject.Find("TempFortressObject(Clone)");
        Assert.AreNotEqual(fortressId, revertedGameObject.GetInstanceID());
    }

    [UnityTest]
    public IEnumerator CanUndoAndRedoAssetDeletionPostReversion() {
        // Paint an asset
        PlayModeTestUtil.PaintAnAsset(new Vector2(-100, 150), "Fortress");

        // Select the asset and delete it
        GameObject.Find("Selection Tool").GetComponent<Button>().onClick.Invoke();
        GameObject assetToDelete = GameObject.Find("TempFortressObject(Clone)");
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

        // Revert to 2D
        GameObject.Find("BackButton").GetComponent<Button>().onClick.Invoke();
        yield return null;
        Assert.AreEqual("MapEditor", SceneManager.GetActiveScene().name);
        yield return new WaitForEndOfFrame();

        // Undo the deletion
        GameObject revertedGameObject = GameObject.Find("TempFortressObject(Clone)");
        Assert.Null(revertedGameObject);
        GameObject.Find("Undo").GetComponent<Button>().onClick.Invoke();
        revertedGameObject = GameObject.Find("TempFortressObject(Clone)");
        int revertedGameObjectId = revertedGameObject.GetInstanceID();
        Assert.IsTrue(MapEditorManager.MapObjects[revertedGameObjectId].IsActive);

        // Redo the deletion
        GameObject.Find("Redo").GetComponent<Button>().onClick.Invoke();
        Assert.IsFalse(MapEditorManager.MapObjects[revertedGameObjectId].IsActive);

        // Reset testing var
        SelectMapObject.IsTesting = false;
    }

    [UnityTest]
    public IEnumerator CanSwitchToolsAfterReversion(){
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


        // Check if user can change to panning tool
        Button panningToolButton = GameObject.Find("Panning Tool").GetComponent<Button>();
        panningToolButton.onClick.Invoke();
        Assert.IsTrue(Tool.ToolStatus["Panning Tool"]);
        PlayModeTestUtil.CheckAllOtherToolsAreUnselected("Panning Tool");
        Assert.IsFalse(Tool.SelectionMenu.activeSelf);
        Assert.IsFalse(Tool.PaintingMenu.activeSelf);
    }
    
}