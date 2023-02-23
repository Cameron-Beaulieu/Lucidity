using RaycastingLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AssetCollision : MonoBehaviour {
    [SerializeField] private Material _errorMaterial;
    private Material _originalMaterial;
    private LayerMask _filterMask;
    private int _assetLayer = 6;
    private int _uiLayer = 5;
    // Use this to ensure that the Gizmos are being drawn when in Play Mode
    private bool _detectionStarted = true;

    private void Awake() {
        _filterMask = LayerMask.GetMask("Asset");
        CheckAssetOnUI();
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
        if (gameObject.tag != "DynamicBoundingBox") {
            return;
        }
        Collider[] hitColliders = GetAssetCollisions();
        if (GetCollisionCount() > 1) {
            gameObject.tag = "CollisionObject";
            foreach (Collider collisionObject in hitColliders) {
                if (collisionObject.gameObject.layer == _assetLayer
                    && collisionObject.gameObject.GetComponent<MeshRenderer>() != null) {
                    _originalMaterial = collisionObject.gameObject.GetComponent<MeshRenderer>()
                        .material;
                    collisionObject.gameObject.GetComponent<MeshRenderer>()
                        .material = _errorMaterial;
                    StartCoroutine(RevertMaterialAndDestroy(_originalMaterial,
                                                            collisionObject.gameObject));
                }
            }
            MapEditorManager.LastEncounteredObject = hitColliders[0].gameObject;
        }
    }

    /// <summary>
    /// Retrieve the number of collisions that the <c>GameObject</c> is in direct collision with.
    /// </summary>
    /// <returns>
    /// <c>int</c> corresponding to the number of collisions that occur with the <c>GameObject</c>
    /// </returns>
    public int GetCollisionCount() {
        Collider[] hitColliders = GetAssetCollisions();
        int collisions = hitColliders.Length;
        foreach (Collider collisionObject in hitColliders) {
            // If an object is labeled with the "CollisionObject" tag, then it can be considered as
            // not colliding, as it will not be placed because of legality.
            if (collisionObject.tag == "CollisionObject") {
                collisions--;
            }
        }
        return collisions;
    }

    /// <summary>
    /// Retrieve an array of Collider that the <c>GameObject</c> is in direct collision with.
    /// </summary>
    /// <returns>
    /// Array of <c>Collider</c> corresponding to the collisions that occur with the
    /// <c>GameObject</c>
    /// </returns>
    public Collider[] GetAssetCollisions() {
        Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position,
                                                     transform.localScale / 2, Quaternion.identity,
                                                     _filterMask);
        return hitColliders;
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
    /// <param name="_originalMaterial">
    /// <c>Material</c> corresponding to the collision <c>GameObject</c>.
    /// </param>
    /// <param name="collisionObject">
    /// <c>GameObject</c> that is experiencing collision, to be highlighted briefly.
    /// </param>
    IEnumerator RevertMaterialAndDestroy(Material _originalMaterial, GameObject collisionObject) {
        yield return new WaitForSecondsRealtime(0.5f);
        if (collisionObject.gameObject.name == "Spawn Point") {
            // spawn point doesn't have material (would hide the sprite)
            collisionObject.gameObject.GetComponent<MeshRenderer>().materials = new Material[]{};
        } else {
            collisionObject.gameObject.GetComponent<MeshRenderer>().material = _originalMaterial;
        }
        if (collisionObject == gameObject) {
            MapEditorManager.MapObjects.Remove(gameObject.GetInstanceID());
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