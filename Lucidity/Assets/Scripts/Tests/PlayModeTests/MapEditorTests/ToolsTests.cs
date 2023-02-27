using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

[TestFixture]
public class ToolsTests : MapEditorTests {
    [Test]
    public void DefaultsToBrushTool() {
        Assert.AreEqual("MapEditor", SceneManager.GetActiveScene().name);
        Assert.IsTrue(Tool.ToolStatus["Brush Tool"]);
        CheckAllOtherToolsAreUnselected("Brush Tool");
        Assert.IsTrue(Tool.PaintingMenu.activeSelf);
    }

    [Test]
    public void CanSwitchToSelectionTool() {
        Button selectionToolButton = GameObject.Find("Selection Tool").GetComponent<Button>();
        selectionToolButton.onClick.Invoke();
        Assert.IsTrue(Tool.ToolStatus["Selection Tool"]);
        CheckAllOtherToolsAreUnselected("Selection Tool");
        Assert.IsTrue(Tool.SelectionMenu.activeSelf);
        Assert.IsFalse(Tool.PaintingMenu.activeSelf);
    }

    [Test]
    public void CanSwitchToPanningTool() {
        Button panningToolButton = GameObject.Find("Panning Tool").GetComponent<Button>();
        panningToolButton.onClick.Invoke();
        Assert.IsTrue(Tool.ToolStatus["Panning Tool"]);
        CheckAllOtherToolsAreUnselected("Panning Tool");
        Assert.IsFalse(Tool.SelectionMenu.activeSelf);
        Assert.IsFalse(Tool.PaintingMenu.activeSelf);
    }

    [Test]
    public void CanSwitchToBrushTool() {
        // switch to another tool first since brush tool is default
        Button selectionToolButton = GameObject.Find("Selection Tool").GetComponent<Button>();
        selectionToolButton.onClick.Invoke();
        Assert.IsFalse(Tool.ToolStatus["Brush Tool"]);

        Button brushToolButton = GameObject.Find("Brush Tool").GetComponent<Button>();
        brushToolButton.onClick.Invoke();
        Assert.IsTrue(Tool.ToolStatus["Brush Tool"]);
        CheckAllOtherToolsAreUnselected("Brush Tool");
        Assert.IsFalse(Tool.SelectionMenu.activeSelf);
        Assert.IsTrue(Tool.PaintingMenu.activeSelf);
    }

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
    public void CanSwitchToZoomOutTool() {
        Button zoomOutButton = GameObject.Find("Zoom Out").GetComponent<Button>();
        zoomOutButton.onClick.Invoke();
        Assert.IsTrue(Tool.ToolStatus["Zoom Out"]);
        CheckAllOtherToolsAreUnselected("Zoom Out");
        Assert.IsFalse(Tool.SelectionMenu.activeSelf);
        Assert.IsFalse(Tool.PaintingMenu.activeSelf);
    }
}
