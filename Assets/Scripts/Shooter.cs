using System.Collections;
using UnityEngine;

/// <summary>
/// Controller for enemy that can shoot.
/// </summary>
public abstract class Shooter : MonoBehaviour {
    public GameObject bullet;

    public float fireDelay;
    public float walkSpeed = 10f;
    //public float runSpeed = 30f;

    protected Rigidbody Body { get { return GetComponent<Rigidbody>(); } }

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
    /// Direction of Target relative to position.
    /// </summary>
    /// <value>The target direction.</value>
    protected Vector3 TargetDirection {
        get { return Target - transform.position; }
    }

    /// <summary>
    /// Distance to Target.
    /// </summary>
    /// <value>The target distance.</value>
    protected float TargetDistance { get { return TargetDirection.magnitude; } }

    /// <summary>
    /// Angle relative to local forward.
    /// </summary>
    /// <value>The relative target angle.</value>
    protected float RelativeTargetAngle {
        get { return Vector3.Angle(TargetDirection, transform.forward); }
    }

    /// <summary>
    /// Angle relative to world forward.
    /// </summary>
    /// <value>The absolute target angle.</value>
    protected float AbsoluteTargetAngle {
        get {
            float angle = Mathf.Atan2(transform.position.z - Target.z,
                                      transform.position.x - Target.x);

            return -angle * Mathf.Rad2Deg;
        }
    }

    /// <summary>
    /// Effective moving speed.
    /// </summary>
    /// <value>The speed.</value>
    protected virtual float Speed { get { return walkSpeed; } }

    /// <summary>
    /// Find bullet spawn point.
    /// </summary>
    /// <returns>Spawn point.</returns>
    /// <param name="angle">Angle of shooting direction.</param>
    protected Vector3 BulletSpawnPoint {
        get {
            Quaternion outside = Quaternion.AngleAxis(AbsoluteTargetAngle,
                                                      Vector3.up);
            return Body.position - (outside * Vector3.right).normalized;
        }
    }

    protected virtual void Start() {
        // nothing.
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
            Instantiate(bullet, BulletSpawnPoint,
                        Quaternion.Euler(0, AbsoluteTargetAngle, 90));

            yield return new WaitForSeconds(fireDelay);
        }
        shooting = false;
    }
}
