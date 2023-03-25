using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

[TestFixture]
public class ThreeDMapTests {

    [UnitySetUp]
    public IEnumerator SetUp() {
        Util.ResetStaticVariables();
        SceneManager.LoadScene("MapEditor");
        yield return null;
    }

    [OneTimeSetUp] 
    public void OneTimeSetUp() {
        AvatarMovement.IsTesting = true;
    }

    [TearDown]
    public void TearDown() {
        Util.ResetStaticVariables();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown() {
        AvatarMovement.IsTesting = false;
    }

    [UnityTest]
    public IEnumerator PlacesAvatarCorrectly() {
        // Get SpawnPoint location on 2D Map
        Vector2 spawnPointPosition = GameObject.Find("Spawn Point").transform.localPosition;

        // 3D-ify
        GameObject.Find("3D-ify Button").GetComponent<Button>().onClick.Invoke();
        yield return null;
        Assert.AreEqual("3DMap", SceneManager.GetActiveScene().name);
        yield return new WaitForEndOfFrame();

        // Check if SpawnPoint is at the correct location and the Avatar spawned correctly 
        Assert.AreEqual(spawnPointPosition.x, GameObject.Find("Avatar").transform.position.x, 
                        PlayModeTestUtil.FloatTolerance);
        Assert.AreEqual(6f, GameObject.Find("Avatar").transform.position.y, 
                        PlayModeTestUtil.FloatTolerance);
        Assert.AreEqual(spawnPointPosition.y, GameObject.Find("Avatar").transform.position.z, 
                        PlayModeTestUtil.FloatTolerance);
    }

    [UnityTest]
    public IEnumerator AvatarCanMoveForward() {
        // 3D-ify
        GameObject.Find("3D-ify Button").GetComponent<Button>().onClick.Invoke();
        yield return null;
        Assert.AreEqual("3DMap", SceneManager.GetActiveScene().name);
        yield return new WaitForEndOfFrame();
        
        // Get current Avatar position
        GameObject avatar = GameObject.Find("Avatar");
        Vector3 avatarPosition = avatar.transform.position;
        
        // Move Avatar forward
        AvatarMovement.VerticalTestingInput = 1;
        yield return new WaitForFixedUpdate();

        // Check Avatar position updated properly
        Assert.AreEqual(avatarPosition.x, avatar.transform.position.x, 
                        PlayModeTestUtil.FloatTolerance);
        Assert.AreEqual(avatarPosition.y, avatar.transform.position.y, 
                        PlayModeTestUtil.FloatTolerance);
        Assert.Greater(avatar.transform.position.z, avatarPosition.z);
    }

    [UnityTest]
    public IEnumerator AvatarCanMoveBackward() {
        // 3D-ify
        GameObject.Find("3D-ify Button").GetComponent<Button>().onClick.Invoke();
        yield return null;
        Assert.AreEqual("3DMap", SceneManager.GetActiveScene().name);
        yield return new WaitForEndOfFrame();

        // Get current Avatar position
        GameObject avatar = GameObject.Find("Avatar");
        Vector3 avatarPosition = avatar.transform.position;

        // Move Avatar backwards
        AvatarMovement.VerticalTestingInput = -1;
        yield return new WaitForFixedUpdate();

        // Check Avatar position updated properly
        Assert.AreEqual(avatarPosition.x, avatar.transform.position.x, 
                        PlayModeTestUtil.FloatTolerance);
        Assert.AreEqual(avatarPosition.y, avatar.transform.position.y, 
                        PlayModeTestUtil.FloatTolerance);
        Assert.Less(avatar.transform.position.z, avatarPosition.z);
    }

    [UnityTest]
    public IEnumerator AvatarCanMoveRight() {
        // 3D0ify
        GameObject.Find("3D-ify Button").GetComponent<Button>().onClick.Invoke();
        yield return null;
        Assert.AreEqual("3DMap", SceneManager.GetActiveScene().name);
        yield return new WaitForEndOfFrame();

        // Get current Avatar position
        GameObject avatar = GameObject.Find("Avatar");
        Vector3 avatarPosition = avatar.transform.position;
        AvatarMovement movementScript = avatar.GetComponent<AvatarMovement>();

        // Move Avatar right
        AvatarMovement.HorizontalTestingInput = 1;
        AvatarMovement.VerticalTestingInput = 1;
        yield return new WaitForFixedUpdate();

        // Check Avatar position updated properly
        Assert.Greater(avatar.transform.position.x, avatarPosition.x);
        Assert.AreEqual(avatarPosition.y, avatar.transform.position.y, 
                        PlayModeTestUtil.FloatTolerance);
        Assert.Greater(avatar.transform.position.z, avatarPosition.z);
    }

    [UnityTest]
    public IEnumerator AvatarCanMoveLeft() {
        // 3D-ify
        GameObject.Find("3D-ify Button").GetComponent<Button>().onClick.Invoke();
        yield return null;
        Assert.AreEqual("3DMap", SceneManager.GetActiveScene().name);
        yield return new WaitForEndOfFrame();
        
        // Get current Avatar position
        GameObject avatar = GameObject.Find("Avatar");
        Vector3 avatarPosition = avatar.transform.position;

        // Move Avatar left
        AvatarMovement movementScript = avatar.GetComponent<AvatarMovement>();
        AvatarMovement.HorizontalTestingInput = -1;
        AvatarMovement.VerticalTestingInput = 1;
        yield return new WaitForFixedUpdate();

        // Check Avatar position udpated properly
        Assert.Less(avatar.transform.position.x, avatarPosition.x);
        Assert.AreEqual(avatarPosition.y, avatar.transform.position.y, 
                        PlayModeTestUtil.FloatTolerance);
        Assert.Greater(avatar.transform.position.z, avatarPosition.z);
    }

    [UnityTest]
    public IEnumerator AvatarCanNoclip() {
        // 3D-ify
        GameObject.Find("3D-ify Button").GetComponent<Button>().onClick.Invoke();
        yield return null;
        Assert.AreEqual("3DMap", SceneManager.GetActiveScene().name);
        yield return new WaitForEndOfFrame();
        
        // Get current Avatar position
        GameObject avatar = GameObject.Find("Avatar");
        Vector3 avatarPosition = avatar.transform.position;

        // open up options menu
        Render3DScene.EscapeTestingInput = true;
        yield return null;

        // change the noclip toggle value
        AvatarMovement movementScript = GameObject.Find("Avatar").GetComponent<AvatarMovement>();
        Toggle noclipToggle = GameObject.Find("NoclipContainer").transform.Find("Toggle")
            .GetComponent<Toggle>();
        noclipToggle.isOn = true;
        yield return null;
        Assert.AreEqual(true, movementScript.Noclip);

        // close options menu
        Render3DScene.EscapeTestingInput = false;
        yield return null;

        // Make Avatar ascend
        AvatarMovement.AscendTestingInput = true;
        yield return new WaitForFixedUpdate();

        // Check Avatar position updated properly
        Assert.AreEqual(avatar.transform.position.x, avatarPosition.x, 
                        PlayModeTestUtil.FloatTolerance);
        Assert.Greater(avatar.transform.position.y, avatarPosition.y);
        Assert.AreEqual(avatar.transform.position.z, avatarPosition.z, 
                        PlayModeTestUtil.FloatTolerance);

        // Make Avatar descend (going into the ground)
        avatarPosition = avatar.transform.position;
        AvatarMovement.AscendTestingInput = false;
        AvatarMovement.DescendTestingInput = true;
        yield return new WaitForFixedUpdate();

        // Check Avatar position updated properly
        Assert.AreEqual(avatar.transform.position.x, avatarPosition.x, 
                        PlayModeTestUtil.FloatTolerance);
        Assert.Less(avatar.transform.position.y, avatarPosition.y);
        Assert.AreEqual(avatar.transform.position.z, avatarPosition.z, 
                        PlayModeTestUtil.FloatTolerance);
    }

    [UnityTest]
    public IEnumerator SpeedSliderModifiesAvatarSpeed() {
        // 3D-ify
        GameObject.Find("3D-ify Button").GetComponent<Button>().onClick.Invoke();
        yield return null;
        Assert.AreEqual("3DMap", SceneManager.GetActiveScene().name);
        yield return new WaitForEndOfFrame();

        // open up options menu
        Render3DScene.EscapeTestingInput = true;
        yield return null;

        // change the slider value
        AvatarMovement movementScript = GameObject.Find("Avatar").GetComponent<AvatarMovement>();
        Slider speedSlider = GameObject.Find("SpeedContainer").transform.Find("Slider")
            .GetComponent<Slider>();
        speedSlider.value = 50f;
        yield return null;
        Assert.AreEqual((speedSlider.value * 10f), movementScript.Speed);
    }

    [UnityTest]
    public IEnumerator SensitivitySliderModifiesMouseSensitivity() {
        // 3D-ify
        GameObject.Find("3D-ify Button").GetComponent<Button>().onClick.Invoke();
        yield return null;
        Assert.AreEqual("3DMap", SceneManager.GetActiveScene().name);
        yield return new WaitForEndOfFrame();

        // open up options menu
        Render3DScene.EscapeTestingInput = true;
        yield return null;

        // change the slider value
        MoveCamera cameraScript = GameObject.Find("Camera Holder").GetComponent<MoveCamera>();
        Slider sensitivitySlider = GameObject.Find("SensitivityContainer").transform.Find("Slider")
            .GetComponent<Slider>();
        sensitivitySlider.value = 50f;
        yield return null;
        Assert.AreEqual((sensitivitySlider.value * 10f), cameraScript.Sensitivity);
    }

    [UnityTest]
    public IEnumerator Maps2DTo3DProperly() {
        // paint assets
        PlayModeTestUtil.PaintAnAsset(new Vector2(-100, 150), "Fortress");
        PlayModeTestUtil.PaintAnAsset(new Vector2(100, 150), "Mountain");
        GameObject fortressParent = GameObject.Find("FortressObject Parent");
        GameObject mountainParent = GameObject.Find("MountainObject Parent");
        Vector2 fortressPosition = fortressParent.transform.localPosition;
        Vector3 fortressScale = fortressParent.transform.localScale;
        Vector2 mountainPosition = mountainParent.transform.localPosition;
        Vector3 mountainScale = mountainParent.transform.localScale;

        // 3D-ify
        GameObject.Find("3D-ify Button").GetComponent<Button>().onClick.Invoke();
        yield return null;
        Assert.AreEqual("3DMap", SceneManager.GetActiveScene().name);
        yield return new WaitForEndOfFrame();

        // check that the assets are in the right place
        GameObject fortress3D = GameObject.Find("LucidityFortress(Clone)");
        Assert.IsNotNull(fortress3D);
        GameObject mountain3D = GameObject.Find("LucidityMountain(Clone)");
        Assert.IsNotNull(mountain3D);
        GameObject map = GameObject.Find("ForestPlane(Clone)");
        Assert.AreEqual(fortressPosition.x, fortress3D.transform.position.x, 
                        PlayModeTestUtil.FloatTolerance);
        Assert.AreEqual(fortressPosition.y, fortress3D.transform.position.z, 
                        PlayModeTestUtil.FloatTolerance);
        Assert.Greater(fortress3D.transform.position.y, 0);
        Assert.AreEqual(mountainPosition.x, mountain3D.transform.position.x, 
                        PlayModeTestUtil.FloatTolerance);
        Assert.AreEqual(mountainPosition.y, mountain3D.transform.position.z, 
                        PlayModeTestUtil.FloatTolerance);
        // mountain has special positioning for the y due to the way the asset was modelled
        Assert.AreEqual(0, mountain3D.transform.position.y, PlayModeTestUtil.FloatTolerance);
    }

    [UnityTest]
    public IEnumerator TreeSpawnsCorrectlyAboveMountain() {
        // paint the mountain on base layer
        Button mountainButton = GameObject.Find("MountainButton").GetComponent<Button>();
        mountainButton.onClick.Invoke();
        Assert.IsTrue(mountainButton.GetComponent<AssetController>().Clicked);
        Vector2 positionToPlace = new Vector2(3, 3);
        MapEditorManager mapEditorManager = GameObject.Find("MapEditorManager")
            .GetComponent<MapEditorManager>();
        mapEditorManager.PaintAtPosition(positionToPlace);
        Assert.AreEqual(1, MapEditorManager.Layers[0].Count);

        // add new layer
        GameObject.Find("Layer Tool").GetComponent<Button>().onClick.Invoke();
        yield return null;
        Assert.AreEqual(2, MapEditorManager.Layers.Count);

        // paint the tree on mountain
        Button treeButton = GameObject.Find("TreeButton").GetComponent<Button>();
        treeButton.onClick.Invoke();
        Assert.IsTrue(treeButton.GetComponent<AssetController>().Clicked);
        mapEditorManager.PaintAtPosition(positionToPlace + new Vector2(0.2f,0.2f));
        yield return null;
        yield return new WaitForSeconds(0.5f);

        GameObject treeParent = GameObject.Find("TreeObject Parent");
        Vector2 treePosition = treeParent.transform.localPosition;

        // 3D-ify
        GameObject.Find("3D-ify Button").GetComponent<Button>().onClick.Invoke();
        yield return null;
        Assert.AreEqual("3DMap", SceneManager.GetActiveScene().name);
        yield return new WaitForEndOfFrame();

        GameObject tree3D = GameObject.Find("LucidityTree(Clone)");
        Assert.IsNotNull(tree3D);
        GameObject mountain3D = GameObject.Find("LucidityMountain(Clone)");
        Assert.IsNotNull(mountain3D);
        MeshCollider mountainCollider = mountain3D.GetComponent<MeshCollider>();
        float mountainHeight = mountainCollider.bounds.size.y * 
            mountain3D.transform.localScale.y;

        // Make sure the tree is off the ground, but not above the mountain
        Assert.Greater(tree3D.transform.position.y, 100f);
        Assert.Less(tree3D.transform.position.y, mountainHeight);
    }
}