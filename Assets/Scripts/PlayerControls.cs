using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour {
    public GameObject bullet;

    public float fireDelay;
    public float walkSpeed = 10f;
    //public float runSpeed = 30f;

    Vector3 inputAxes;

    Rigidbody body;
    Camera follow;

    void Start() {
        body = GetComponent<Rigidbody>();
        follow = Camera.main;
        inputAxes = new Vector3(0, 0, 0);
    }

    void Update() {
        updateInputAxes();
        TrackCamera();

        if (Input.GetMouseButtonDown(0)) {
            StartCoroutine(ShootBullets());
        }
    }

    void FixedUpdate() {
        body.velocity = CopyY(inputAxes, body.velocity);
    }

    void updateInputAxes() {
        inputAxes.x = Input.GetAxis("Horizontal") * walkSpeed;
        inputAxes.z = Input.GetAxis("Vertical") * walkSpeed;
    }

    void TrackCamera() {
        follow.transform.position = CopyY(transform.position,
                                         follow.transform.position);
    }

    /// <summary>
    /// Creates a copy of v with w.y substituted.
    /// </summary>
    /// <returns>"Copied" vector.</returns>
    /// <param name="to">Destination.</param>
    /// <param name="from">Source.</param>
    Vector3 CopyY(Vector3 to, Vector3 from) {
        to.y = from.y;
        return to;
    }

    IEnumerator ShootBullets() {
        while (true) {
            float angle = ToMouseAngle();

            Instantiate(bullet, BulletSpawnPoint(angle),
                        Quaternion.Euler(0, angle, 90));

            yield return new WaitForSeconds(fireDelay);

            if (!Input.GetMouseButton(0)) {
                yield break;
            }
        }
    }

    float ToMouseAngle() {
        // Raycast to corresponding point on screen.
        Ray viewRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Physics.Raycast(viewRay, out hit);

        Vector3 target = hit.point;

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
        return body.position - (Quaternion.AngleAxis(angle, Vector3.up) 
                                * Vector3.right).normalized;
    }
}
