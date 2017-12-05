using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewFloorDoor : MonoBehaviour {

    private LevelGen lg;

	// Use this for initialization
	void Start () {
        lg = GameObject.Find("LevelSpawner").GetComponent<LevelGen>();

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            lg.RemoveFloor();
            lg.NewFloor();
        }
    }
}
