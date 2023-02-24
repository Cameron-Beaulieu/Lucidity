using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class MapCreationScreenTests {

    [SetUp]
    public void SetUp() {
        SceneManager.LoadScene("MapCreationUI"); // TODO: change this to scene's new name
    }

    [UnityTest]
    public IEnumerator CancelRedirectsToStartupScreen() {
        Assert.AreEqual("MapCreationUI", SceneManager.GetActiveScene().name); // TODO: change this to scene's new name
        Button button = GameObject.Find("Cancel Button").GetComponent<Button>();
        button.onClick.Invoke();
        yield return null;
        Assert.AreEqual("StarterScreenUI", SceneManager.GetActiveScene().name); // TODO: change this to scene's new name
    }

    [UnityTest]
    public IEnumerator ErrorsOnEmptyFileName() {
        Assert.AreEqual("MapCreationUI", SceneManager.GetActiveScene().name); // TODO: change this to scene's new name
        Button button = GameObject.Find("Create Button").GetComponent<Button>();
        button.onClick.Invoke();
        yield return null;
        Assert.AreEqual("MapCreationUI", SceneManager.GetActiveScene().name); // TODO: change this to scene's new name
        Assert.AreEqual("You must provide a file name to create a map", GameObject.Find("ErrorMessage").GetComponent<Text>().text);
    }
}
