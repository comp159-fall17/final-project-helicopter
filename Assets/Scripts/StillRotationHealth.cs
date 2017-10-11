using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StillRotationHealth : MonoBehaviour {
	
	Vector3 stillRotation = new Vector3(0.0f, 0.0f, 0.0f);

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.rotation = Quaternion.LookRotation(stillRotation);
	}
}
