using RaycastingLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AssetCollision : MonoBehaviour {
    private LayerMask _filterMask;
    private int _assetLayer = 6;
    private int _uiLayer = 5;
    // Use this to ensure that the Gizmos are being drawn when in Play Mode
    private bool _detectionStarted = true;

    private void Awake() {
        _filterMask = LayerMask.GetMask("Asset");
        CheckAssetOnUI();
    }

    private void Start() {
        CheckAssetCollisions();
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        if (_detectionStarted) {
            // Draw a cube where the OverlapBox is (positioned where the GameObject and with
            // identical size).
            Gizmos.DrawWireCube(transform.position, transform.localScale);
        }
    }

    /// <summary>
    /// Checks for all collisions during placement of a new map object and handles them.
    /// Handling involves turning the map object's material to an error material and
    /// calling a corountine to revert the materials and destroy the map object after an
    /// appropriate amount of time.
    /// </summary>
    public void CheckAssetCollisions() {
        // All created GameObjects have a parent container except dynamic bounding boxes
        if (gameObject.transform.parent == null) {
            return;
        }
        List<Collider2D> hitColliders = GetAssetCollisions();
        if (GetCollisionCount() > 1) {
            foreach (Collider2D collisionObject in hitColliders) {
                if (collisionObject.gameObject.layer == _assetLayer
                    && collisionObject.gameObject.GetComponent<Image>() != null
                    && collisionObject.gameObject.tag != "DynamicBoundingBox") {
                    collisionObject.gameObject.GetComponent<Image>()
                        .color = Color.red;
                    StartCoroutine(RevertMaterialAndDestroy(collisionObject.gameObject));
                }
            }
        }
    }

    /// <summary>
    /// Determine whether the current <c>GameObject</c> is in direct collision with at least one
    /// dynamic bounding box.
    /// </summary>
    /// <returns>
    /// <c>bool</c> representing whether the <c>GameObject</c> collides with a dynamic bounding box
    /// </returns>
    public bool GetDynamicCollision() {
        List<Collider2D> hitColliders = GetAssetCollisions();
        foreach (Collider2D collisionObject in hitColliders) {
            if (collisionObject.gameObject.tag == "DynamicBoundingBox"
                    && collisionObject.gameObject != gameObject) {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Retrieve the number of collisions that the <c>GameObject</c> is in direct collision with.
    /// </summary>
    /// <returns>
    /// <c>int</c> corresponding to the number of collisions that occur with the <c>GameObject</c>
    /// </returns>
    public int GetCollisionCount() {
        List<Collider2D> hitColliders = GetAssetCollisions();
        int numCollisions = 0;
        foreach (Collider2D collider in hitColliders) {
            if (collider.gameObject.tag != "DynamicBoundingBox") {
                numCollisions++;
            }
        }
        return numCollisions;
    }

    /// <summary>
    /// Retrieve an array of Collider2D that the <c>GameObject</c> is in direct collision with.
    /// </summary>
    /// <returns>
    /// Array of <c>Collider2D</c> corresponding to the collisions that occur with the
    /// <c>GameObject</c>
    /// </returns>
    public List<Collider2D> GetAssetCollisions() {
        List<Collider2D> hitColliders = new List<Collider2D>();
        ContactFilter2D filter2D = new ContactFilter2D();
        filter2D.SetLayerMask(_filterMask);
        int collisions = GetComponent<Collider2D>().OverlapCollider(filter2D, hitColliders);
        foreach (Collider2D collider in hitColliders) {
            if (collider.gameObject == gameObject) {
                return hitColliders;
            }
        }

        List<Collider2D> allColliders = new List<Collider2D>();
        for (int i = 0; i < hitColliders.Count; i++) {
            allColliders.Add(hitColliders[i]);
        }
        allColliders.Add(gameObject.GetComponent<Collider2D>());
        return allColliders;
    }

    /// <summary>
    /// Checks asset placement and ensures that assets cannot be placed over UI elements.
    /// </summary>
    private void CheckAssetOnUI() {
        if (IsInvalidPlacement()) {
            MapEditorManager.MapObjects.Remove(gameObject.GetInstanceID());
            Destroy(gameObject.transform.parent);
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Changes a map object's material back to its original material from <c>_errorMaterial</c>
    /// and destroys the placed asset causing the collision. This is required during collision
    /// handling.
    /// </summary>
    /// <param name="collisionObject">
    /// <c>GameObject</c> that is experiencing collision, to be highlighted briefly.
    /// </param>
    IEnumerator RevertMaterialAndDestroy(GameObject collisionObject) {
        yield return new WaitForSecondsRealtime(0.5f);
        collisionObject.gameObject.GetComponent<Image>().color = Color.white;

        if (collisionObject.gameObject == gameObject) {
            MapEditorManager.MapObjects.Remove(gameObject.GetInstanceID());
            GameObject parent = gameObject.transform.parent.gameObject;
            Destroy(gameObject);
            Destroy(parent);
        }
    }

    /// <summary>
    /// Checks if the current mouse position would place an asset down illegally
    /// </summary>
    public bool IsInvalidPlacement() {
        RayLibrary rayLib = new RayLibrary();
        return (rayLib.IsPointerOverLayer(_uiLayer));
    }
}