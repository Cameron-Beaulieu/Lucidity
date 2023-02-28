using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

[TestFixture]
public class OtherToolsTests : MapEditorTests {
    
    [Test]
    public void CanSwitchToPanningTool() {
        Button panningToolButton = GameObject.Find("Panning Tool").GetComponent<Button>();
        panningToolButton.onClick.Invoke();
        Assert.IsTrue(Tool.ToolStatus["Panning Tool"]);
        CheckAllOtherToolsAreUnselected("Panning Tool");
        Assert.IsFalse(Tool.SelectionMenu.activeSelf);
        Assert.IsFalse(Tool.PaintingMenu.activeSelf);
    }

    // TODO: tests for panning

    [Test]
    public void CanSwitchToZoomInTool() {
        Button zoomInButton = GameObject.Find("Zoom In").GetComponent<Button>();
        zoomInButton.onClick.Invoke();
        Assert.IsTrue(Tool.ToolStatus["Zoom In"]);
        CheckAllOtherToolsAreUnselected("Zoom In");
        Assert.IsFalse(Tool.SelectionMenu.activeSelf);
        Assert.IsFalse(Tool.PaintingMenu.activeSelf);
    }

    [Test]
    public void CanZoomIn() {
        GameObject mapContainer = GameObject.Find("Map Container");
        Vector3 originalScale = mapContainer.transform.localScale;
        Zoom.IsTesting = true;
        GameObject.Find("Zoom In").GetComponent<Button>().onClick.Invoke();
        mapContainer.GetComponent<Zoom>().OnMouseDown();
        Assert.AreEqual(new Vector3(originalScale.x + 0.5f, originalScale.y + 0.5f, originalScale.z + 0.5f), mapContainer.transform.localScale);

        // reset testing var
        Zoom.IsTesting = false;
    }

    [Test]
    public void CanSwitchToZoomOutTool() {
        Button zoomOutButton = GameObject.Find("Zoom Out").GetComponent<Button>();
        zoomOutButton.onClick.Invoke();
        Assert.IsTrue(Tool.ToolStatus["Zoom Out"]);
        CheckAllOtherToolsAreUnselected("Zoom Out");
        Assert.IsFalse(Tool.SelectionMenu.activeSelf);
        Assert.IsFalse(Tool.PaintingMenu.activeSelf);
    }

    [Test]
    public void CanZoomOut() {
        GameObject mapContainer = GameObject.Find("Map Container");
        Vector3 originalScale = mapContainer.transform.localScale;
        Zoom.IsTesting = true;

        // zoom in first
        GameObject.Find("Zoom In").GetComponent<Button>().onClick.Invoke();
        mapContainer.GetComponent<Zoom>().OnMouseDown();
        Assert.AreEqual(new Vector3(originalScale.x + 0.5f, originalScale.y + 0.5f, originalScale.z + 0.5f), mapContainer.transform.localScale);
        
        // zoom out
        GameObject.Find("Zoom Out").GetComponent<Button>().onClick.Invoke();
        mapContainer.GetComponent<Zoom>().OnMouseDown();
        Assert.AreEqual(originalScale, mapContainer.transform.localScale);

        // reset testing var
        Zoom.IsTesting = false;
    }

}
