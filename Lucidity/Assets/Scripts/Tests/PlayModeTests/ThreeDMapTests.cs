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
        AvatarMovement.HorizontalTestingInput = 0f;
        AvatarMovement.VerticalTestingInput = 0f;
    }

    [OneTimeTearDown]
    public void OneTimeTearDown() {
        AvatarMovement.IsTesting = false;
    }

    [UnityTest]
    public IEnumerator PlacesAvatarCorrectly() {
        Vector2 spawnPointPosition = GameObject.Find("Spawn Point").transform.localPosition;
        GameObject.Find("3D-ify Button").GetComponent<Button>().onClick.Invoke();
        yield return null;
        Assert.AreEqual("3DMap", SceneManager.GetActiveScene().name);
        yield return new WaitForEndOfFrame();
        Assert.AreEqual(spawnPointPosition.x, GameObject.Find("Avatar").transform.position.x, 
                        PlayModeTestUtil.FloatTolerance);
        Assert.AreEqual(1f, GameObject.Find("Avatar").transform.position.y, PlayModeTestUtil.FloatTolerance);
        Assert.AreEqual(spawnPointPosition.y, GameObject.Find("Avatar").transform.position.z, 
                        PlayModeTestUtil.FloatTolerance);
    }

    [UnityTest]
    public IEnumerator AvatarCanMoveForward() {
        GameObject.Find("3D-ify Button").GetComponent<Button>().onClick.Invoke();
        yield return null;
        Assert.AreEqual("3DMap", SceneManager.GetActiveScene().name);
        yield return new WaitForEndOfFrame();
        GameObject avatar = GameObject.Find("Avatar");
        Vector3 avatarPosition = avatar.transform.position;
        AvatarMovement.VerticalTestingInput = 1;
        yield return new WaitForFixedUpdate();
        Assert.AreEqual(avatarPosition.x, avatar.transform.position.x, PlayModeTestUtil.FloatTolerance);
        Assert.AreEqual(avatarPosition.y, avatar.transform.position.y, PlayModeTestUtil.FloatTolerance);
        Assert.Greater(avatar.transform.position.z, avatarPosition.z);
    }

    [UnityTest]
    public IEnumerator AvatarCanMoveBackward() {
        GameObject.Find("3D-ify Button").GetComponent<Button>().onClick.Invoke();
        yield return null;
        Assert.AreEqual("3DMap", SceneManager.GetActiveScene().name);
        yield return new WaitForEndOfFrame();
        GameObject avatar = GameObject.Find("Avatar");
        Vector3 avatarPosition = avatar.transform.position;
        AvatarMovement.VerticalTestingInput = -1;
        yield return new WaitForFixedUpdate();
        Assert.AreEqual(avatarPosition.x, avatar.transform.position.x, PlayModeTestUtil.FloatTolerance);
        Assert.AreEqual(avatarPosition.y, avatar.transform.position.y, PlayModeTestUtil.FloatTolerance);
        Assert.Less(avatar.transform.position.z, avatarPosition.z);
    }

    [UnityTest]
    public IEnumerator AvatarCanMoveRight() {
        GameObject.Find("3D-ify Button").GetComponent<Button>().onClick.Invoke();
        yield return null;
        Assert.AreEqual("3DMap", SceneManager.GetActiveScene().name);
        yield return new WaitForEndOfFrame();
        GameObject avatar = GameObject.Find("Avatar");
        Vector3 avatarPosition = avatar.transform.position;
        AvatarMovement movementScript = avatar.GetComponent<AvatarMovement>();
        float avatarOrientation = movementScript.Orientation.rotation.y;
        AvatarMovement.HorizontalTestingInput = 1;
        AvatarMovement.VerticalTestingInput = 1;
        yield return new WaitForFixedUpdate();
        Assert.Greater(avatar.transform.position.x, avatarPosition.x);
        Assert.AreEqual(avatarPosition.y, avatar.transform.position.y, PlayModeTestUtil.FloatTolerance);
        Assert.Greater(avatar.transform.position.z, avatarPosition.z);
        Assert.Greater(movementScript.Orientation.rotation.y, avatarOrientation);
    }

    [UnityTest]
    public IEnumerator AvatarCanMoveLeft() {
        GameObject.Find("3D-ify Button").GetComponent<Button>().onClick.Invoke();
        yield return null;
        Assert.AreEqual("3DMap", SceneManager.GetActiveScene().name);
        yield return new WaitForEndOfFrame();
        GameObject avatar = GameObject.Find("Avatar");
        Vector3 avatarPosition = avatar.transform.position;
        AvatarMovement movementScript = avatar.GetComponent<AvatarMovement>();
        float avatarOrientation = movementScript.Orientation.rotation.y;
        AvatarMovement.HorizontalTestingInput = -1;
        AvatarMovement.VerticalTestingInput = 1;
        yield return new WaitForFixedUpdate();
        Assert.Less(avatar.transform.position.x, avatarPosition.x);
        Assert.AreEqual(avatarPosition.y, avatar.transform.position.y, PlayModeTestUtil.FloatTolerance);
        Assert.Greater(avatar.transform.position.z, avatarPosition.z);
        Assert.Less(movementScript.Orientation.rotation.y, avatarOrientation);
    }

    [UnityTest]
    public IEnumerator Maps2DTo3DProperly() {
        // paint assets
        PlayModeTestUtil.PaintAnAsset(new Vector2(-100, 150), "Fortress");
        PlayModeTestUtil.PaintAnAsset(new Vector2(100, 150), "House");
        GameObject fortressParent = GameObject.Find("TempFortressObject Parent");
        GameObject houseParent = GameObject.Find("TempHouseObject Parent");
        Vector2 fortressPosition = fortressParent.transform.localPosition;
        Vector3 fortressScale = fortressParent.transform.localScale;
        Vector2 housePosition = houseParent.transform.localPosition;
        Vector3 houseScale = houseParent.transform.localScale;

        // 3D-ify
        GameObject.Find("3D-ify Button").GetComponent<Button>().onClick.Invoke();
        yield return null;
        Assert.AreEqual("3DMap", SceneManager.GetActiveScene().name);
        yield return null;
        yield return new WaitForEndOfFrame();

        // check that the assets are in the right place
        GameObject fortress3D = GameObject.Find("TempOceanMap(Clone)");
        Assert.IsNotNull(fortress3D);
        GameObject house3D = GameObject.Find("TempDesertMap(Clone)");
        Assert.IsNotNull(house3D);
        GameObject map = GameObject.Find("ForestPlane(Clone)");
        Assert.AreEqual(new Vector3(fortressPosition.x, 
                                    fortressScale.y / 2 + map.transform.position.y, 
                                    fortressPosition.y), 
                        fortress3D.transform.position);
        Assert.AreEqual(new Vector3(housePosition.x, 
                                    houseScale.y / 2 + map.transform.position.y, 
                                    housePosition.y),
                        house3D.transform.position);

    }
    
}