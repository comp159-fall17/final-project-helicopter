using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Shooter : MonoBehaviour {
    public GameObject bullet;

    public float fireDelay;
    public float walkSpeed = 10f;
    //public float runSpeed = 30f;

    protected Rigidbody body;

    protected abstract bool StartShooting { get; }
    protected abstract bool StopShooting { get; }

    protected abstract Vector3 GetTarget();

    protected virtual void Start() {
        body = GetComponent<Rigidbody>();
    }

    protected virtual void Update() {
        if (StartShooting && !shooting) {
            StartCoroutine(ShootBullets());
        }
    }

    protected virtual float CurrentSpeed() {
        return walkSpeed;
    }

    bool shooting;

    protected IEnumerator ShootBullets() {
        shooting = true;
        while (true) {
            float angle = FacePointAngle(GetTarget());

            Instantiate(bullet, BulletSpawnPoint(angle),
                        Quaternion.Euler(0, angle, 90));

            yield return new WaitForSeconds(fireDelay);

            if (StopShooting) {
                shooting = false;
                yield break;
            }
        }
    }

    float FacePointAngle(Vector3 target) {
        // TODO: fix -angle
        float angle = Mathf.Atan2(transform.position.z - target.z,
                                  transform.position.x - target.x);

        return -angle * Mathf.Rad2Deg;
    }

    /// <summary>
    /// Find bullet spawn point.
    /// </summary>
    /// <returns>Spawn point.</returns>
    /// <param name="angle">angle of shooting.</param>
    Vector3 BulletSpawnPoint(float angle) {
        Debug.Log(angle);
        return body.position - (Quaternion.AngleAxis(angle, Vector3.up)
                                * Vector3.right).normalized;
    }
}
