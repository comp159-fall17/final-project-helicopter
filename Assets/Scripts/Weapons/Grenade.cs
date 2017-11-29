using System.Linq;
using UnityEngine;

public class Grenade : SpecialWeapon {
    public GameObject ExplosionPrefab;

    public float countdown = 3f;
    public float radius = 3f;
    public int damage = 5;
    public int throwForce = 1000;

    void Start() {
        GetComponent<Rigidbody>().AddForce(transform.up * throwForce);

        Invoke("Explode", countdown);
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ignore Raycast") ||
            other.gameObject.layer == LayerMask.NameToLayer("Shield")) {
            return;
        }

        Explode();
    }

    void Explode() {
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
            hit.Hit(damage);

            // send explosion force
            //hit.Body.AddExplosionForce(damage * 500, transform.position, radius);
        }

        Destroy(gameObject);
    }
}
