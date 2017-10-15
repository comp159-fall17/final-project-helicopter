using System.Linq;
using UnityEngine;

public class Cannonball : BulletController {
    public GameObject ExplosionPrefab;

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
        Destroy(Instantiate(ExplosionPrefab, transform.position, Quaternion.identity),
                ExplosionPrefab.GetComponent<Animation>().clip.length);

        // find surrounding shooters
        Shooter[] hits = Physics.OverlapSphere(transform.position, radius)
                                .Select(i => i.gameObject)
                                .Where(j => j.layer == LayerMask.NameToLayer("Player"))
                                .Select(k => k.GetComponent<Shooter>())
                                .ToArray();

        foreach (Shooter hit in hits) {
            // take health away
            hit.Hit(damage * 1000);

            // send explosion force
            hit.Body.AddExplosionForce(damage * 500, transform.position, radius);
        }

        Destroy(gameObject);
    }
}
