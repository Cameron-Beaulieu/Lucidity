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
    }

    [OneTimeTearDown]
    public void OneTimeTearDown() {
        if (SceneManager.GetSceneByName("Startup").isLoaded) {
            SceneManager.UnloadSceneAsync("Startup");
        }
    }

    [UnityTest]
    public IEnumerator RedirectsToMapCreation() {
        Assert.AreEqual("Startup", SceneManager.GetActiveScene().name);
        Button button = GameObject.Find("Create New Map Button").GetComponent<Button>();
        button.onClick.Invoke();
        yield return null;
        Assert.AreEqual("MapCreation", SceneManager.GetActiveScene().name); 
    }
}
