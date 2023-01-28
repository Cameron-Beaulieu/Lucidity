using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetCollision : MonoBehaviour
{

    public Vector3 scanRadius;
    public LayerMask filterMask;
    Collider[] checkCollider; 
    bool m_Started;
    public Material originalMaterial;
    public Material errorMaterial;

    void Start()
    {
        //Use this to ensure that the Gizmos are being drawn when in Play Mode.
        m_Started = true;
        originalMaterial = GetComponent<MeshRenderer>().material;
        CheckCollisions();
    }

    void Update(){
        
    }

    void CheckCollisions()
    {
        Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, transform.localScale / 2, Quaternion.identity, filterMask);
        Debug.Log(hitColliders);
        if(hitColliders.Length > 1){
            Destroy(gameObject);
            foreach (Collider collisionObject in hitColliders){
                originalMaterial = collisionObject.gameObject.GetComponent<MeshRenderer>().material;
                collisionObject.gameObject.GetComponent<MeshRenderer>().material = errorMaterial;
                Debug.Log("Outside Coroutine: " + originalMaterial);
                StartCoroutine(ChangeMaterialBack(originalMaterial, collisionObject.gameObject));
            }
        }
    }

    IEnumerator ChangeMaterialBack(Material originalMaterial, GameObject collisionObject)
    {
        Debug.Log("Starting timer");
        Debug.Log("Inside Coroutine: " + originalMaterial);
        yield return new WaitForSecondsRealtime(2);
        collisionObject.gameObject.GetComponent<MeshRenderer>().material = originalMaterial;
        Debug.Log("Material Reverted");
    }

    //Draw the Box Overlap as a gizmo to show where it currently is testing. Click the Gizmos button to see this
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (m_Started)
            //Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)
            Gizmos.DrawWireCube(transform.position, transform.localScale);
    }

}