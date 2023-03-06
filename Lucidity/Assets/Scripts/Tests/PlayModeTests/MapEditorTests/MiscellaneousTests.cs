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

        Assert.AreEqual("MapEditor", SceneManager.GetActiveScene().name);
        Assert.AreEqual(0, MapEditorManager.MapObjects.Count);
        editor.LoadMapFromMapData(mapData);
        Assert.AreEqual("MapEditor", SceneManager.GetActiveScene().name);
        Assert.AreEqual(1, MapEditorManager.MapObjects.Count);
        int loadedAssetId = new List<int>(MapEditorManager.MapObjects.Keys)[0];
        Assert.AreEqual("Fortress", MapEditorManager.MapObjects[loadedAssetId].Name);
        Assert.AreEqual(new Vector2(100,100), MapEditorManager.MapObjects[loadedAssetId].MapOffset);

        GameObject mapContainer = GameObject.Find("Map Container");
        Assert.AreEqual("Spawn Point", mapContainer.transform.GetChild(1).name);
        Assert.AreEqual(new Vector3(0,0,-101), mapContainer.transform.GetChild(1).localPosition);
        Assert.AreEqual("TempFortressObject Parent", mapContainer.transform.GetChild(2).name);
        Assert.AreEqual(new Vector3(100,100,0), mapContainer.transform.GetChild(2).localPosition);
        Assert.AreEqual(new Vector3(0,0,-2), mapContainer.transform.GetChild(2).transform.GetChild(0).localPosition);
        
    }
}
