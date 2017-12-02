using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnDoors : MonoBehaviour {

    private GameObject door;
    private bool spawn = true;

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter()
    {
        door = GameObject.Find("LevelSpawner").GetComponent<LevelGen>().door;
        if (spawn)
        {
            Instantiate(door, this.transform.position, this.transform.rotation, this.transform);
            spawn = false;
        }
    }
}
