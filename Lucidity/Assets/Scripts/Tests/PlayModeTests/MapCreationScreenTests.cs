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
    public void CreateButtonOpensFileBrowser() {
        // give map name
        InputField nameInputField = GameObject.Find("Name Input").GetComponent<InputField>();
        nameInputField.text = "TestMap";

        // click button create a new map
        Button button = GameObject.Find("Create Button").GetComponent<Button>();
        button.onClick.Invoke();

        // check that file browser appears
        GameObject browser = GameObject.Find("SimpleFileBrowserCanvas(Clone)");
        Assert.IsNotNull(browser);
        GameObject submitButtonText = browser.transform.Find("SimpleFileBrowserWindow/Padding/BottomView/Padding/BottomRow/SubmitButton/SubmitButtonText").gameObject;
        Assert.AreEqual("Select", submitButtonText.GetComponent<Text>().text);
        GameObject titleText = browser.transform.Find("SimpleFileBrowserWindow/Titlebar/TitlebarText").gameObject;
        Assert.AreEqual("Select Save Location", titleText.GetComponent<Text>().text);

        // close file browser
        GameObject cancelButton = browser.transform.Find("SimpleFileBrowserWindow/Padding/BottomView/Padding/BottomRow/CancelButton").gameObject;
        cancelButton.GetComponent<Button>().onClick.Invoke();
        Assert.IsNull(GameObject.Find("SimpleFileBrowserCanvas(Clone)"));
    }

    [Test]
    public void ErrorsOnEmptyFileName() {
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
