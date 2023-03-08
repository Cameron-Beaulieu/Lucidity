using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

[TestFixture]
public class OtherToolsTests : MapEditorTests {
    
    [Test]
    public void CanSwitchToPanningTool() {
        // Switching to panning tool
        Button panningToolButton = GameObject.Find("Panning Tool").GetComponent<Button>();
        panningToolButton.onClick.Invoke();

        // Checking active tool
        Assert.IsTrue(Tool.ToolStatus["Panning Tool"]);
        PlayModeTestUtil.CheckAllOtherToolsAreUnselected("Panning Tool");

        // Checking menuing has correctly updated
        Assert.IsFalse(Tool.SelectionMenu.activeSelf);
        Assert.IsFalse(Tool.PaintingMenu.activeSelf);
    }

    [Test]
    public void CanSwitchToZoomInTool() {
        // Switching to zoom in tool
        Button zoomInButton = GameObject.Find("Zoom In").GetComponent<Button>();
        zoomInButton.onClick.Invoke();

        // Checking active tool
        Assert.IsTrue(Tool.ToolStatus["Zoom In"]);
        PlayModeTestUtil.CheckAllOtherToolsAreUnselected("Zoom In");

        // Checking menuing has correctly updated
        Assert.IsFalse(Tool.SelectionMenu.activeSelf);
        Assert.IsFalse(Tool.PaintingMenu.activeSelf);
    }

    [Test]
    public void CanZoomIn() {
        // Getting original map scale
        GameObject mapContainer = GameObject.Find("Map Container");
        Vector3 originalScale = mapContainer.transform.localScale;

        // Zooming in
        Zoom.IsTesting = true;
        GameObject.Find("Zoom In").GetComponent<Button>().onClick.Invoke();
        mapContainer.GetComponent<Zoom>().OnMouseDown();

        // Checking new scale vs expected scale
        Assert.AreEqual(new Vector3(originalScale.x + 0.5f, 
                                    originalScale.y + 0.5f, 
                                    originalScale.z + 0.5f), 
                        mapContainer.transform.localScale);

        // reset testing var
        Zoom.IsTesting = false;
    }

    [Test]
    public void CanSwitchToZoomOutTool() {
        // Switching to zoom out tool
        Button zoomOutButton = GameObject.Find("Zoom Out").GetComponent<Button>();
        zoomOutButton.onClick.Invoke();

        // Checking active tool
        Assert.IsTrue(Tool.ToolStatus["Zoom Out"]);
        PlayModeTestUtil.CheckAllOtherToolsAreUnselected("Zoom Out");

        // Checking menuing has correctly updates
        Assert.IsFalse(Tool.SelectionMenu.activeSelf);
        Assert.IsFalse(Tool.PaintingMenu.activeSelf);
    }

    [Test]
    public void CanZoomOut() {
        // Getting original MapContainer scale within MapEditor
        GameObject mapContainer = GameObject.Find("Map Container");
        Vector3 originalScale = mapContainer.transform.localScale;
        Zoom.IsTesting = true;

        // zoom in first
        GameObject.Find("Zoom In").GetComponent<Button>().onClick.Invoke();
        mapContainer.GetComponent<Zoom>().OnMouseDown();
        Assert.AreEqual(new Vector3(originalScale.x + 0.5f, 
                                    originalScale.y + 0.5f, 
                                    originalScale.z + 0.5f), 
                        mapContainer.transform.localScale);
        
        // zoom out
        GameObject.Find("Zoom Out").GetComponent<Button>().onClick.Invoke();
        mapContainer.GetComponent<Zoom>().OnMouseDown();
        Assert.AreEqual(originalScale, mapContainer.transform.localScale);

        // reset testing var
        Zoom.IsTesting = false;
    }
}
