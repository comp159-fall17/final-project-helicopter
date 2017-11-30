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

    public ShopUpgrade[] Upgrades;
    public Text[] levelText;
    public Text[] costText;
    public Text[] statsText;

    bool[] weaponLocked = new bool[3];
    public GameObject[] weaponButton;
    public GameObject[] weaponCover;

    /*
    public Text specialUpgradeLevel;
    public Text specialCostText;
    public Text specialStatsText;
    public int currentSpecialUpgrade = 1;
    public int maxSpecialLevel = 5;
    public float specialRadiusIncrease = 1.5f;
    public int specialDamageIncrease = 1;
    public int specialAmmoIncrease = 10;
    public int specialCost = 50;
    */

    public static ShopManager Instance;

    void Start () {
        if (Instance == null) {
            Instance = this;
        } else if (Instance != this) {
            Destroy(gameObject);
        }

        mainShop.SetActive(true);
        specialShop.SetActive(false);

        for (int i = 0; i < 3; i++) {
            weaponLocked[i] = true;
        }

        for (int i = 0; i < 6; i++) {
            costText[i].text = "Cost: " + Upgrades[i].cost;
            Upgrades[i].currentUpgrade = 1;
        }

        //specialCostText.text = "Cost: " + specialCost;
    }

    void Update() {
        statsText[0].text = "Max Health Limit: " + GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControls>().Health.maxPointsLimit;
        statsText[1].text = "Player Speed: " + GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControls>().walkSpeed;
        statsText[2].text = "Player Damage: " + GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControls>().bulletDamage;
        statsText[3].text = "Health Pickup: +" + GameManager.Instance.healAmount;
        statsText[4].text = "Shield Duration: " + GameManager.Instance.shieldActiveTime + "s";
        statsText[5].text = "Ammo Pickup: +" + GameManager.Instance.ammoRecovery;
        //specialStatsText.text = "Special Weapon\nLevel: " + currentSpecialUpgrade;

        for (int i = 0; i < 3; i++) {
            weaponButton[i].GetComponent<Button>().enabled = !weaponLocked[i];
            weaponCover[i].SetActive(weaponLocked[i]);
        }
    }

    public void UpdateShopPoints(int points) {
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
                Healthbar playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControls>().Health;
                playerHealth.maxPointsLimit += Upgrades[type].increase;
                playerHealth.Reset();
                break;
            case 1: //player speed
                GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControls>().walkSpeed += Upgrades[type].increase;
                break;
            case 2: //player damage
                GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControls>().bulletDamage += Upgrades[type].increase;
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

    /*void UpgradeSpecial() {
        if (shopPoints >= specialCost && currentSpecialUpgrade < maxSpecialLevel) {
            currentSpecialUpgrade++;
            specialUpgradeLevel.text = "Level: " + currentSpecialUpgrade;
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControls>().specialBullet.GetComponent<Cannonball>().damage += specialDamageIncrease;
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControls>().specialBullet.GetComponent<Cannonball>().radius += specialRadiusIncrease;
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControls>().maxSpecialAmmo += specialAmmoIncrease;
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControls>().ResetAmmo();
            shopPoints -= specialCost;

            if (currentSpecialUpgrade == maxSpecialLevel) {
                specialUpgradeLevel.text = "Level: MAX";
            }
        }
    }*/
    
    public void DisplaySpecialUpgrades(bool display) {
        mainShop.SetActive(!display);
        specialShop.SetActive(display);
    }

    public void UnlockSpecial(int num) {
        weaponLocked[num-1] = false;
    }

    public void UpgradeSpecial(int num) {
        switch (num) {
        case 1:
            UpgradeSpecial1();
            break;
        case 2:
            UpgradeSpecial2();
            break;
        case 3:
            UpgradeSpecial3();
            break;
        }

        UpdateGamePoints();
    }

    void UpgradeSpecial1() {
        Debug.Log("modify values for special weapon 1.");
    }

    void UpgradeSpecial2() {
        Debug.Log("modify values for special weapon 2.");
    }

    void UpgradeSpecial3() {
        Debug.Log("modify values for special weapon 3.");
    }
}