using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {
    public float bulletSpeed;
    public float lifetime;

    Rigidbody body;

    void Start() {
        // shoots in the bullet's positive x direction
        body = gameObject.GetComponent<Rigidbody>();
        body.AddForce(transform.up * bulletSpeed);
    }

    void FixedUpdate() {
        if (Mathf.Abs(body.velocity.magnitude) < Mathf.Epsilon) {
            Destroy(gameObject, lifetime);
        }
    }

    void OnCollisionEnter(Collision collision) {
        string otherTag = collision.gameObject.tag;
        //if (otherTag != "Ignore") {
        //    Destroy(gameObject);
        //}
    }
}
