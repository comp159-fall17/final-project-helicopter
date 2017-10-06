using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Shooter : MonoBehaviour {
    public GameObject bullet;

    public float fireDelay;
    public float walkSpeed = 10f;
    //public float runSpeed = 30f;

    protected Rigidbody body;

    /// <summary>
    /// Whether this <see cref="T:Shooter"/> should shoot.
    /// </summary>
    /// <value><c>true</c> if should shoot; otherwise, <c>false</c>.</value>
    protected abstract bool ShouldShoot { get; }

    /// <summary>
    /// Target to aim at.
    /// </summary>
    /// <value>The target.</value>
    protected abstract Vector3 Target { get; }

    /// <summary>
    /// Effective moving speed.
    /// </summary>
    /// <value>The speed.</value>
    protected virtual float Speed { get { return walkSpeed; } }

    protected virtual void Start() {
        body = GetComponent<Rigidbody>();
    }

    protected virtual void Update() {
        if (ShouldShoot && !shooting) {
            StartCoroutine(ShootBullets());
        }
    }

    /// <summary>
    /// True if ShootBullets() is currently running.
    /// </summary>
    bool shooting;

    /// <summary>
    /// Shoots bullet at the target until ShouldShoot becomes false.
    /// </summary>
    protected IEnumerator ShootBullets() {
        shooting = true;
        while (ShouldShoot) {
            float angle = FaceTargetAngle(Target);

            Instantiate(bullet, BulletSpawnPoint(angle),
                        Quaternion.Euler(0, angle, 90));

            yield return new WaitForSeconds(fireDelay);
        }
        shooting = false;
    }

    /// <summary>
    /// Find angle towards target.
    /// </summary>
    /// <returns>Angle towards target from 0 degrees.</returns>
    /// <param name="target">Target point.</param>
    float FaceTargetAngle(Vector3 target) {
        // TODO: fix -angle
        float angle = Mathf.Atan2(transform.position.z - target.z,
                                  transform.position.x - target.x);

        return -angle * Mathf.Rad2Deg;
    }

    /// <summary>
    /// Find bullet spawn point.
    /// </summary>
    /// <returns>Spawn point.</returns>
    /// <param name="angle">Angle of shooting direction.</param>
    Vector3 BulletSpawnPoint(float angle) {
        return body.position - (Quaternion.AngleAxis(angle, Vector3.up)
                                * Vector3.right).normalized;
    }
}
