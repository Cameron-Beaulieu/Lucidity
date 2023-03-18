using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

[TestFixture]
public class MiscellaneousTests : MapEditorTests {

    [Test]
    public void MapHasSpawnPoint() {
        // Checking if the map has a spawn point
        GameObject spawnPoint = GameObject.Find("Spawn Point");
        Assert.IsNotNull(spawnPoint);
        Assert.AreEqual(spawnPoint.transform.parent, GameObject.Find("Map Container").transform);
    }

    [UnityTest]
    public IEnumerator RedirectsTo3DNavigationUpon3Dify() {
        GameObject.Find("3D-ify Button").GetComponent<Button>().onClick.Invoke();
        yield return null;
        Assert.AreEqual("3DMap", SceneManager.GetActiveScene().name);
    }

    [Test]
    public void LoadsMapFromMapData() {
        // check map is initially empty
        Assert.AreEqual("MapEditor", SceneManager.GetActiveScene().name);
        Assert.AreEqual(0, MapEditorManager.MapObjects.Count);

        // mock map data
        MapData mapData = new MapData(new Biome(0),
                                      new Vector2(0, 0),
                                      new List<Dictionary<int, MapObject>> {
                                          new Dictionary<int, MapObject> {
                                              {0, new MapObject(new MapObject(0, 
                                                                "Fortress", 
                                                                0, 
                                                                new Vector2(0, 0), 
                                                                new Vector2(100,100), 
                                                                new Vector3(1, 1, 1), 
                                                                Quaternion.identity, true),
                                                                "Layer0")}}},
                                      new Dictionary<string, int> {{"Layer0", 0}});
        MapEditorManager editor = GameObject.Find("MapEditorManager")
            .GetComponent<MapEditorManager>();

        // load a map from the mock map data
        editor.LoadMapFromMapData(mapData);
        Assert.AreEqual("MapEditor", SceneManager.GetActiveScene().name);

        // check that mapobjects on map reflect those in the mock map data
        Assert.AreEqual(1, MapEditorManager.MapObjects.Count);
        int loadedAssetId = new List<int>(MapEditorManager.MapObjects.Keys)[0];
        Assert.AreEqual("Fortress", MapEditorManager.MapObjects[loadedAssetId].Name);
        Assert.AreEqual(new Vector2(100,100), 
                        MapEditorManager.MapObjects[loadedAssetId].MapOffset);
        GameObject mapContainer = GameObject.Find("Map Container");
        GameObject fortressParent = mapContainer.transform.GetChild(2).gameObject;
        GameObject fortress = fortressParent.transform.GetChild(0).gameObject;
        Assert.AreEqual("FortressObject Parent", fortressParent.name);
        Assert.AreEqual(new Vector3(100,100,0), fortressParent.transform.localPosition);
        Assert.AreEqual("FortressObject(Clone)", fortress.name);
        Assert.AreEqual(0, fortress.transform.localPosition.x, PlayModeTestUtil.FloatTolerance);
        Assert.AreEqual(0, fortress.transform.localPosition.y, PlayModeTestUtil.FloatTolerance);
        Assert.AreEqual(0, fortress.transform.localPosition.z, PlayModeTestUtil.FloatTolerance);

        // check that spawn point was loaded correctly based on mock map data
        Assert.AreEqual("Spawn Point", mapContainer.transform.GetChild(1).name);
        Assert.AreEqual(new Vector3(0,0,0), mapContainer.transform.GetChild(1).localPosition);
    }

    [Test]
    public void ModalAppearsWhenLoadingNewMapInEditor() {
        // check modal is inactive by default
        GameObject modal = GameObject.Find("SaveModal");
        Assert.IsNull(modal);

        // try to load a new map
        GameObject.Find("File Button").GetComponent<EventTrigger>().OnPointerEnter(new PointerEventData(EventSystem.current));
        GameObject.Find("Open Button").GetComponent<Button>().onClick.Invoke();
        
        // check that modal appears
        modal = GameObject.Find("SaveModal");
        Assert.IsNotNull(modal);
        Assert.IsTrue(modal.activeSelf);
    }

    [Test]
    public void LoadMapInEditorOpensFileBrowser() {
        // click button to load a map
        GameObject.Find("File Button").GetComponent<EventTrigger>().OnPointerEnter(new PointerEventData(EventSystem.current));
        GameObject.Find("Open Button").GetComponent<Button>().onClick.Invoke();

        // decline to save current map
        GameObject.Find("No Button").GetComponent<Button>().onClick.Invoke();

        // check that file browser appears
        GameObject browser = GameObject.Find("SimpleFileBrowserCanvas(Clone)");
        Assert.IsNotNull(browser);
        GameObject submitButtonText = browser.transform.Find("SimpleFileBrowserWindow/Padding/BottomView/Padding/BottomRow/SubmitButton/SubmitButtonText").gameObject;
        Assert.AreEqual("Select", submitButtonText.GetComponent<Text>().text);
        GameObject titleText = browser.transform.Find("SimpleFileBrowserWindow/Titlebar/TitlebarText").gameObject;
        Assert.AreEqual("Select File", titleText.GetComponent<Text>().text);

        // close file browser
        GameObject cancelButton = browser.transform.Find("SimpleFileBrowserWindow/Padding/BottomView/Padding/BottomRow/CancelButton").gameObject;
        cancelButton.GetComponent<Button>().onClick.Invoke();
        Assert.IsNull(GameObject.Find("SimpleFileBrowserCanvas(Clone)"));
    }

    [Test]
    public void SaveAsOpensFileBrowser() {
        // click button to save map
        GameObject.Find("File Button").GetComponent<EventTrigger>().OnPointerEnter(new PointerEventData(EventSystem.current));
        GameObject.Find("Save As Button").GetComponent<Button>().onClick.Invoke();

        // check that file browser appears
        GameObject browser = GameObject.Find("SimpleFileBrowserCanvas(Clone)");
        Assert.IsNotNull(browser);
        GameObject submitButtonText = browser.transform.Find("SimpleFileBrowserWindow/Padding/BottomView/Padding/BottomRow/SubmitButton/SubmitButtonText").gameObject;
        Assert.AreEqual("Save", submitButtonText.GetComponent<Text>().text);
        GameObject titleText = browser.transform.Find("SimpleFileBrowserWindow/Titlebar/TitlebarText").gameObject;
        Assert.AreEqual("Save Map", titleText.GetComponent<Text>().text);

        // close file browser
        GameObject cancelButton = browser.transform.Find("SimpleFileBrowserWindow/Padding/BottomView/Padding/BottomRow/CancelButton").gameObject;
        cancelButton.GetComponent<Button>().onClick.Invoke();
        Assert.IsNull(GameObject.Find("SimpleFileBrowserCanvas(Clone)"));
    }
}
