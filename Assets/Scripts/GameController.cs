using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    //Pickups
    public GameObject healthPickup;
    public GameObject ammoPickup;
    public GameObject shieldPickup;
    public float pickupSpawnDelay = 15.0f; //how long to wait before spawning another pickup
    public float pickupDestroyTime = 5.0f; //how long before the pickup disappears

    //Wave Spawning
    public Rigidbody enemyPrefab;
    public float enemySpawnDelay = 5f;
    public float spawnDistanceFromPlayer = 15f;
    public int enemiesPerWave = 5;
    public int waveDelay = 5;
    public int enemyCount;
    public int newEmemiesPerWave = 1;

    int enemySpawnedCount;
    bool startWave = true;
    bool stopWave;
    bool startNewWave;

    // Use this for initialization
    void Start () {
        StartCoroutine(SpawnPickups()); //here now for testing, will move out of Start later
    }
	
	// Update is called once per frame
	void Update () {
        NewWave(); //Checks if new wave needs to be started
	}

    IEnumerator SpawnPickups() {
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

    void SpawnPickup(int type, Vector3 location) {
        GameObject pickup;
        switch (type) {
        case 0:
            pickup = healthPickup;
            break;
        case 1:
            pickup = ammoPickup;
            break;
        default:
            pickup = shieldPickup;
            break;
        }

        Destroy(Instantiate(pickup, location, Quaternion.identity),
                pickupDestroyTime);
    }

    //Waves
    void NewWave() { //Checks if wave needs to be started
        if (startWave) { //Starts wave if needed
            StartCoroutine(EnemySpawn());
            startWave = false;
        }

        if (stopWave) { //Ends wave when max enemies are spawned
            StopCoroutine(EnemySpawn());
            stopWave = false;
            startNewWave = true;
        }

        if (enemyCount == 0 && startNewWave) { //When all enemies are dead and its time to start a new wave
            Invoke("nextWave", waveDelay); //Starts new wave after a delay
            startNewWave = false;
        }
    }

    private void NextWave() { //Called by Invoke so it has a delay starting the next wave
        startWave = true;
        enemiesPerWave++; //One more max enemy every wave
    }

    private IEnumerator EnemySpawn() {
        GameObject Player = GameObject.Find("Player"); //Gets the player object so enemies dont spawn on the player
        PlayerControls p = Player.GetComponent<PlayerControls>(); //Same as above
        enemyCount = 0; //Resets enemy count each wave
        enemySpawnedCount = 0;
        while (true) {
            Vector3 position = new Vector3(Random.Range(-38.0f, 38.0f), 0.5f, Random.Range(-38.0f, 38.0f)); //Gets random position for enemy spawn
            Collider[] emptySpot = Physics.OverlapBox(position, new Vector3(.75f, 0f, .75f)); //Checks for nearby colliders so enemies dont spawn on things
            if ((Vector3.Distance(position, p.transform.position) > spawnDistanceFromPlayer) && (emptySpot.Length == 0)) { //Stops enemies from spawning near the player
                Rigidbody enemy = Instantiate(enemyPrefab, position, transform.rotation) as Rigidbody; //Spawns new enemy
                enemyCount++; //Counts enemies alive (gets subtracted when player kills enemy)
                enemySpawnedCount++; //Counts enemies spawned
            }
            if (enemySpawnedCount == enemiesPerWave) { //Ends wave when max enemies spawned
                stopWave = true;
                break;
            }
            yield return new WaitForSeconds(enemySpawnDelay); //Delay between enemy spawns
        }
    }

}