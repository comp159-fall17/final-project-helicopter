﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementFall : MonoBehaviour {

    public float rotateSpeed;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        this.transform.Rotate(Vector3.up * rotateSpeed);
	}
}
