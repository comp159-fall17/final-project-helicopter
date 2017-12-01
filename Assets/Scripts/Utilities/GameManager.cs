using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    //Pickups
    public float healAmount = 10f;
    public int ammoRecovery = 5;
    public float shieldActiveTime = 5.0f;
    public float pickupSpawnInterval = 15.0f;

    //Canvases
    public GameObject guiCanvas;
    public GameObject shopCanvas;
    public GameObject GameOverCanvas;

    //GameOver
    public int gameoverTime;
    public GameObject playerPrefab;

    //Wave Spawning
    //public Text waveTimerText;
    //public Text waveNumberText;  
    public Text moneyText;
    public Text gameOverPointsText;

    public float enemySpawnDelay = 5f;
    public float minSpawnDistance = 15f;
    public float waveDelay = 5;
    public int wave;
    public int newEmemiesPerWave = 1;

    public Text healthText;
    public Text ammoText;

    public static GameManager Instance;

    int enemyCount;
    int enemySpawnedCount;
    int enemiesKilled;

    public int money; //per run, in-game shop money
    public int points; //outside of game shop currency
    int startPoints;
    public int highestWave;
    bool closeShop;

	private AudioSource deathSound;

    public GameObject Player;

    void Start() {
        if (Instance == null) {
            Instance = this;
        } else if (Instance != this) {
            Destroy(gameObject);
        }
		
        Player = GameObject.FindGameObjectWithTag("Player");
        SetWaveTexts();
        ShopActive = true;
        deathSound = GetComponent<AudioSource>();
    }

	public void playDeathSound(bool canPlay){
		if (canPlay) {
			AudioSource.PlayClipAtPoint (deathSound.clip, Camera.main.transform.position);
		}
	}

    public void StartGame() {
        ShopActive = false;

        // reset stats
        startPoints = points;
        enemiesKilled = 0;
        enemyCount = 0;
        money = 0;
        enemySpawning = false;
        Player.GetComponent<PlayerControls>().Health.Reset();
        Player.GetComponent<PlayerControls>().SetDamage();

        UpdateHealthText();
        UpdateAmmoText();
		playDeathSound (false);

        StartCoroutine(ManageWaves());
    }

    public IEnumerator gameOver(PlayerControls player) {
        // wait for showing to finish
        gameOverPointsText.enabled = false;
        GameOverCanvas.SetActive(true);
        yield return new WaitForSeconds(1f);

        gameOverPointsText.text = "" + (points - startPoints); //display how many points were earned this run
        gameOverPointsText.enabled = true;
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
        startPoints = points;
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
                StartCoroutine(SpawnPickups());

                lastWaveBegin = Time.time;
            }
            SetWaveTexts();

            yield return null;
        }
    }

    public bool ShopActive {
        get { return shopCanvas.activeSelf; }
        set {
            ShopManager.Instance.UpdateShopPoints();

            shopCanvas.SetActive(value);
            guiCanvas.SetActive(!value);
        }
    }

    IEnumerator SpawnPickups() {
        while (!ShopActive) {
            Spawner.Instance.SpawnPickup();

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

    void SetWaveTexts() {
        // set timer
        float tilSpawn = waveDelay - (Time.time - lastWaveBegin) + 0.5f;
        tilSpawn = Mathf.Round(tilSpawn);

        /*if (tilSpawn <= 0) {
            waveTimerText.text = "Go!";
        } else {
            waveTimerText.text = "New wave in "
                + Mathf.Round(tilSpawn).ToString() + "...";
        }*/

        // set wave number and points text
        //waveNumberText.text = wave.ToString();
        moneyText.text = "money: " + money;
    }

    public void UpdateHealthText() {
        healthText.text = "Health: " + Player.GetComponent<PlayerControls>().Health.Points;
    }

    public void UpdateAmmoText() {
        ammoText.text = "Special Ammo: " + Player.GetComponent<PlayerControls>().specialAmmo;
    }

    // if EnemySpawn() is currently running
    bool enemySpawning;

    /// <summary>
    /// Spawn a few enemies.
    /// </summary>
    /// <returns>Coroutine.</returns>
    IEnumerator EnemySpawn() {
        enemySpawning = true;

        // wait between waves
        yield return new WaitForSeconds(waveDelay);

        enemyCount = 0;
        enemySpawnedCount = 0;
        wave++;

        // create enemies
        while (enemySpawnedCount < EnemiesOnWave(wave)) {
            Spawner.Instance.SpawnEnemy();

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

        //testing
        money += 5;
        InGameShop.Instance.UpdateShopMoney();

        if (enemiesKilled == EnemiesOnWave(wave)) {
            points += pointsPerWave;
            enemiesKilled = 0;
        }
    }
}
