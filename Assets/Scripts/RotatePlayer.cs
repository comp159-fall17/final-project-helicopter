using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatePlayer : MonoBehaviour {
	void Update () {
		float moveHorizontal = Input.GetAxis("Horizontal");
		float moveVertical = Input.GetAxis("Vertical");

		// TODO: Change to mouse position
		Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
		transform.rotation = Quaternion.LookRotation(movement);
	}
}
