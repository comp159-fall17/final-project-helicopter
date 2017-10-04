using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {
    private Rigidbody rb;

    public float bulletSpeed;
    public float lifetime;

	// Use this for initialization
	void Start() {
        Destroy(this.gameObject, lifetime); //destroy this object after a certain amount of time

        rb = gameObject.GetComponent<Rigidbody>();
        rb.AddForce(transform.up * bulletSpeed); //shoots in the bullet's positive x direction
	}
	
	// Update is called once per frame
	void Update() {
		
	}
}
