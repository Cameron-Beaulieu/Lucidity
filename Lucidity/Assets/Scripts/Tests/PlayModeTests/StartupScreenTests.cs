using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class StartupScreenTests {

    [SetUp]
    public void SetUp() {
        SceneManager.LoadScene("StarterScreenUI"); // TODO: change this to scene's new name
    }

    [UnityTest]
    public IEnumerator RedirectsToMapCreation() {
        Assert.AreEqual("StarterScreenUI", SceneManager.GetActiveScene().name); // TODO: change this to scene's new name
        Button button = GameObject.Find("Create new map button").GetComponent<Button>();
        button.onClick.Invoke();
        yield return null;
        Assert.AreEqual("MapCreationUI", SceneManager.GetActiveScene().name); // TODO: change this to scene's new name
    }

    // TODO: find a way to test file loading
}
