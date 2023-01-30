using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetCollision : MonoBehaviour {

    // Use this to ensure that the Gizmos are being drawn when in Play Mode.
    private bool _detectionStarted = true;

    [SerializeField] private LayerMask _filterMask;
    private Material _originalMaterial;
    [SerializeField] private Material _errorMaterial;

    void Start() {
      CheckCollisions();
    }

    /// <summary>
    /// Checks for all collisions during placement of a new mapobject and handles them.
    /// Handling involves turning the mapobject's material to an error material and
    /// calling the a corountine to revert the materials and destroy the map object
    /// </summary>
    void CheckCollisions() {
      Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, 
        transform.localScale / 2, Quaternion.identity, _filterMask);
      if (hitColliders.Length > 1){
        foreach (Collider collisionObject in hitColliders){
          _originalMaterial = collisionObject.gameObject.GetComponent<MeshRenderer>().material;
          collisionObject.gameObject.GetComponent<MeshRenderer>().material = _errorMaterial;
          StartCoroutine(RevertMaterialAndDestroy(_originalMaterial, collisionObject.gameObject));
        }
      }  
    }

    /// <summary>
    /// Changes a mapobject's material back to the original material from the error material and
    /// destroys the placed asset causing the collision
    /// Required during collision handling
    /// </summary>
    IEnumerator RevertMaterialAndDestroy(Material _originalMaterial, GameObject collisionObject) {
      yield return new WaitForSecondsRealtime(0.5f);
      collisionObject.gameObject.GetComponent<MeshRenderer>().material = _originalMaterial;
      if (collisionObject == gameObject){
        Destroy(gameObject);
      }
    }

    /// <summary>
    /// Draws the Box Overlap as a gizmo to show where it currently is testing. 
    /// Click the Gizmos button to see this
    /// </summary>
    void OnDrawGizmos() {
      Gizmos.color = Color.red;
      if (_detectionStarted){
        //Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)
        Gizmos.DrawWireCube(transform.position, transform.localScale);
      }
    }
}