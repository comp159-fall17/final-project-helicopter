using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableNode : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter() //Should disable the node if a room spawns on it
    {
        gameObject.SetActive(false);
    }
}
