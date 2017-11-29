using System.Linq;
using UnityEngine;

public class Shotgun : SpecialWeapon {
    public GameObject bullet;

    public int numBullets;
    public float fireArc = 50f;

    void Start() {
        Shoot();
    }

    void Shoot() {
        //shoot bullets in shotgun spread
        float step = fireArc / (numBullets - 1);
        float total = -(fireArc / 2.0f);

        for (int i = 0; i < numBullets; i++) {
            Quaternion bulletRot = Quaternion.Euler(
                transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + total, transform.rotation.eulerAngles.z);

            Instantiate(bullet, transform.position, bulletRot);

            total += step;
        }
    }
}
