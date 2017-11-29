using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldController : MonoBehaviour {
    void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Bullet") {
            Destroy(other.gameObject); //if the player is too close to an enemy, bullet hits may not be detected
        }
    }
}
