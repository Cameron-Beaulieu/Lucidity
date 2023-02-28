using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

public abstract class MapEditorTests {

    [UnitySetUp]
    public IEnumerator SetUp() {
        Util.ResetStaticVariables();
        StartupScreen.FilePath = null;
        SceneManager.LoadScene("MapEditor");
        yield return null;
    }

    [TearDown]
    public void TearDown() {
        GameObject[] paintButtons = GameObject.FindGameObjectsWithTag("PaintButton");
        foreach (GameObject paintButton in paintButtons) {
            paintButton.GetComponent<AssetController>().UnselectButton();
        }
    }

    [OneTimeTearDown]
    public void OneTimeTearDown() {
        if (SceneManager.GetSceneByName("MapEditor").isLoaded) {
            SceneManager.UnloadSceneAsync("MapEditor");
        }
        Util.ResetStaticVariables();
    }
}
