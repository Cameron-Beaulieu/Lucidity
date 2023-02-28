using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
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
}
