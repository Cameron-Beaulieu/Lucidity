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
        Assert.AreEqual("Startup", SceneManager.GetActiveScene().name); 
        StartupScreen.IsTesting = true;
        Button button = GameObject.Find("Load Existing Map Button").GetComponent<Button>();
        button.onClick.Invoke();
        yield return null;
        Assert.AreEqual("MapEditor", SceneManager.GetActiveScene().name); 

        // reset testing var
        StartupScreen.IsTesting = false;
    }

    [Test]
    public void LoadMapInEditorOpensFileBrowser() {
        // click button to load a map
        Button button = GameObject.Find("Load Existing Map Button").GetComponent<Button>();
        button.onClick.Invoke();

        // check that file browser appears
        GameObject browser = GameObject.Find("SimpleFileBrowserCanvas(Clone)");
        Assert.IsNotNull(browser);
        GameObject submitButtonText = browser.transform.Find("SimpleFileBrowserWindow/Padding/"
            + "BottomView/Padding/BottomRow/SubmitButton/SubmitButtonText").gameObject;
        Assert.AreEqual("Select", submitButtonText.GetComponent<Text>().text);
        GameObject titleText = browser.transform
            .Find("SimpleFileBrowserWindow/Titlebar/TitlebarText").gameObject;
        Assert.AreEqual("Select File", titleText.GetComponent<Text>().text);

        // close file browser
        GameObject cancelButton = browser.transform.Find("SimpleFileBrowserWindow/Padding/"
            + "BottomView/Padding/BottomRow/CancelButton").gameObject;
        cancelButton.GetComponent<Button>().onClick.Invoke();
        Assert.IsNull(GameObject.Find("SimpleFileBrowserCanvas(Clone)"));
    }
}
