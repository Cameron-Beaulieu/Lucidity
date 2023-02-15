using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarPlacement : MonoBehaviour {
    
    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Asset") {
            Debug.Log("Collided with asset");
            // transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        } else {
            Debug.Log("Collided with something else");
        }
    }
}
