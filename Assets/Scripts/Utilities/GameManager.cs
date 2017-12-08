using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-188)]
[RequireComponent(typeof(AudioSource))]
public class GameManager : MonoBehaviour {
    public static GameManager Instance;

    [Header("Pickups")]
    public float healAmount = 10f;
    public int ammoRecovery = 5;
    public float shieldActiveTime = 5.0f;
    public int moneyGain = 5;
    public float pickupSpawnInterval = 15.0f;

    [Space(10)]
    [Header("Canvases")]
    public GameObject GameOverCanvas;
    public GameObject screenCanvas;

    [Space(10)]
    [Header("Game Over")]
    public int gameoverTime;

    [Space(10)]
    [Header("Wave Spawning")]
    public Text gameOverPointsText;

    public Text healthText;
    public Text ammoText;
    public Text moneyText;
    public Text floorText;

    [Space(10)]
    [Header("Stats")]
    public int money; // per run, in-game shop money
    public int points; // outside of game shop currency

    public int pointsIncrease; //points gained on clearing an enemy room
    public int moneyOnEnemyKill; //money gained when killing an enemy

    public float playerLuck;

    [Space(10)]
    public AudioClip RoomClearSound;

    public AudioSource playerSound { get; private set; }
    private int startPoints;

    [HideInInspector] public bool enemyRoom = false;

    public GameObject Player { get; private set; }

    void Start() {
        if (Instance == null) {
            Instance = this;
        } else if (Instance != this) {
            Destroy(gameObject);
        }

        Player = GameObject.FindGameObjectWithTag("Player");
        playerSound = GetComponent<AudioSource>();
    }

    private void Update() {
        if (GameObject.FindWithTag("Enemy") == null) {
            if (enemyRoom) {
                ClearRoom();
            } else {
                DoorScript.CanCross = true;
            }
        }
    }

    /// <summary>
    /// Adds points on clearing a room
    /// </summary>
    void ClearRoom() {
        playerSound.PlayOneShot(RoomClearSound);
        enemyRoom = false;
        points += pointsIncrease;
        DoorScript.CanCross = true;
    }

    public void PlayDeathSound() {
        playerSound.Play();
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
        UpdateFloorText();
    }

    public IEnumerator GameOver(PlayerControls player) {
        enemyRoom = false;

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
        LevelGen.Instance.ReloadFloor(false);
    }

    public void RestartGame() {
        StopAllCoroutines();

        RemoveTagged("Enemy");
        RemoveTagged("Pickup");

        startPoints = points;

        ShopManager.Instance.Restart();
    }

    //remove remaining objects
    public void RemoveTagged(string tag) {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag(tag)) {
            Destroy(obj);
        }
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

    public void UpdateFloorText() {
        floorText.text = "Floor: " + (LevelGen.Instance.CurrentFloor + 1);
    }
    
    public void CollectMoney(int amount) {
        money += amount;
        //InGameShop.Instance.UpdateShopMoney();
        UpdateMoneyText();
    }

    public void EnemyHasDied(Transform enemyPos) {
        money += moneyOnEnemyKill;
        UpdateMoneyText();
        Spawner.Instance.SpawnPickupAtLocation(enemyPos);
    }
}
