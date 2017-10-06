using UnityEngine;

/// <summary>
/// Bullet controller.
/// </summary>
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

        LookAhead();
    }

    void FixedUpdate() {
        if (Vector3.Distance(origin, transform.position) > distance) {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Look ahead two frames.
    /// </summary>
    void LookAhead() {
        RaycastHit hit;
        if (Physics.Raycast(body.position, body.velocity, out hit,
                            body.velocity.magnitude * Time.deltaTime * 0)) {
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

    /// <summary>
    /// Player trigger handler.
    /// </summary>
    /// <param name="player">Player collider.</param>
    void TriggerPlayer(Collider player) {
        Destroy(gameObject);
    }

    /// <summary>
    /// Enemy trigger handler.
    /// </summary>
    /// <param name="enemy">Enemy collider.</param>
    void TriggerEnemy(Collider enemy) {
        Destroy(gameObject);
    }
}
