using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoomPortal : MonoBehaviour {

    private Transform node;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            // teleport to Far Away where the boss room is
            Vector3 somewhere = LevelGen.ReallyFarAway;
            somewhere.y = 1;
            somewhere.x -= 10;

            other.transform.position = somewhere;
        }
    }
}
