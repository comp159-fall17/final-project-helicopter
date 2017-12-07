using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoomPortal : MonoBehaviour {

    private Transform node;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.transform.position = new Vector3(500, 1, 500); //Teleport player to 500, 500
        }
    }
}
