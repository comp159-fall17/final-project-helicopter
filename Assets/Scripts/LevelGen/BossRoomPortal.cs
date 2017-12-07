using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoomPortal : MonoBehaviour {
    public Vector3 offset;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (LevelGen.Instance.CurrentFloor == 0){
                GameObject.FindGameObjectWithTag("BossTeir1").SetActive(true);
            }
            else if (LevelGen.Instance.CurrentFloor == 1)
            {
                GameObject.FindGameObjectWithTag("BossTeir2").SetActive(true);
            }
            else if (LevelGen.Instance.CurrentFloor == 2)
            {
                GameObject.FindGameObjectWithTag("BossTeir3").SetActive(true);
            }
            else if (LevelGen.Instance.CurrentFloor == 3)
            {
                GameObject.FindGameObjectWithTag("BossTeir4").SetActive(true);
            }

            // teleport to Far Away where the boss room is
            Vector3 somewhere = LevelGen.ReallyFarAway;
            somewhere.y = 1;

            // add offset to the whole thing
            somewhere.x += offset.x;
            somewhere.z += offset.z;

            other.transform.position = somewhere;
        }
    }
}
