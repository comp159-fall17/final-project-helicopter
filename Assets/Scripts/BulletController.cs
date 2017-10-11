using UnityEngine;

/// <summary>
/// Bullet controller.
/// </summary>
public class BulletController : MonoBehaviour {
    public float bulletSpeed;
    public float distance;

    public float Speed { get { return Body.velocity.magnitude; } }

    Rigidbody Body { get { return GetComponent<Rigidbody>(); } }

    Vector3 origin;

    void Start() {
        origin = transform.position;

        // shoots in the bullet's positive x direction
        Body.AddForce(transform.up * bulletSpeed);

        LookAhead();
    }

    void FixedUpdate() {
        if (Vector3.Distance(origin, transform.position) > distance) {
            Destroy(gameObject);
        }

        LookAhead();
    }

    /// <summary>
    /// Look ahead a frame.
    /// </summary>
    void LookAhead() {
        RaycastHit hit;
        if (Physics.Raycast(Body.position, Body.velocity, out hit,
                            Body.velocity.magnitude * Time.deltaTime)) {
            OnTriggerEnter(hit.collider);
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ignore Raycast") ||
            other.gameObject.layer == LayerMask.NameToLayer("Shield")) {
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

    /// <summary>
    /// Player trigger handler.
    /// </summary>
    /// <param name="player">Player collider.</param>
    void TriggerPlayer(Collider player) {
        player.gameObject.GetComponent<Shooter>().Health.Hit(this);
        Destroy(gameObject);
    }

    /// <summary>
    /// Enemy trigger handler.
    /// </summary>
    /// <param name="enemy">Enemy collider.</param>
    void TriggerEnemy(Collider enemy) {
        enemy.gameObject.GetComponent<Shooter>().Health.Hit(this);
        Destroy(gameObject);
    }
}
