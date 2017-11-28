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

    public Text healthUpgradeLevel;
    public Text healthCostText;
    public Text healthStatsText;
    public int currentHealthUpgrade = 1;
    public int maxHealthLevel = 5;
    public int healthIncrease = 50;
    public int healthCost = 35;

    public Text speedUpgradeLevel;
    public Text speedCostText;
    public Text speedStatsText;
    public int currentSpeedUpgrade = 1;
    public int maxSpeedLevel = 6;
    public int speedIncrease = 2;
    public int speedCost = 25;

    public Text damageUpgradeLevel;
    public Text damageCostText;
    public Text damageStatsText;
    public int currentDamageUpgrade = 1;
    public int maxDamageLevel = 4;
    public int damageIncrease = 1;
    public int damageCost = 40;

    public Text healPackUpgradeLevel;
    public Text healPackCostText;
    public Text healPackStatsText;
    public int currentHealPackUpgrade = 1;
    public int maxHealPackLevel = 4;
    public int healPackIncrease = 30;
    public int healPackCost = 40;

    public Text shieldUpgradeLevel;
    public Text shieldCostText;
    public Text shieldStatsText;
    public int currentShieldUpgrade = 1;
    public int maxShieldLevel = 6;
    public int shieldIncrease = 1;
    public int shieldCost = 30;

    public Text ammoUpgradeLevel;
    public Text ammoCostText;
    public Text ammoStatsText;
    public int currentAmmoUpgrade = 1;
    public int maxAmmoLevel = 5;
    public int ammoIncrease = 5;
    public int ammoCost = 30;

    public bool weapon1Locked;
    public bool weapon2Locked;
    public bool weapon3Locked;

    public GameObject weapon1Button;
    public GameObject weapon1Cover;
    public GameObject weapon2Button;
    public GameObject weapon2Cover;
    public GameObject weapon3Button;
    public GameObject weapon3Cover;

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

        weapon1Locked = true;
        weapon2Locked = true;
        weapon3Locked = true;

        healthCostText.text = "Cost: " + healthCost;
        speedCostText.text = "Cost: " + speedCost;
        damageCostText.text = "Cost: " + damageCost;
        healPackCostText.text = "Cost: " + healPackCost;
        shieldCostText.text = "Cost: " + shieldCost;
        ammoCostText.text = "Cost: " + ammoCost;
        //specialCostText.text = "Cost: " + specialCost;
    }

    void Update() {
        healthStatsText.text = "Max Health Limit: " + GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControls>().Health.maxPointsLimit;
        speedStatsText.text = "Player Speed: " + GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControls>().walkSpeed;
        damageStatsText.text = "Player Damage: " + GameManager.Instance.playerBulletDamage;
        healPackStatsText.text = "Health Pickup: +" + GameManager.Instance.healAmount;
        shieldStatsText.text = "Shield Duration: " + GameManager.Instance.shieldActiveTime + "s";
        ammoStatsText.text = "Ammo Pickup: +" + GameManager.Instance.ammoRecovery;
        //specialStatsText.text = "Special Weapon\nLevel: " + currentSpecialUpgrade;

        if (weapon1Locked) {
            weapon1Button.GetComponent<Button>().enabled = false;
            weapon1Cover.SetActive(true);
        } else {
            weapon1Button.GetComponent<Button>().enabled = true;
            weapon1Cover.SetActive(false);
        }

        if (weapon2Locked) {
            weapon2Button.GetComponent<Button>().enabled = false;
            weapon2Cover.SetActive(true);
        } else {
            weapon2Button.GetComponent<Button>().enabled = true;
            weapon2Cover.SetActive(false);
        }

        if (weapon3Locked) {
            weapon3Button.GetComponent<Button>().enabled = false;
            weapon3Cover.SetActive(true);
        } else {
            weapon3Button.GetComponent<Button>().enabled = true;
            weapon3Cover.SetActive(false);
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

    //methods called when shop buttons are pressed
    public void PurchaseUpgrade(int upgrade) {
        switch (upgrade) {
        case 0:
            IncreaseHealth();
            break;
        case 1:
            IncreaseSpeed();
            break;
        case 2:
            IncreaseDamage();
            break;
        case 3:
            IncreaseHealPack();
            break;
        case 4:
            IncreaseShield();
            break;
        case 5:
            IncreaseAmmoPack();
            break;
        case 6:
            //UpgradeSpecial();
            break;
        }

        UpdateGamePoints();
    }
    
    void IncreaseHealth() {
        if (shopPoints >= healthCost && currentHealthUpgrade < maxHealthLevel) {
            currentHealthUpgrade++;
            healthUpgradeLevel.text = "Level: " + currentHealthUpgrade;
            Healthbar playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControls>().Health;
            playerHealth.maxPointsLimit += healthIncrease;
            playerHealth.Reset();
            shopPoints -= healthCost;

            if (currentHealthUpgrade == maxHealthLevel) {
                healthUpgradeLevel.text = "Level: MAX";
            }
        }
    }

    void IncreaseSpeed() {
        if (shopPoints >= speedCost && currentSpeedUpgrade < maxSpeedLevel) {
            currentSpeedUpgrade++;
            speedUpgradeLevel.text = "Level: " + currentSpeedUpgrade;
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControls>().walkSpeed += speedIncrease;
            shopPoints -= speedCost;

            if (currentSpeedUpgrade == maxSpeedLevel) {
                speedUpgradeLevel.text = "Level: MAX";
            }
        }
    }

    void IncreaseDamage() {
        if (shopPoints >= damageCost && currentDamageUpgrade < maxDamageLevel) {
            currentDamageUpgrade++;
            damageUpgradeLevel.text = "Level: " + currentDamageUpgrade;
            GameManager.Instance.playerBulletDamage += damageIncrease;
            shopPoints -= damageCost;

            if (currentDamageUpgrade == maxDamageLevel) {
                damageUpgradeLevel.text = "Level: MAX";
            }
        }
    }

    void IncreaseHealPack() {
        if (shopPoints >= healPackCost && currentHealPackUpgrade < maxHealPackLevel) {
            currentHealPackUpgrade++;
            healPackUpgradeLevel.text = "Level: " + currentHealPackUpgrade;
            GameManager.Instance.healAmount += healPackIncrease;
            shopPoints -= healPackCost;

            if (currentHealPackUpgrade == maxHealPackLevel) {
                healPackUpgradeLevel.text = "Level: MAX";
            }
        }
    }

    void IncreaseShield() {
        if (shopPoints >= shieldCost && currentShieldUpgrade < maxShieldLevel) {
            currentShieldUpgrade++;
            shieldUpgradeLevel.text = "Level: " + currentShieldUpgrade;
            GameManager.Instance.shieldActiveTime += shieldIncrease;
            shopPoints -= shieldCost;

            if (currentShieldUpgrade == maxShieldLevel) {
                shieldUpgradeLevel.text = "Level: MAX";
            }
        }
    }

    void IncreaseAmmoPack() {
        if (shopPoints >= ammoCost && currentAmmoUpgrade < maxAmmoLevel) {
            currentAmmoUpgrade++;
            ammoUpgradeLevel.text = "Level: " + currentAmmoUpgrade;
            GameManager.Instance.ammoRecovery += ammoIncrease;
            shopPoints -= ammoCost;

            if (currentAmmoUpgrade == maxAmmoLevel) {
                ammoUpgradeLevel.text = "Level: MAX";
            }
        }
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
        switch (num) {
        case 1:
            weapon1Locked = false;
            break;
        case 2:
            weapon2Locked = false;
            break;
        case 3:
            weapon3Locked = false;
            break;
        }
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