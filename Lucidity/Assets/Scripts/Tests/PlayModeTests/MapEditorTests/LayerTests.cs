using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

[TestFixture]
public class LayerTests : MapEditorTests {
    [Test]
    public void EmptyMapHasOneLayer() {
        Assert.AreEqual("MapEditor", SceneManager.GetActiveScene().name);
        GameObject layerScrollContent = GameObject.Find("LayerScrollContent");
        Assert.AreEqual(layerScrollContent.transform.childCount, 1);
    }

    [Test]
    public void CanCollapseAndExpandLayersMenu() {
        GameObject menuBody = GameObject.Find("Layers Body");
        Assert.IsTrue(menuBody.activeSelf);
        Button collapseButton = GameObject.Find("Layers Header (Expanded)").GetComponent<Button>();
        collapseButton.onClick.Invoke();
        Assert.IsFalse(menuBody.activeSelf);
        Assert.IsFalse(collapseButton.gameObject.activeSelf);
        Button expandButton = GameObject.Find("Layers Header (Collapsed)").GetComponent<Button>();
        expandButton.onClick.Invoke();
        Assert.IsTrue(menuBody.activeSelf);
        Assert.IsFalse(expandButton.gameObject.activeSelf);
    }
}
