using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannonball : BulletController {
    public float countdown = 3f;
    public float radius = 3f;
    public int damage = 5;

    protected override void Start() {
        base.Start();

        Invoke("Explode", countdown);
    }

    protected override void OnTriggerEnter(Collider other) {
        Explode();
    }

    void Explode() {
        // TODO: implement explosion

        // instantiate explosion animation 
        // find surrounding shooters
        // take health away

        Destroy(gameObject);
    }
}
