using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Grenade))]
public class Exploder : Tackler {
    Grenade Explosion;

    protected override void Start() {
        base.Start();

        Explosion = GetComponent<Grenade>();

        // disable automatic actions
        Explosion.countdown = Mathf.Infinity;
        Explosion.throwForce = 0;
    }

    protected override void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.Equals(GameManager.Instance.Player)) {
            Explosion.Explode();
            Destroy(gameObject); // sometimes it doesn't go through
        }
    }

    protected override void Die() {
        GameManager.Instance.EnemyHasDied(transform);
        GameManager.Instance.PlayDeathSound();
        Destroy(gameObject);
    }
}
