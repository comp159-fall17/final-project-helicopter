using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {
    public GameObject bullet;

    public float fireDelay;

    Transform target;

    void Start() {
        StartCoroutine(ShootBullets());
    }

    void Update() {

    }

    IEnumerator ShootBullets() { //fire bullets in the direction of the target
        while (true) {
            GetTarget();
            ToSearchAngle();

            Instantiate(bullet, transform.position, ToSearchAngle());

            yield return new WaitForSeconds(fireDelay);
        }
    }

    void GetTarget() {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    Quaternion ToSearchAngle() {
        Vector3 targetDir = target.position - transform.position;
        float angle = Vector3.Angle(transform.forward, targetDir);

        float rotation = 90;
        rotation += transform.position.x > target.position.x ? -angle : angle;

        return Quaternion.Euler(0, rotation, 90);
    }
}
