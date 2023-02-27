using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

[TestFixture]
public class SpawnPointTests : MapEditorTests {
    [Test]
    public void MapHasSpawnPoint() {
        Assert.AreEqual("MapEditor", SceneManager.GetActiveScene().name);
        GameObject spawnPoint = GameObject.Find("Spawn Point");
        Assert.IsNotNull(spawnPoint);
        Assert.AreEqual(spawnPoint.transform.parent, GameObject.Find("Map Container").transform);
    }
}
