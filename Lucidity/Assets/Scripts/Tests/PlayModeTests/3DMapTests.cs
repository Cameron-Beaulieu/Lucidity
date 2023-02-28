using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class ThreeDMapTests {

    [UnitySetUp]
    public IEnumerator SetUp() {
        MapEditorTests.ResetStaticVariables();
        SceneManager.LoadScene("MapEditor");
        yield return null;
    }

    [OneTimeTearDown]
    public void TearDown() {
        if (SceneManager.GetSceneByName("3DMap").isLoaded) {
            SceneManager.UnloadSceneAsync("3DMap");
        }
        MapEditorTests.ResetStaticVariables();
    }

    [UnityTest]
    public IEnumerator Maps2DTo3DProperly() {
        MapEditorTests.PaintAnAsset(new Vector2(-100, 150), "Fortress");
        MapEditorTests.PaintAnAsset(new Vector2(100, 150), "House");

        GameObject.Find("3D-ify Button").GetComponent<Button>().onClick.Invoke();
        yield return null;
        Assert.AreEqual("3DMap", SceneManager.GetActiveScene().name);

        GameObject fortress3D = GameObject.Find("TempOceanMap(Clone)");
        Assert.IsNotNull(fortress3D);
        GameObject house3D = GameObject.Find("TempDesertMap(Clone)");
        Assert.IsNotNull(house3D);
        GameObject map = GameObject.Find("ForestPlane(Clone)");

        Assert.AreEqual(new Vector3(-100, 0, 150), fortress3D.transform.position);
        Assert.AreEqual(new Vector3(100, 0, 150), house3D.transform.position);

    }
    
}