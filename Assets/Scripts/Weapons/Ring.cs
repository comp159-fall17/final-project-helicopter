using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ring : MonoBehaviour {

    public float lifetime;
    public float growthRate;
    public float damage = 30.0f;

    void Start () {
        Destroy(gameObject, lifetime);
	}
	
	void Update () {
        Vector3 scale = transform.localScale;

        scale.x += growthRate;
        scale.z += growthRate;

        transform.localScale = scale;
	}

    void OnTriggerEnter(Collider other) {
        Shooter hit = other.gameObject.GetComponent<Shooter>();
        if (hit != null) {
            hit.Hit(damage);
        }
    }
}
