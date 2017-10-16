using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatePlayer : MonoBehaviour {

	private PlayerControls PController;
	private GameObject PControllerRef;

	void Start(){
		// These help reference gameObjects in hierarchy, can now use methods from other classes
		PControllerRef = GameObject.Find ("Player");
		PController = PControllerRef.GetComponent<PlayerControls> ();
	}

	void Update () {
		// Rotates based on mouse location
		transform.rotation = Quaternion.Euler(0.0f, PController.AbsTargetAngle ()-75f, 0.0f);

	}


}
