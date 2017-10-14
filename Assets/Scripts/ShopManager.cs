using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour {
    public Text shopPointsText;
    public Text waveText;
    int shopPoints;

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

    public Text specialUpgradeLevel;
    public Text specialCostText;
    public Text specialStatsText;
    public int currentSpecialUpgrade = 1;
    public int maxSpecialLevel = 5; //temp
    public int specialCost = 50;

    public static ShopManager Instance;

    void Start () {
        if (Instance == null) {
            Instance = this;
        } else if (Instance != this) {
            Destroy(gameObject);
        }

        healthCostText.text = "Cost: " + healthCost;
        speedCostText.text = "Cost: " + speedCost;
        damageCostText.text = "Cost: " + damageCost;
        healPackCostText.text = "Cost: " + healPackCost;
        shieldCostText.text = "Cost: " + shieldCost;
        ammoCostText.text = "Cost: " + ammoCost;
        specialCostText.text = "Cost: " + specialCost;
    }

    void Update() {
        waveText.text = "Highest Wave\nReached: " + GameManager.Instance.highestWave;

        healthStatsText.text = "Max Health: " + GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControls>().Health.maxPoints;
        speedStatsText.text = "Player Speed: " + GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControls>().walkSpeed;
        damageStatsText.text = "Player Damage: " + GameManager.Instance.playerBulletDamage;
        healPackStatsText.text = "Health Pickup: +" + GameManager.Instance.healAmount;
        shieldStatsText.text = "Shield Duration: " + GameManager.Instance.shieldActiveTime + "s";
        //ammoStatsText.text = "Ammo Pickup: +" + (ammo recovery)
        specialStatsText.text = "Special Weapon\nLevel: " + currentSpecialUpgrade;
    }

    public void UpdateShopPoints(int points) {
        shopPoints = GameManager.Instance.points;
        shopPointsText.text = "Points: " + shopPoints;
    }

    void UpdateGamePoints() {
        shopPointsText.text = "Points: " + shopPoints;
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
            UpgradeSpecial();
            break;
        default:
            break;
        }

        UpdateGamePoints();
    }
    
    void IncreaseHealth() {
        if (shopPoints >= healthCost && currentHealthUpgrade < maxHealthLevel) {
            currentHealthUpgrade++;
            healthUpgradeLevel.text = "Level: " + currentHealthUpgrade;
            Healthbar playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControls>().Health;
            playerHealth.maxPoints += healthIncrease;
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
            //increase ammo recovery from pickup
            shopPoints -= ammoCost;

            if (currentAmmoUpgrade == maxAmmoLevel) {
                ammoUpgradeLevel.text = "Level: MAX";
            }
        }
    }

    void UpgradeSpecial() {
        if (shopPoints >= specialCost && currentSpecialUpgrade < maxSpecialLevel) {
            currentSpecialUpgrade++;
            specialUpgradeLevel.text = "Level: " + currentSpecialUpgrade;
            //increase various values of the special, like damage and blast radius
            shopPoints -= specialCost;

            if (currentSpecialUpgrade == maxSpecialLevel) {
                specialUpgradeLevel.text = "Level: MAX";
            }
        }
    }
}
