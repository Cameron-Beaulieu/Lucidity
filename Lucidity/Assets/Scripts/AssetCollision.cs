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
    private void CheckAssetCollisions() {
        Collider2D collider2D = gameObject.GetComponent<Collider2D>();
        List<Collider2D> hitColliders2D = new List<Collider2D>();
        ContactFilter2D filter2D = new ContactFilter2D();
        filter2D.SetLayerMask(_filterMask);
        collider2D.OverlapCollider(filter2D, hitColliders2D);
        Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position,
                                                     transform.localScale / 2, Quaternion.identity,
                                                     _filterMask);
        int collisions = hitColliders2D.Count;
        foreach (Collider2D collisionObject in hitColliders2D) {
            // If an object is labeled with the "CollisionObject" tag, then it can be considered as
            // not colliding, as it will not be placed because of legality.
            if (collisionObject.tag == "CollisionObject") {
                collisions--;
            }
        }
        if (collisions > 0) {
            hitColliders2D.Add(gameObject.GetComponent<Collider2D>());
            gameObject.tag = "CollisionObject";
            foreach (Collider2D collisionObject in hitColliders2D) {
                if (collisionObject.gameObject.layer == _assetLayer
                    && collisionObject.gameObject.GetComponent<Image>() != null) {
                    collisionObject.gameObject.GetComponent<Image>().color = Color.red;
                    StartCoroutine(RevertMaterialAndDestroy(collisionObject.gameObject));
                }
            }
            MapEditorManager.LastEncounteredObject = hitColliders2D[0].gameObject;
        }
    }

    /// <summary>
    /// Checks asset placement and ensures that assets cannot be placed over UI elements.
    /// </summary>
    private void CheckAssetOnUI() {
        if (IsInvalidPlacement()) {
            MapEditorManager.MapObjects.Remove(gameObject.GetInstanceID());
            Destroy(gameObject.transform.parent.gameObject);
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
            Destroy(gameObject.transform.parent.gameObject);
            Destroy(gameObject);
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