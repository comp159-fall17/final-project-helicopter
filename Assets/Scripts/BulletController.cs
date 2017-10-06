using System.Collections;
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

        RaycastHit hit;
        if (Physics.Raycast(body.position, body.velocity, out hit,
                            body.velocity.magnitude * Time.deltaTime)) {
            OnTriggerEnter(hit.collider);
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ignore Raycast")) {
            return;
        }

        switch (other.gameObject.tag) {
        case "Player":
            TriggerPlayer(other);
            break;
        case "Enemy":
            TriggerEnemy(other);
            break;
        default:
            Destroy(gameObject);
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
