using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class MapCreationScreenTests {

    [UnitySetUp]
    public IEnumerator SetUp() {
        SceneManager.LoadScene("MapCreation"); 
        yield return null;
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("MapCreation"));
    }

    [UnityTest]
    public IEnumerator CancelButtonRedirectsToStartupScreen() {
        Assert.AreEqual("MapCreation", SceneManager.GetActiveScene().name); 
        Button button = GameObject.Find("Cancel Button").GetComponent<Button>();
        button.onClick.Invoke();
        yield return null;
        Assert.AreEqual("Startup", SceneManager.GetActiveScene().name); 
    }

    [Test]
    public void ErrorsOnEmptyFileName() {
        Assert.AreEqual("MapCreation", SceneManager.GetActiveScene().name); 
        Button button = GameObject.Find("Create Button").GetComponent<Button>();
        button.onClick.Invoke();
        Assert.AreEqual("MapCreation", SceneManager.GetActiveScene().name);
        Assert.AreEqual("You must provide a file name to create a map.", 
                        GameObject.Find("ErrorMessage").GetComponent<Text>().text);
    }

    [UnityTest]
    public IEnumerator RedirectsToMapEditorWithSpecifiedMapDetails() {
        // "create" a map
        CreateNewMap.IsTesting = true;
        Assert.AreEqual("MapCreation", SceneManager.GetActiveScene().name);
        InputField nameInputField = GameObject.Find("Name Input").GetComponent<InputField>();
        nameInputField.text = "TestMap";
        GameObject.Find("Create Button").GetComponent<Button>().onClick.Invoke();

        // check that the map editor has the correct details
        Assert.AreEqual(Biome.BiomeType.Forest, CreateNewMap.ChosenBiome.Name);
        yield return null;
        Assert.AreEqual("MapEditor", SceneManager.GetActiveScene().name);
        CreateNewMap.IsTesting = false;
    }
}
