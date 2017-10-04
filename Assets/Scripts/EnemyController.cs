using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {
    public GameObject bullet;
    public float fireDelay;

    public Transform target; //this could be substituted for the player's position

	// Use this for initialization
	void Start() {
        StartCoroutine(ShootBullets());
	}
	
	// Update is called once per frame
	void Update() {
		
	}

    private IEnumerator ShootBullets(){ //fire bullets in the direction of the target
        while (true){
            Vector3 targetDir = target.position - transform.position;
            float angle = Vector3.Angle(transform.forward, targetDir);

            float rotation;
            if (transform.position.x > target.transform.position.x){
                rotation = 90 - angle;
            }else{
                rotation = 90 + angle;
            }

            Instantiate(bullet, this.gameObject.transform.position, Quaternion.Euler(0, rotation, 90));

            yield return new WaitForSeconds(fireDelay);
        }
    }
}
