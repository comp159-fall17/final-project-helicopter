using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour {
    public static bool CanCross;
    public int moveDistance;
    private float rotation;
    private float playerRotatation;

    void OnTriggerEnter(Collider other) {
        if (CanCross && other.tag == "Player") {
            other.transform.rotation = new Quaternion(0, 0, 0, 0); //Temp fix for player rotation through doors. Imporve if there is extra time.
            rotation = this.transform.rotation.y;
            if (rotation == 0) { //Up
                other.transform.Translate(0, 0, moveDistance);
            } else if (rotation == 1) { //Down
                other.transform.Translate(0, 0, -moveDistance);
            } else if (rotation > 0) { //Right
                other.transform.Translate(moveDistance, 0, 0);
            } else if (rotation < 0) { //Left
                other.transform.Translate(-moveDistance, 0, 0);
            }
        }
    }
}