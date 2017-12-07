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
        }
    }
}
