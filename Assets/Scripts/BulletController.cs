﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {
    public float bulletSpeed;
    public float distance;

    Rigidbody body;
    Vector3 origin;

    void Start() {
        origin = transform.position;

        // shoots in the bullet's positive x direction
        body = gameObject.GetComponent<Rigidbody>();
        body.AddForce(transform.up * bulletSpeed);

        body.sleepThreshold = 100f;
    }

    void FixedUpdate() {
        if (Vector3.Distance(origin, transform.position) > distance) {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other) {
        switch (other.gameObject.tag) {
        case "Player":
            TriggerPlayer(other);
            break;
        case "Enemy":
            TriggerEnemy(other);
            break;
        }
    }

    void TriggerPlayer(Collider player) {
        Destroy(gameObject);
    }

    void TriggerEnemy(Collider enemy) {
        Destroy(gameObject);
    }
}
