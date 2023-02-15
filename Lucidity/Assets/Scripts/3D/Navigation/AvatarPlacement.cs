using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarPlacement : MonoBehaviour {

    private Grid _mapGrid;

    void Start() {
        // _mapGrid = GameObject.FindGameObjectWithTag("3DMap").GetComponent<Grid>();
        // isColliding = GetCollisionsWithMapObjects().Length > 0;
        // while (isColliding) {
        //     transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        //     isColliding = GetCollisionsWithMapObjects().Length > 0;
        // }
    }
    
    // void OnCollisionEnter(Collision collision) {
    //     if (collision.gameObject.layer == LayerMask.NameToLayer("Asset")) {
    //         Debug.Log("Collided with asset");
    //         // transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    //     } else {
    //         Debug.Log("Collided with something else");
    //     }
    // }

    // private Collider[] GetCollisionsWithMapObjects() {
    //     Collider[] hitColliders = Physics.OverlapBox(transform.position, transform.localScale/2, Quaternion.identity, _filterMask);
    //     Debug.Log(hitColliders.Length);
    //     return hitColliders;

    // }
}
