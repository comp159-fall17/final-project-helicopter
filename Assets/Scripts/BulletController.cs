using UnityEngine;

/// <summary>
/// Bullet controller.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class BulletController : MonoBehaviour {
    public float bulletSpeed;
    public float distance;

    public float Speed { get { return Body.velocity.magnitude; } }

    Rigidbody Body { get { return GetComponent<Rigidbody>(); } }

    Vector3 origin;

    protected virtual void Start() {
        origin = transform.position;

        // shoots in the bullet's positive x direction
        Body.AddForce(transform.up * bulletSpeed);

        LookAhead();
    }

    protected virtual void FixedUpdate() {
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

    protected virtual void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ignore Raycast") ||
            other.gameObject.layer == LayerMask.NameToLayer("Shield")) {
            return;
        }
        
        switch (other.gameObject.tag) {
        case "Enemy":
            //TriggerEnemy(other);
            //break;
        case "Player":
            TriggerPlayer(other);
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
    protected virtual void TriggerPlayer(Collider player) {
        player.gameObject.GetComponent<Shooter>().Hit(this);
        Destroy(gameObject);
    }

    ///// <summary>
    ///// Enemy trigger handler.
    ///// </summary>
    ///// <param name="enemy">Enemy collider.</param>
    //protected virtual void TriggerEnemy(Collider enemy) {
    //    enemy.gameObject.GetComponent<Shooter>().Hit(this);
    //    Destroy(gameObject);
    //}
}
