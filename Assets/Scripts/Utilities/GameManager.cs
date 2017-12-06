using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-188)]
[RequireComponent(typeof(AudioSource))]
public class GameManager : MonoBehaviour {
    public static GameManager Instance;

    //Pickups
    public float healAmount = 10f;
    public int ammoRecovery = 5;
    public float shieldActiveTime = 5.0f;
    public int moneyGain = 5;
    public float pickupSpawnInterval = 15.0f;

    //Canvases
    public GameObject GameOverCanvas;
    public GameObject screenCanvas;

    //GameOver
    public int gameoverTime;

    //Wave Spawning
    public Text gameOverPointsText;

    public Text healthText;
    public Text ammoText;
    public Text moneyText;

    public int money; // per run, in-game shop money
    public int points; // outside of game shop currency

    public float playerLuck;

    private AudioSource deathSound;
    private int startPoints;

    public GameObject Player { get; private set; }

    void Start() {
        if (Instance == null) {
            Instance = this;
        } else if (Instance != this) {
            Destroy(gameObject);
        }

        Player = GameObject.FindGameObjectWithTag("Player");
        deathSound = GetComponent<AudioSource>();
    }

    private void Update() {
        if (GameObject.FindWithTag("Enemy") == null) {
            DoorScript.CanCross = true;
        }
    }

    public void PlayDeathSound() {
        AudioSource.PlayClipAtPoint(deathSound.clip, Camera.main.transform.position);
    }

    public void StartGame() {
        // reset stats
        startPoints = points;
        money = 0;
        playerLuck = 0f;
        Player.GetComponent<PlayerControls>().Health.Reset();
        Player.GetComponent<PlayerControls>().SetDamage();

        UpdateHealthText();
        UpdateAmmoText();
        UpdateMoneyText();
    }

    public IEnumerator GameOver(PlayerControls player) {
        // wait for showing to finish
        gameOverPointsText.enabled = false;
        GameOverCanvas.SetActive(true);
        screenCanvas.SetActive(false);
        yield return new WaitForSeconds(1f);

        // display how many points were earned this run
        gameOverPointsText.text = "" + (points - startPoints); 
        gameOverPointsText.enabled = true;
        yield return new WaitForSeconds(gameoverTime);
        GameOverCanvas.SetActive(false);
        screenCanvas.SetActive(true);

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

        startPoints = points;

        ShopManager.Instance.Restart();
    }

    /// <summary>
    /// Closes the game. Will not close the game in editor.
    /// </summary>
    public void CloseGame() {
        Application.Quit();
    }

    IEnumerator SpawnPickups() {
        while (true) {
            Spawner.Instance.SpawnPickup();
            yield return new WaitForSeconds(pickupSpawnInterval);
        }
    }

    public void UpdateHealthText() {
        healthText.text = "Health: " + Player.GetComponent<PlayerControls>().Health.Points;
    }

    public void UpdateAmmoText() {
        ammoText.text = "Special Ammo: " + Player.GetComponent<PlayerControls>()
            .specialAmmo[Player.GetComponent<PlayerControls>().specialType];
    }

    public void UpdateMoneyText() {
        moneyText.text = "Money: " + money;
    }

    public void CollectMoney(int amount) {
        money += amount;
        InGameShop.Instance.UpdateShopMoney();
        UpdateMoneyText();
    }

    public void EnemyHasDied(Transform enemyPos) {
        Spawner.Instance.SpawnPickupAtLocation(enemyPos);
    }
}
