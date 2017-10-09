using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    //Pickups
    public GameObject healthPickup;
    public GameObject ammoPickup;
    public GameObject shieldPickup;

    public float pickupSpawnInterval = 15.0f;
    public float pickupDestroyTime = 5.0f;

    //Wave Spawning
    public Rigidbody enemyPrefab;
    public Text waveTimerText;
    public Text waveNumberText;

    public float enemySpawnDelay = 5f;
    public float minSpawnDistance = 15f;
    public float waveDelay = 5;
    public int wave;
    public int newEmemiesPerWave = 1;

    public static GameManager Instance;

    int enemyCount;
    int enemySpawnedCount;

    void Start() {
        if (Instance == null) {
            Instance = this;
        } else if (Instance != this) {
            Destroy(gameObject);
        }

        // TODO: set correct wave timer value
        SetWaveTexts();

        // TODO: testing, move out of Start
        StartCoroutine(SpawnPickups());
    }

    void Update() {
        ManageWaves();
    }

    IEnumerator SpawnPickups() {
        while (true) {
            int pickup = Random.Range(0, 3);

            System.Func<Vector3, bool> overlaps = delegate (Vector3 position) {
                float prefabRadius = healthPickup.GetComponent<SphereCollider>()
                                                 .radius;

                Collider[] hits = Physics.OverlapSphere(position, prefabRadius);
                return hits.Where(i => i.gameObject.name != "Ground")
                           .ToArray().Length > 0;
            };

            SpawnPickup(pickup, GeneratePosition(overlaps));

            yield return new WaitForSeconds(pickupSpawnInterval);
        }
    }

    /// <summary>
    /// Generates valid position.
    /// </summary>
    /// <returns>The position.</returns>
    /// <param name="overlapFunc">Overlap validation function.</param>
    Vector3 GeneratePosition(System.Func<Vector3, bool> overlapFunc) {
        Vector3 candidate;

        Vector3 playerPostion = GameObject.FindGameObjectWithTag("Player")
                                          .transform.position;

        do {
            candidate = new Vector3(Random.Range(-38.0f, 38.0f), 0.5f,
                                    Random.Range(-38.0f, 38.0f));
            // if far from player and nothing collides with it
        } while (Vector3.Distance(candidate, playerPostion) < minSpawnDistance
                 || overlapFunc(candidate));

        return candidate;
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

    float lastWaveBegin;

    /// <summary>
    /// Checks if wave needs to be started.
    /// </summary>
    void ManageWaves() {
        if (!enemySpawning && enemyCount == 0) {
            StartCoroutine(EnemySpawn());
            lastWaveBegin = Time.time;
        }
        SetWaveTexts();
    }

    void SetWaveTexts() {
        // set timer
        float tilSpawn = waveDelay - (Time.time - lastWaveBegin) + 0.5f;
        tilSpawn = Mathf.Round(tilSpawn);

        if (tilSpawn <= 0) {
            waveTimerText.text = "Go!";
        } else {
            waveTimerText.text = "New wave in "
                + Mathf.Round(tilSpawn).ToString() + "...";
        }

        // set wave number
        waveNumberText.text = wave.ToString();
    }

    // if EnemySpawn() is currently running
    bool enemySpawning;

    /// <summary>
    /// Spawn a few enemies.
    /// </summary>
    /// <returns>The spawn.</returns>
    IEnumerator EnemySpawn() {
        enemySpawning = true;

        // wait between waves
        yield return new WaitForSeconds(waveDelay);

        enemyCount = 0;
        enemySpawnedCount = 0;
        wave++;

        System.Func<Vector3, bool> overlaps = delegate (Vector3 position) {
            Collider[] hits = Physics.OverlapBox(position,
                                                 new Vector3(.75f, 0f, .75f));
            return hits.Where(i => i.gameObject.name != "Ground")
                       .ToArray().Length > 0;
        };


        while (enemySpawnedCount < EnemiesOnWave(wave)) {
            Instantiate(enemyPrefab, GeneratePosition(overlaps), transform.rotation);
            enemyCount++;
            enemySpawnedCount++;

            yield return new WaitForSeconds(enemySpawnDelay);
        }

        enemySpawning = false;
    }

    static int EnemiesOnWave(int wave) {
        return wave + 4;
    }

    public void EnemyHasDied() {
        enemyCount--;
    }
}
