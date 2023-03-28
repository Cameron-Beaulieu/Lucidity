using RaycastingLibrary;
using System;
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
    public static List<List<MapObject>> LayerCollisions = new List<List<MapObject>>();
    private bool _isCollidingAfterRotation = false;

    public bool IsCollidingAfterRotation {
        get { return _isCollidingAfterRotation; }
    }

    private void Awake() {
        _filterMask = LayerMask.GetMask("Asset");
        // CheckAssetOnUI();
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
                    && collisionObject.gameObject.tag != "DynamicBoundingBox"
                    && (LayerCollisions.Count == 0 || collisionObject.gameObject.GetInstanceID() 
                    != LayerCollisions[LayerCollisions.Count -1][0].Id)) {
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
    public List<Collider2D> GetAssetCollisions(bool isRotating = false) {
        List<Collider2D> hitColliders = new List<Collider2D>();
        ContactFilter2D filter2D = new ContactFilter2D();
        filter2D.SetLayerMask(_filterMask);
        int collisions = GetComponent<Collider2D>().OverlapCollider(filter2D, hitColliders);
        List<Collider2D> hitCollidersClone = new List<Collider2D>(hitColliders);
        foreach (Collider2D collider in hitColliders) {
            if (collider.gameObject == gameObject) {
                return hitCollidersClone;
            }
            if (CheckMapObjectStackingValidity(collider.gameObject, isRotating) && 
                collider.gameObject != gameObject && hitColliders.Count == 2 || 
                MapEditorManager.Reversion || MapEditorManager.LoadFlag) {
                Debug.Log("Collision between mountain and tree");

                int layerIndex1 = MapEditorManager.LayerContainsMapObject(
                    collider.gameObject.GetInstanceID());
                int layerIndex2 = MapEditorManager.LayerContainsMapObject(
                    gameObject.GetInstanceID());

                MapObject obj1 = MapEditorManager.MapObjects[collider.gameObject.GetInstanceID()];
                MapObject obj2 = MapEditorManager.MapObjects[gameObject.GetInstanceID()];

                int last = LayerCollisions.Count - 1;
                if (!MapEditorManager.Reversion) {
                    if (layerIndex1 < layerIndex2) {
                        if (LayerCollisions.Count == 0 || 
                            !LayerCollisionsContainsList(obj1.Id, obj2.Id)) {
                            LayerCollisions.Add(new List<MapObject>() {obj1, obj2});
                        }
                    } else {
                        if (LayerCollisions.Count == 0 || 
                            !LayerCollisionsContainsList(obj2.Id, obj1.Id)) {
                            LayerCollisions.Add(new List<MapObject>() {obj2, obj1});
                        }
                    }
                }
                Debug.Log("removing collider of " + collider.gameObject.name);
                hitCollidersClone.Remove(collider);
            }
        }

        if (!hitCollidersClone.Contains(gameObject.GetComponent<Collider2D>())) {
            hitCollidersClone.Add(gameObject.GetComponent<Collider2D>());
        }

        return hitCollidersClone;
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
            MapEditorManager.Layers[MapEditorManager.LayerContainsMapObject(
                gameObject.GetInstanceID())].Remove(gameObject.GetInstanceID());
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

    /// <summary>
    /// Checks to see if the collision between two <c>MapObjects</c> is a legal stacking
    /// interaction or not
    /// </summary>
    /// <param name="collisionObject">
    /// <c>GameObject</c> that is experiencing collision.
    /// </param>
    /// <returns>
    /// <c>true</c> if the gameObject is stacked legally, <c>false</c> otherwise
    /// </returns>
    private bool CheckMapObjectStackingValidity(GameObject collisionObject, bool isRotating = false) {
        Debug.Log("isRotating: " + isRotating);
        if (isRotating) {
            int gameObjectLayer = MapEditorManager.LayerContainsMapObject(gameObject.GetInstanceID());
            int collisionObjectLayer = MapEditorManager.LayerContainsMapObject(
                    collisionObject.GetInstanceID());
            
            if (gameObjectLayer == -1 || collisionObjectLayer == -1) {
                Debug.Log("Invalid layer");
                return false;
            }

            MapObject gameObjectMapObject = MapEditorManager.Layers[gameObjectLayer][
                gameObject.GetInstanceID()];
            MapObject collisionMapObject = MapEditorManager.Layers[collisionObjectLayer][
                    collisionObject.GetInstanceID()];
            
            if (collisionObjectLayer <= gameObjectLayer || 
                collisionMapObject.Name != "Tree" || 
                gameObjectMapObject.Name != "Mountain" || 
                !IsFullyEncompassed(gameObject, collisionObject)) { 
                    Debug.Log(gameObjectLayer + " " + collisionObjectLayer + " " + gameObjectMapObject.Name + " " + collisionMapObject.Name);
                return false;
            }

            return true;
        } else {
            int newObjectLayer = MapEditorManager.LayerContainsMapObject(gameObject.GetInstanceID());
            int collisionObjectLayer = MapEditorManager.LayerContainsMapObject(
                    collisionObject.GetInstanceID());

            if (newObjectLayer == -1 || collisionObjectLayer == -1) {
                Debug.Log("Invalid layer1");
                return false;
            }

            MapObject newMapObject = MapEditorManager.Layers[newObjectLayer][
                gameObject.GetInstanceID()];
            MapObject collisionMapObject = MapEditorManager.Layers[collisionObjectLayer][
                    collisionObject.GetInstanceID()];

            if (newObjectLayer <= collisionObjectLayer || newMapObject.Name != "Tree" || 
                collisionMapObject.Name != "Mountain" || !IsFullyEncompassed(collisionObject, gameObject)) { 
                Debug.Log("invalid stacking");
                return false;
            }

            Debug.Log("return true from stacking");
            return true;
        }
    }

    /// <summary>
    /// Checks to see if the current gameObject is fully encompassed by the colliding objects
    /// collider or only partially encompassed.
    /// </summary>
    /// <param name="collisionObject">
    /// <c>GameObject</c> that is experiencing collision.
    /// </param>
    /// <returns>
    /// <c>true</c> if the gameObject is fully encompassed, <c>false</c> otherwise
    /// </returns>
    private bool IsFullyEncompassed(GameObject mountainObject, GameObject treeObject) {
        foreach (Vector2 point in treeObject.GetComponent<PolygonCollider2D>().points) {
            Vector3 newPoint = treeObject.GetComponent<PolygonCollider2D>().bounds.center + 
                new Vector3(point.x, point.y, 0);
            if (!mountainObject.GetComponent<PolygonCollider2D>().bounds.IntersectRay(new Ray(
                    newPoint, new Vector3(0,0, 1)))) {
                Debug.Log("Not fully encompassed");
                return false;
            }
        }
        return true;
    }

    private bool LayerCollisionsContainsList(int id1, int id2) {
        foreach (List<MapObject> mapObjects in LayerCollisions) {
            if (mapObjects[0].Id == id1 && mapObjects[1].Id == id2) {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Checks if the scaling of the current <c>MapObject</c> would cause a collision with
    /// another <c>MapObject</c> or not.
    /// </summary>
    /// <param name="originalScale">
    /// The original scale of the <c>MapObject</c> before scaling.
    /// </param>
    /// <param name="scalingObject">
    /// The <c>GameObject</c> that is being scaled.
    /// </param>
    /// <returns>
    /// <c>true</c> if the scaling would cause a collision, <c>false</c> otherwise.
    /// </returns>
    public bool ScaleCausesCollision(float originalScale, GameObject scalingObject) {
        List<Collider2D> hitColliders = GetAssetCollisions();
        if (GetCollisionCount() > 1) {
            foreach (Collider2D collisionObject in hitColliders) {
                if (collisionObject.gameObject.layer == _assetLayer
                    && collisionObject.gameObject.GetComponent<Image>() != null
                    && collisionObject.gameObject.tag != "DynamicBoundingBox"
                    && (LayerCollisions.Count == 0 || collisionObject.gameObject.GetInstanceID() 
                    != LayerCollisions[LayerCollisions.Count -1][0].Id)) {
                    collisionObject.gameObject.GetComponent<Image>()
                        .color = Color.red;
                    StartCoroutine(RevertMaterialAndScale(originalScale, scalingObject, collisionObject.gameObject));
                }
            }
            return true;
        }
        return false;
    }

    /// <summary>
    /// Changes a <c>MapObject</c>'s material back to its original color from red and scales
    /// the placed asset causing the collision back down to the original scale before the 
    /// collision.
    /// </summary>
    /// <param name="originalScale">
    /// The original scale of the <c>MapObject</c> before the collision.
    /// </param>
    /// <param name="scalingObject">
    /// <c>GameObject</c> that was scaled and caused the collision.
    /// </param>
    /// <param name="collisionObject">
    /// <c>GameObject</c> that is colliding with the scaled object.
    /// </param>
    private IEnumerator RevertMaterialAndScale(float originalScale, GameObject scalingObject, GameObject collisionObject) {
        yield return new WaitForSecondsRealtime(0.5f);
        collisionObject.gameObject.GetComponent<Image>().color = Color.white;

        if (collisionObject.gameObject == scalingObject) {
            collisionObject.gameObject.GetComponent<Image>().color = new Color32(73, 48, 150, 255);
            collisionObject.transform.parent.localScale = 
                new Vector3(Util.ParentAssetDefaultScale * originalScale, 
                            Util.ParentAssetDefaultScale * originalScale, 
                            Util.ParentAssetDefaultScale * originalScale);
        }
    }

    /// <summary>
    /// Checks if any collisions occur as a result of rotating the specified <c>GameObject</c>.
    /// </summary>
    /// <param name="isClockwise">
    /// <c>true</c> if the rotation is clockwise, <c>false</c> otherwise.
    /// </param>
    /// <param name="rotatingObject">
    /// <c>GameObject</c> that is being rotated.
    /// </param>
    /// <param name="originalRotation">
    /// The original rotation of the <c>GameObject</c> before the rotation.
    /// </param>
    /// <param name="newRotation">
    /// The new rotation of the <c>GameObject</c> after the rotation.
    /// </param>
    /// <param name="callback">
    /// The callback function to be called after the coroutine is finished.
    /// </param>
    public IEnumerator CheckCollisionsAfterRotation(bool isClockwise, GameObject rotatingObject,  
                                                    System.Action<bool, bool> callback) {
        yield return new WaitForFixedUpdate(); 
        List<Collider2D> hitColliders = GetAssetCollisions(true);
        if (GetCollisionCount() > 1) {
            foreach (Collider2D collisionObject in hitColliders) {
                if (collisionObject.gameObject.layer == _assetLayer
                    && collisionObject.gameObject.GetComponent<Image>() != null
                    && collisionObject.gameObject.tag != "DynamicBoundingBox"
                    && (LayerCollisions.Count == 0 || collisionObject.gameObject.GetInstanceID() 
                    != LayerCollisions[LayerCollisions.Count -1][0].Id)) {
                    Debug.Log("collisionObject: " + collisionObject.gameObject.name);
                    collisionObject.gameObject.GetComponent<Image>()
                        .color = Color.red;
                    StartCoroutine(RevertMaterialAndRotate(isClockwise, rotatingObject,
                                                           collisionObject.gameObject));
                }
            }
            callback(true, isClockwise);
        } else {
            callback(false, isClockwise);
        }
    }

    /// <summary>
    /// Reverts the material of the <c>GameObject</c> that is colliding with the rotating object
    /// and reverts the rotation of the parent object that caused the collision.
    /// </summary>
    /// <param name="isClockwise">
    /// <c>true</c> if the rotation is clockwise, <c>false</c> otherwise.
    /// </param>
    /// <param name="rotatingObject">
    /// <c>GameObject</c> that was rotated and caused the collision.
    /// </param>
    /// <param name="collisionObject">
    /// <c>GameObject</c> that is colliding with the rotating object.
    /// </param>
    private IEnumerator RevertMaterialAndRotate(bool isClockwise, GameObject rotatingObject, 
                                                GameObject collisionObject) {
        yield return new WaitForSecondsRealtime(0.5f);
        collisionObject.gameObject.GetComponent<Image>().color = Color.white; 

        if (collisionObject.gameObject == rotatingObject) {
            collisionObject.gameObject.GetComponent<Image>().color = new Color32(73, 48, 150, 255);
            // revert the rotation of the parent object depending on whether the applied rotation 
            // was clockwise or counter-clockwise
            if (isClockwise) {
                rotatingObject.transform.parent.Rotate(0, 0, 90);
            } else {
                rotatingObject.transform.parent.Rotate(0, 0, -90);
            }
        }
    }
}