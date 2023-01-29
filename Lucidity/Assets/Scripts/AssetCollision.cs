using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetCollision : MonoBehaviour{

    [SerializeField] private LayerMask _filterMask;
    private bool _detectionStarted;
    private Material _originalMaterial;
    [SerializeField] private Material _errorMaterial;

    void Start(){
        //Use this to ensure that the Gizmos are being drawn when in Play Mode.
      _detectionStarted = true;
      CheckCollisions();
    }

    void CheckCollisions(){
      Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, 
        transform.localScale / 2, Quaternion.identity, _filterMask);
      if (hitColliders.Length > 1){
        foreach (Collider collisionObject in hitColliders){
          _originalMaterial = collisionObject.gameObject.GetComponent<MeshRenderer>().material;
          collisionObject.gameObject.GetComponent<MeshRenderer>().material = _errorMaterial;
          StartCoroutine(ChangeMaterialBack(_originalMaterial, collisionObject.gameObject));
        }
      }  
    }

    IEnumerator ChangeMaterialBack(Material _originalMaterial, GameObject collisionObject){
      yield return new WaitForSecondsRealtime(0.5f);
      collisionObject.gameObject.GetComponent<MeshRenderer>().material = _originalMaterial;
      if (collisionObject == gameObject){
        Destroy(gameObject);
      }
    }

    //Draw the Box Overlap as a gizmo to show where it currently is testing. Click the Gizmos button to see this
    void OnDrawGizmos(){
      Gizmos.color = Color.red;
      if (_detectionStarted){
        //Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)
        Gizmos.DrawWireCube(transform.position, transform.localScale);
      }
    }
}