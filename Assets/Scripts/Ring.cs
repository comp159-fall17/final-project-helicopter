using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ring : MonoBehaviour {

    public float lifetime;
    public float growthRate;
    public float damage = 30.0f;

	// Use this for initialization
	void Start () {
        Destroy(gameObject, lifetime);
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 scale = transform.localScale;

        scale.x += growthRate;
        scale.z += growthRate;

        transform.localScale = scale;
	}

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") && other.gameObject.CompareTag("Enemy")) {
            other.gameObject.GetComponent<Shooter>().Hit(damage);
        }
    }
}
