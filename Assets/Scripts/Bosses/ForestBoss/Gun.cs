using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {

    public GameObject bulletPrefab;
    public float bulletCoolDown;

	// Use this for initialization
	void Start () {
        StartCoroutine("spawnBullet");
    }

    IEnumerator spawnBullet()
    {
        while (true)
        {
            GameObject bullet = Instantiate(bulletPrefab, this.transform.position, this.transform.rotation);
            bullet.GetComponent<Rigidbody>().AddForce(transform.up * 1000);
            yield return new WaitForSeconds(bulletCoolDown);
        }
    }
}
