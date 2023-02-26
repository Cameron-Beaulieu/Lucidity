using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class MapEditorTests : InputTestFixture {

    [UnitySetUp]
    public IEnumerator SetUp() {
        StartupScreen.FilePath = null;
        SceneManager.LoadScene("MapEditor");
        ResetStaticVariables();
        yield return null;
    }

    [Test]
    public void MapHasSpawnPoint() {
        Assert.AreEqual("MapEditor", SceneManager.GetActiveScene().name);
        GameObject spawnPoint = GameObject.Find("Spawn Point");
        Assert.IsNotNull(spawnPoint);
        Assert.AreEqual(spawnPoint.transform.parent, GameObject.Find("Map Container").transform);
    }

    [Test]
    public void EmptyMapHasOneLayer() {
        Assert.AreEqual("MapEditor", SceneManager.GetActiveScene().name);
        GameObject layerScrollContent = GameObject.Find("LayerScrollContent");
        Assert.AreEqual(layerScrollContent.transform.childCount, 1);
    }

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

    [Test]
    public void CanSwitchBetweenPaintButtons() {
        Assert.IsTrue(Tool.ToolStatus["Brush Tool"]);
        GameObject[] paintButtons = GameObject.FindGameObjectsWithTag("PaintButton");
        foreach (GameObject paintButton1 in paintButtons) {
            paintButton1.GetComponent<Button>().onClick.Invoke();
            Assert.IsTrue(paintButton1.GetComponent<AssetController>().Clicked);
            foreach (GameObject paintButton2 in paintButtons) {
                if (paintButton1 != paintButton2) {
                    Assert.IsFalse(paintButton2.GetComponent<AssetController>().Clicked);
                }
            }
        } 

        // TODO: maybe include this in teardown
        foreach (GameObject paintButton in paintButtons) {
            paintButton.GetComponent<AssetController>().UnselectButton();
        }
    }

    [Test]
    public void CanCollapseAndExpandPaintingMenu() {
        GameObject menuBody = GameObject.Find("Painting Body");
        Assert.IsTrue(menuBody.activeSelf);
        Button collapseButton = GameObject.Find("Painting Header (Expanded)").GetComponent<Button>();
        collapseButton.onClick.Invoke();
        Assert.IsFalse(menuBody.activeSelf);
        Assert.IsFalse(collapseButton.gameObject.activeSelf);
        Button expandButton = GameObject.Find("Painting Header (Collapsed)").GetComponent<Button>();
        expandButton.onClick.Invoke();
        Assert.IsTrue(menuBody.activeSelf);
        Assert.IsFalse(expandButton.gameObject.activeSelf);
    }

    [Test]
    public void CanCollapseAndExpandSelectionMenu() {
        Button selectionButton = GameObject.Find("Selection Tool").GetComponent<Button>();
        selectionButton.onClick.Invoke();
        GameObject menuBody = GameObject.Find("Selection Body");
        Assert.IsTrue(menuBody.activeSelf);
        Button collapseButton = GameObject.Find("Selection Header (Expanded)")
            .GetComponent<Button>();
        collapseButton.onClick.Invoke();
        Assert.IsFalse(menuBody.activeSelf);
        Assert.IsFalse(collapseButton.gameObject.activeSelf);
        Button expandButton = GameObject.Find("Selection Header (Collapsed)")
            .GetComponent<Button>();
        expandButton.onClick.Invoke();
        Assert.IsTrue(menuBody.activeSelf);
        Assert.IsFalse(expandButton.gameObject.activeSelf);
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

    [Test]
    public void CanPlaceAssets() {

        Assert.Zero(MapEditorManager.MapObjects.Count);

        Assert.IsTrue(Tool.ToolStatus["Brush Tool"]);
        Button fortressButton = GameObject.Find("FortressButton").GetComponent<Button>();
        fortressButton.onClick.Invoke();
        Assert.IsTrue(fortressButton.GetComponent<AssetController>().Clicked);

        Vector2 positionToPlace = new Vector2(-100, 150);
        MapEditorManager mapEditorManager = GameObject.Find("MapEditorManager").GetComponent<MapEditorManager>();
        mapEditorManager.PaintAtPosition(positionToPlace);

        GameObject placedParent = GameObject.Find("TempFortressObject Parent");
        Assert.IsNotNull(placedParent);
        Assert.AreEqual(1, placedParent.transform.childCount);
        Assert.AreEqual(1, MapEditorManager.MapObjects.Count);
        Assert.AreEqual(positionToPlace.x, placedParent.transform.position.x, 0.005f);
        Assert.AreEqual(positionToPlace.y, placedParent.transform.position.y, 0.005f);
        Assert.Zero(placedParent.transform.localPosition.z);

        fortressButton.GetComponent<AssetController>().UnselectButton();

    }

    // TODO: tests for nav menu

    // Utility Methods

    /// <summary>
    /// Checks that all tools are unselected except the one specified.
    /// </summary>
    /// <param name="toolSelected">The name of the tool that should be selected.</param>
    private void CheckAllOtherToolsAreUnselected(string toolSelected) {
        foreach (string tool in Tool.ToolKeys) {
            if (tool != toolSelected) {
                Assert.IsFalse(Tool.ToolStatus[tool]);
            }
        }
    }

    /// <summary>
    /// Resets all static variables in MapEditorManager and Tool. This is necessary to avoid 
    /// interference between tests.
    /// </summary>
    private void ResetStaticVariables() {
        MapEditorManager.MapObjects = new Dictionary<int, MapObject>();
        MapEditorManager.Actions = null;
        MapEditorManager.CurrentAction = null;
        MapEditorManager.ToolToCursorMap = new Dictionary<string, Texture2D>();
        MapEditorManager.Map = null;
        MapEditorManager.MapContainer = null;
        MapEditorManager.CurrentButtonPressed = 0;
        MapEditorManager.LastEncounteredObject = null;
        Tool.ToolKeys = new List<string>();
        Tool.ToolStatus = new Dictionary<string, bool>();
    }
}
