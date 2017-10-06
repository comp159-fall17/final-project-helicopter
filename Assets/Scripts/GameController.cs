using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine(SpawnPickups()); //here now for testing, will move out of Start later
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private IEnumerator SpawnPickups() {
        int pickup = Random.Range(0, 3);

        if (pickup == 0) {
            SpawnHealth();
        }else if (pickup == 1) {
            SpawnAmmo();
        }else {
            SpawnShield();
        }

        yield return new WaitForSeconds(10.0f); //temporary value
    }

    private void SpawnHealth() {

    }

    private void SpawnAmmo() {

    }

    private void SpawnShield() {

    }

}
