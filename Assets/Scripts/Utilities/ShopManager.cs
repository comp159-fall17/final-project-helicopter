using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour {
    public Text shopPointsText1;
    public Text shopPointsText2;
    int shopPoints;

    public GameObject mainShop;
    public GameObject specialShop;
    public GameObject playerCanvas;

    public ShopUpgrade[] Upgrades;
    public Text[] levelText;
    public Text[] costText;
    public Text[] statsText;

    bool[] weaponLocked = new bool[3];
    public GameObject[] weaponButton;
    public GameObject[] weaponCover;

    public SpecialShopUpgrade[] SpecialUpgrades;
    public Text[] specialLevelText;
    public Text[] specialCostText;
    public Text[] specialStatsText;

    [Header("Ammo, radius, damage")]
    public float[] grenadeValues; //ammo, radius and damage

    [Header("Ammo, fire arc, num bullets, damage")]
    public float[] shotgunValues; //ammo, fire-arc, num bullets and damage

    [Header("Ammo, growth rate, damage")]
    public float[] ringValues; //ammo, growth rate, and damage
    [Space(10)]

    public float grenadeRadiusIncrease = 1.5f;
    public float shotgunFireArcIncrease = 10f;
    public int shotgunBulletsIncrease = 4;
    public float ringGrowthRateIncrease = 0.25f;

    PlayerControls player; //player's script

    public static ShopManager Instance;

    void Start () {
        if (Instance == null) {
            Instance = this;
        } else if (Instance != this) {
            Destroy(gameObject);
        }

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControls>();

        Restart();

        for (int i = 0; i < 3; i++) {
            weaponLocked[i] = true;
            specialCostText[i].text = "Cost: " + SpecialUpgrades[i].cost;
            SpecialUpgrades[i].currentUpgrade = 1;
            SpecialUpgrades[i].weaponLevel = 1;
        }

        //grenade launcher original values
        Grenade grenade = player.specialWeapons[0].GetComponent<Grenade>();

        grenade.maxAmmo = (int)grenadeValues[0];
        grenade.radius = grenadeValues[1];
        grenade.damage = (int)grenadeValues[2];

        //shotgun original values
        Shotgun shotgun = player.specialWeapons[1].GetComponent<Shotgun>();

        shotgun.maxAmmo = (int)shotgunValues[0];
        shotgun.fireArc = shotgunValues[1];
        shotgun.numBullets = (int)shotgunValues[2];
        shotgun.bullet.GetComponent<BulletController>().damage = shotgunValues[3];

        //ring cannon original values
        RingWeapon ringWeapon = player.specialWeapons[2].GetComponent<RingWeapon>();

        ringWeapon.maxAmmo = (int)ringValues[0];
        ringWeapon.ring.GetComponent<Ring>().growthRate = ringValues[1];
        ringWeapon.ring.GetComponent<Ring>().damage = ringValues[2];

        for (int i = 0; i < 6; i++) {
            costText[i].text = "Cost: " + Upgrades[i].cost;
            Upgrades[i].currentUpgrade = 1;
        }
    }

    void Update() {
        statsText[0].text = "Max Health Limit: " + player.Health.maxPointsLimit;
        statsText[1].text = "Player Speed: " + player.walkSpeed;
        statsText[2].text = "Player Damage: " + player.bulletDamage;
        statsText[3].text = "Health Pickup: +" + GameManager.Instance.healAmount;
        statsText[4].text = "Shield Duration: " + GameManager.Instance.shieldActiveTime + "s";
        statsText[5].text = "Ammo Pickup: +" + GameManager.Instance.ammoRecovery;

        specialStatsText[0].text = "Grenade Launcher\nLevel: " + SpecialUpgrades[0].weaponLevel;
        specialStatsText[1].text = "Shotgun\nLevel: " + SpecialUpgrades[1].weaponLevel;
        specialStatsText[2].text = "Ring Weapon\nLevel: " + SpecialUpgrades[2].weaponLevel;

        for (int i = 0; i < 3; i++) {
            weaponButton[i].GetComponent<Button>().enabled = !weaponLocked[i];
            weaponCover[i].SetActive(weaponLocked[i]);
        }
    }

    public void Restart() {
        player.Hidden = true;

        mainShop.SetActive(true);
        specialShop.SetActive(false);
        playerCanvas.SetActive(false);

        UpdateShopPoints();
    }

    public void UpdateShopPoints() { //called from GameManager
        shopPoints = GameManager.Instance.points;
        shopPointsText1.text = "Points: " + shopPoints;
        shopPointsText2.text = "Points: " + shopPoints;
    }

    void UpdateGamePoints() {
        shopPointsText1.text = "Points: " + shopPoints;
        shopPointsText2.text = "Points: " + shopPoints;
        GameManager.Instance.points = shopPoints;
    }

    public void PurchaseUpgrade(int type) {
        if (shopPoints >= Upgrades[type].cost && Upgrades[type].currentUpgrade < Upgrades[type].maxLevel) {
            Upgrades[type].currentUpgrade++;
            levelText[type].text = "Level: " + Upgrades[type].currentUpgrade;

            switch (type) {
            case 0: //player max health limit
                Healthbar playerHealth = player.Health;
                playerHealth.maxPointsLimit += Upgrades[type].increase;
                playerHealth.Reset();
                break;
            case 1: //player speed
                player.walkSpeed += Upgrades[type].increase;
                break;
            case 2: //player damage
                player.bulletDamage += Upgrades[type].increase;
                break;
            case 3: //heal pack pickup
                GameManager.Instance.healAmount += Upgrades[type].increase;
                break;
            case 4: // shield pickup
                GameManager.Instance.shieldActiveTime += Upgrades[type].increase;
                break;
            case 5: //ammo pickup
                GameManager.Instance.ammoRecovery += Upgrades[type].increase;
                break;
            }

            if (Upgrades[type].currentUpgrade == Upgrades[type].maxLevel) {
                levelText[type].text = "Level: MAX";
            }

            shopPoints -= Upgrades[type].cost;
        }

        UpdateGamePoints();
    }
    
    public void DisplaySpecialUpgrades(bool display) {
        mainShop.SetActive(!display);
        specialShop.SetActive(display);
    }

    public void UnlockSpecial(int num) {
        weaponLocked[num-1] = false;
    }

    public void UpgradeSpecial(int type) {
        if (shopPoints >= SpecialUpgrades[type].cost && SpecialUpgrades[type].currentUpgrade < SpecialUpgrades[type].maxLevel) {
            SpecialUpgrades[type].currentUpgrade++;
            SpecialUpgrades[type].weaponLevel++;
            specialLevelText[type].text = "Level: " + SpecialUpgrades[type].currentUpgrade;

            player.specialWeapons[type].GetComponent<SpecialWeapon>().maxAmmo += SpecialUpgrades[type].ammoIncrease;

            switch (type) {
            case 0: //grenade launcher
                Grenade grenade = player.specialWeapons[0].GetComponent<Grenade>();

                grenade.damage += SpecialUpgrades[type].increase;
                grenade.radius += grenadeRadiusIncrease;
                break;
            case 1: //shotgun
                Shotgun shotgun = player.specialWeapons[1].GetComponent<Shotgun>();

                shotgun.bullet.GetComponent<BulletController>().damage += SpecialUpgrades[type].increase;
                shotgun.numBullets += shotgunBulletsIncrease;
                shotgun.fireArc += shotgunFireArcIncrease;
                break;
            case 2: //ring cannon
                Ring ring = player.specialWeapons[2].GetComponent<RingWeapon>().ring.GetComponent<Ring>();
                ring.damage += SpecialUpgrades[type].increase;
                ring.growthRate += ringGrowthRateIncrease;
                break;
            }

            if (SpecialUpgrades[type].currentUpgrade == SpecialUpgrades[type].maxLevel) {
                specialLevelText[type].text = "Level: MAX";
            }

            shopPoints -= SpecialUpgrades[type].cost;
        }

        UpdateGamePoints();
    }

    public void StartGame() {
        mainShop.SetActive(false);
        specialShop.SetActive(false);
        playerCanvas.SetActive(true);

        player.Hidden = false;

        GameManager.Instance.StartGame();
    }
}