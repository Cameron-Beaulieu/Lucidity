using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

[TestFixture]
public class MiscellaneousTests : MapEditorTests {

    [Test]
    public void MapHasSpawnPoint() {
        Assert.AreEqual("MapEditor", SceneManager.GetActiveScene().name);
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
        // mock map data
        MapData mapData = new MapData(new Biome(0), 
                                      new Dictionary<int, MapObject> {
                                        {0, new MapObject(0, 
                                                          "Fortress", 
                                                          0, 
                                                          new Vector2(0, 0), 
                                                          new Vector2(100,100), 
                                                          new Vector3(1, 1, 1), 
                                                          Quaternion.identity, true)}},
                                      new Vector2(0, 0));
        MapEditorManager editor = GameObject.Find("MapEditorManager")
            .GetComponent<MapEditorManager>();

        // check map is initially empty
        Assert.AreEqual("MapEditor", SceneManager.GetActiveScene().name);
        Assert.AreEqual(0, MapEditorManager.MapObjects.Count);

        // load a map from the mock map data
        editor.LoadMapFromMapData(mapData);
        Assert.AreEqual("MapEditor", SceneManager.GetActiveScene().name);

        // check that mapobjects on map reflect those in the mock map data
        Assert.AreEqual(1, MapEditorManager.MapObjects.Count);
        int loadedAssetId = new List<int>(MapEditorManager.MapObjects.Keys)[0];
        Assert.AreEqual("Fortress", MapEditorManager.MapObjects[loadedAssetId].Name);
        Assert.AreEqual(new Vector2(100,100), MapEditorManager.MapObjects[loadedAssetId].MapOffset);
        GameObject mapContainer = GameObject.Find("Map Container");
        GameObject fortressParent = mapContainer.transform.GetChild(2).gameObject;
        GameObject fortress = fortressParent.transform.GetChild(0).gameObject;
        Assert.AreEqual("FortressObject Parent", fortressParent.name);
        Assert.AreEqual(new Vector3(100,100,90), fortressParent.transform.localPosition);
        Assert.AreEqual("FortressObject(Clone)", fortress.name);
        Assert.AreEqual(0, fortress.transform.localPosition.x, PlayModeTestUtil.FloatTolerance);
        Assert.AreEqual(0, fortress.transform.localPosition.y, PlayModeTestUtil.FloatTolerance);
        Assert.AreEqual(0, fortress.transform.localPosition.z, PlayModeTestUtil.FloatTolerance);

        // check that spawn point was loaded correctly based on mock map data
        Assert.AreEqual("Spawn Point", mapContainer.transform.GetChild(1).name);
        Assert.AreEqual(new Vector3(0,0,90), mapContainer.transform.GetChild(1).localPosition);
        
    }
}
