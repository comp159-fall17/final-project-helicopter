using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnDoors : MonoBehaviour {
    private GameObject door;
    private bool spawn = true;

    void OnTriggerEnter()
    {
        LevelGen lvlspawn = GameObject.Find("LevelSpawner").GetComponent<LevelGen>();
        door = lvlspawn.Door;
        if (spawn)
        {
            Instantiate(door, transform.position, transform.rotation, transform);
            spawn = false;
        }
    }
}
