using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    //Pickups
    public GameObject healthPickup;
    public float healAmount = 10f;

    public GameObject ammoPickup;
    public float ammoRecovery = 10f;

    public GameObject shieldPickup;
    public float shieldActiveTime = 5.0f;

    public float pickupSpawnInterval = 15.0f;
    public float pickupDestroyTime = 5.0f;

    public int playerBulletDamage = 1;

    //Canvases
    public GameObject screenCanvas;
    public GameObject shopCanvas;

    //Wave Spawning
    public Rigidbody[] enemyPrefabs;
    public Text waveTimerText;
    public Text waveNumberText;
    public Text pointsText;

    public float enemySpawnDelay = 5f;
    public float minSpawnDistance = 15f;
    public float waveDelay = 5;
    public int wave;
    public int newEmemiesPerWave = 1;

    public static GameManager Instance;

    int enemyCount;
    int enemySpawnedCount;
    int enemiesKilled;

    public int points = 0;
    public int highestWave;
    bool closeShop;

    public GameObject Player {
        get { return GameObject.FindGameObjectWithTag("Player"); }
    }

    void Start() {
        if (Instance == null) {
            Instance = this;
        } else if (Instance != this) {
            Destroy(gameObject);
        }

        // TODO: set correct wave timer value
        SetWaveTexts();

        DisplayShop();
    }

    void Update() {
        if (closeShop) {
            ManageWaves();
        }
    }

    public void DisplayShop() {
        closeShop = false;
        shopCanvas.SetActive(true);
        screenCanvas.SetActive(false);

        ShopManager.Instance.UpdateShopPoints(points);
    }

    public void CloseShop() {
        closeShop = true;
        screenCanvas.SetActive(true);
        shopCanvas.SetActive(false);

        SetWaveTexts();
        StartCoroutine(SpawnPickups());

        enemiesKilled = 0;
        enemyCount = 0;
        enemySpawning = false;
    }

    public void RestartGame() {
        StopAllCoroutines();

        foreach (GameObject obj in FindObjectsOfType<GameObject>() as GameObject[]) {
            if (obj.tag == "Enemy" || obj.tag.Contains("Pickup")) {
                Destroy(obj); //remove any lingering objects when restarting the game
            }
        }

        highestWave = wave;
        wave = 0;
        DisplayShop();
    }

    public void CloseGame() { //will not close game in editor
        Application.Quit();
    }

    bool doPickupSpawning;

    IEnumerator SpawnPickups() {
        while (true) {
            if (doPickupSpawning) {
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

            yield return new WaitForSeconds(0.0f);
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

        // set wave number and points text
        waveNumberText.text = wave.ToString();
        pointsText.text = "Points: " + points;
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

        doPickupSpawning = true;

        System.Func<Vector3, bool> overlaps = delegate (Vector3 position) {
            Collider[] hits = Physics.OverlapBox(position,
                                                 new Vector3(.75f, 0f, .75f));
            return hits.Where(i => i.gameObject.name != "Ground")
                       .ToArray().Length > 0;
        };

        while (enemySpawnedCount < EnemiesOnWave(wave)) {
            Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)],
                        GeneratePosition(overlaps), transform.rotation);
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
        enemiesKilled++;

        if (enemiesKilled == EnemiesOnWave(wave)) {
            points += 10;
            enemiesKilled = 0;
        }
    }
}
