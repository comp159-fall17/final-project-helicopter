using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    //Pickups
    public GameObject healthPickup;
    public float healAmount = 10f;

    public GameObject ammoPickup;
    public int ammoRecovery = 5;

    public GameObject shieldPickup;
    public float shieldActiveTime = 5.0f;

    public float pickupSpawnInterval = 15.0f;
    public float pickupDestroyTime = 5.0f;

    public int playerBulletDamage = 1;

    //Canvases
    public GameObject guiCanvas;
    public GameObject shopCanvas;
    public GameObject GameOverCanvas;

    //GameOver
    public int gameoverTime;
    public GameObject playerPrefab;

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

    public Text ammoText;

    public static GameManager Instance;

    int enemyCount;
    int enemySpawnedCount;
    int enemiesKilled;

    public int points;
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

        ShopActive = true;
    }

    public void StartGame() {
        ShopActive = false;

        // reset stats
        enemiesKilled = 0;
        enemyCount = 0;
        enemySpawning = false;
        Player.GetComponent<PlayerControls>().ResetAmmo();
        Player.GetComponent<PlayerControls>().Health.Reset();

        UpdateAmmoText();

        StartCoroutine(ManageWaves());
    }

    public IEnumerator gameOver(PlayerControls player) {
        // wait for showing to finish
        GameOverCanvas.SetActive(true);
        yield return new WaitForSeconds(gameoverTime);
        GameOverCanvas.SetActive(false);

        player.Reset();
        RestartGame();
    }

    public void RestartGame() {
        StopAllCoroutines();

        // remove remaining objects
        System.Action<string> removeTagged = delegate (string tag) {
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag(tag)) {
                Destroy(obj);
            }
        };

        removeTagged("Enemy");
        removeTagged("Pickup");

        highestWave = Mathf.Max(highestWave, wave);
        wave = 0;
        ShopActive = true;
    }

    /// <summary>
    /// Closes the game. Will not close the game in editor.
    /// </summary>
    public void CloseGame() {
        Application.Quit();
    }

    float lastWaveBegin;

    /// <summary>
    /// Checks if wave needs to be started.
    /// </summary>
    IEnumerator ManageWaves() {
        while (!ShopActive) {
            if (!enemySpawning && enemyCount == 0) {
                StartCoroutine(EnemySpawn());
                lastWaveBegin = Time.time;
            }
            SetWaveTexts();

            yield return null;
        }
    }

    public bool ShopActive {
        get { return shopCanvas.activeSelf; }
        set {
            ShopManager.Instance.UpdateShopPoints(points);

            shopCanvas.SetActive(value);
            guiCanvas.SetActive(!value);
        }
    }

    IEnumerator SpawnPickups() {
        while (!ShopActive) {
            SpawnPickup(Random.Range(0, 3));

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

    void SpawnPickup(int type) {
        System.Func<Vector3, bool> overlaps = delegate (Vector3 position) {
            float prefabRadius = healthPickup.GetComponent<SphereCollider>()
                                             .radius;

            Collider[] hits = Physics.OverlapSphere(position, prefabRadius);
            return hits.Where(i => i.gameObject.name != "Ground")
                       .ToArray().Length > 0;
        };

        GameObject pickup;
        switch (type) {
        case 0:
            pickup = healthPickup;
            break;
        case 1:
            pickup = ammoPickup;
            break;
        //case 2:
        default:
            pickup = shieldPickup;
            break;
        }

        Destroy(Instantiate(pickup, GeneratePosition(overlaps), Quaternion.Euler(-90.0f, 180.0f, 0.0f)),
                pickupDestroyTime);
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

    public void UpdateAmmoText() {
        ammoText.text = "Special Ammo: " + Player.GetComponent<PlayerControls>().specialAmmo;
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

        StartCoroutine(SpawnPickups());

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

    public int pointsPerWave;

    public void EnemyHasDied() {
        enemyCount--;
        enemiesKilled++;

        if (enemiesKilled == EnemiesOnWave(wave)) {
            points += pointsPerWave;
            enemiesKilled = 0;
        }
    }
}
