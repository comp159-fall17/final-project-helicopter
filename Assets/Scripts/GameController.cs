using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public GameObject healthPickup;
    public GameObject ammoPickup;
    public GameObject shieldPickup;

    public float pickupSpawnDelay = 15.0f; //how long to wait before spawning another pickup
    public float pickupDestroyTime = 5.0f; //how long before the pickup disappears

	// Use this for initialization
	void Start () {
        StartCoroutine(SpawnPickups()); //here now for testing, will move out of Start later
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private IEnumerator SpawnPickups() {
        while (true) {
            int pickup = Random.Range(0, 3);
            Vector3 spawnPoint = new Vector3(0.0f, 0.51f, 0.0f);
            bool foundLocation = false;

            while (!foundLocation) {
                spawnPoint.x = Random.Range(-30.0f, 30.0f); //temporary values until game boundaries are decided
                spawnPoint.z = Random.Range(-23.0f, 8.0f); //might change it to only spawn within what the camera currently sees
                foundLocation = true;

                // check if the location to spawn the pickup at is occupied by another object
                // for the current prefabs, the sphere collider radii are equal
                if (Physics.OverlapSphere(spawnPoint, healthPickup.GetComponent<SphereCollider>().radius).Length > 0) {
                    foundLocation = false;
                }
            }

            SpawnPickup(pickup, spawnPoint);

            yield return new WaitForSeconds(pickupSpawnDelay);
        }
    }

    private void SpawnPickup(int type, Vector3 location) {
        if (type == 0) { //health
            Destroy(Instantiate(healthPickup, location, Quaternion.identity), pickupDestroyTime);
        }else if (type == 1) { //ammo
            Destroy(Instantiate(ammoPickup, location, Quaternion.identity), pickupDestroyTime);
        }else { //shield
            Destroy(Instantiate(shieldPickup, location, Quaternion.identity), pickupDestroyTime);
        }
    }

}
