using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class StartupScreenTests {

    [UnitySetUp]
    public IEnumerator SetUp() {
        SceneManager.LoadScene("Startup");
        yield return null;
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Startup"));
        yield return null;
    }

    [UnityTest]
    public IEnumerator RedirectsToMapCreation() {
        Assert.AreEqual("Startup", SceneManager.GetActiveScene().name);
        Button button = GameObject.Find("Create New Map Button").GetComponent<Button>();
        button.onClick.Invoke();
        yield return null;
        Assert.AreEqual("MapCreation", SceneManager.GetActiveScene().name); 
    }

    [UnityTest]
    public IEnumerator RedirectsToMapEditor() {
        StartupScreen.IsTesting = true;
        Assert.AreEqual("Startup", SceneManager.GetActiveScene().name);
        Button button = GameObject.Find("Load Existing Map Button").GetComponent<Button>();
        button.onClick.Invoke();
        yield return null;
        Assert.AreEqual("MapEditor", SceneManager.GetActiveScene().name); 

        // reset testing var
        StartupScreen.IsTesting = false;
    }
}
