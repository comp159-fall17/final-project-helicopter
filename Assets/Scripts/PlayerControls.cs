using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// Controller for players. Should only have as many instantiated as there are players.
/// </summary>
public class PlayerControls : Shooter {

	private AudioSource upgradeSound;

    Vector3 inputAxes;
    Vector3 spawn;
    Camera follow;

    public GameObject shieldPrefab;
    public Image shieldIndicator;
    bool hasShield; //player has the shield pickup
    bool shielded;
    bool hidden;

    public int bulletDamage = 1;
    public float damageFlashTime;
    bool damaged; //used for invincibility frames
    public float invincibleTime;

    [HideInInspector] public int specialAmmo = 0;
    int maxSpecialAmmo;

    public GameObject[] specialWeapons;

    protected override bool ShouldShoot {
        get {
            return !hidden && Input.GetMouseButton(0) && WallInWay;
        }
    }

    protected override bool ShouldShootSpecial {
        get {
            return !hidden && Input.GetMouseButtonDown(1) && WallInWay && specialAmmo != 0;
        }
    }

    protected override Vector3 Target {
        get {
            // Raycast to corresponding point on screen.
            Ray viewRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(viewRay, out hit);

            return CopyY(hit.point, Body.position);
        }
    }

    protected override bool WallInWay {
        get {
            Vector3 rayDirection = BulletSpawnPoint - transform.position;
            int notPlayerMask = ~(1 << LayerMask.NameToLayer("Player"));
            return !Physics.Raycast(transform.position,
                                    rayDirection, 2, notPlayerMask);
        }
    }

    protected override void Start() {
        base.Start();

        inputAxes = new Vector3(0, 0, 0);
        follow = Camera.main;
        spawn = transform.position;

		upgradeSound = GetComponent<AudioSource> ();

        shieldIndicator.enabled = false;
    }

    protected override void Update() {
        base.Update();
        UpdateInputAxes();
        TrackCamera();

        //use shield with spacebar
        if (Input.GetKeyDown(KeyCode.Space) && hasShield && !shielded) {
            StartCoroutine(UseShield());
        }

        //testing special weapon collection
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            CollectSpecial(1);
        } else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            CollectSpecial(2);
        }else if (Input.GetKeyDown(KeyCode.Alpha3)) {
            CollectSpecial(3);
        }
    }

	void playUpgradeSound(){
		AudioSource.PlayClipAtPoint (upgradeSound.clip, Camera.main.transform.position);
	}

    void UpdateInputAxes() {
        inputAxes.x = Input.GetAxis("Horizontal");
        //inputAxes.y;
        inputAxes.z = Input.GetAxis("Vertical");

        inputAxes *= Speed;

		// Dont delete this.
		//transform.rotation = Quaternion.Euler(0.0f, AbsoluteTargetAngle, 0.0f);
    }

	public float AbsTargetAngle(){
		return AbsoluteTargetAngle;
	}

    void TrackCamera() {
        follow.transform.position = CopyY(transform.position,
                                          follow.transform.position);
    }

    void FixedUpdate() {
        if (!hidden) {
            Body.velocity = CopyY(inputAxes, Body.velocity);
        }
    }

    /// <summary>
    /// Creates a copy of v with w.y substituted.
    /// </summary>
    /// <returns>"Copied" vector.</returns>
    /// <param name="to">Destination.</param>
    /// <param name="from">Source.</param>
    Vector3 CopyY(Vector3 to, Vector3 from) {
        to.y = from.y;
        return to;
    }

    public GameObject PlayerFlash {
        get { return GameObject.FindWithTag("FX"); }
    }

    void OnTriggerEnter(Collider other) {
        switch (other.gameObject.tag) {
        case "Pickup":
            if (other.gameObject.name.Contains("Health")) {
                CollectHealth();
            } else if (other.gameObject.name.Contains("Shield")) {
                if (hasShield)
                    return;

                CollectShield();
            } else if (other.gameObject.name.Contains("Ammo")) {
                CollectAmmo();
            } else if (other.gameObject.name.Contains("Money")) {
                CollectMoney();
            }

            Destroy(other.gameObject);
            break;
        //case "Bullet":
            //PlayerFlash.GetComponent<MeshRenderer>().enabled = true;
            //PlayerFlash.GetComponent<BoxCollider>().enabled = true;
            //break;
        }
    }

    protected override IEnumerator ShootBullets() {
        shooting = true;
        while (true) {
            if (ShouldShoot) {
                Instantiate(bullet, BulletSpawnPoint,
                            Quaternion.Euler(0, AbsoluteTargetAngle, 90));
            } else if (ShouldShootSpecial) {
                Instantiate(specialWeapon, BulletSpawnPoint,
                            Quaternion.Euler(0, AbsoluteTargetAngle, 90));
                specialAmmo--;
                GameManager.Instance.UpdateAmmoText();
            } else {
                break;
            }

            yield return new WaitForSeconds(fireDelay);
        }
        shooting = false;    
    }

    void CollectHealth() {
        Health.Heal(GameManager.Instance.healAmount);
        GameManager.Instance.UpdateHealthText();
		playUpgradeSound ();
    }

    void CollectShield() {
        hasShield = true;
        shieldIndicator.enabled = true;
    }

    IEnumerator UseShield() {
        hasShield = false;
        shielded = true;
        shieldIndicator.enabled = false;
        playUpgradeSound();

        GameObject shield = Instantiate(shieldPrefab, transform) as GameObject;
        yield return new WaitForSeconds(GameManager.Instance.shieldActiveTime);
        Destroy(shield);

        shielded = false;
    }

    void CollectAmmo() {
        if (specialAmmo + GameManager.Instance.ammoRecovery <= maxSpecialAmmo) {
            specialAmmo += GameManager.Instance.ammoRecovery;
        } else {
            specialAmmo = maxSpecialAmmo;
        }

        GameManager.Instance.UpdateAmmoText();
		playUpgradeSound ();
    }

    void CollectMoney() {
        GameManager.Instance.CollectMoney(GameManager.Instance.moneyGain);
    }

    void CollectSpecial(int type) { //type is 1 to 3, corresponding to the special weapon
        if (type == 0) {
            specialWeapon = null;
            maxSpecialAmmo = 0;
            ResetAmmo();
            return;
        }

        specialWeapon = specialWeapons[type-1];
        maxSpecialAmmo = specialWeapon.GetComponent<SpecialWeapon>().maxAmmo;
        ResetAmmo();

        GameManager.Instance.UpdateAmmoText();

        ShopManager.Instance.UnlockSpecial(type);
    }

    public void ResetAmmo() {
        specialAmmo = maxSpecialAmmo;
    }

    public void SetDamage() {
        bullet.GetComponent<BulletController>().damage = bulletDamage;
    }

    public override void Hit(float damage) {
        // avoid all damage while shielded or during invincibility time
        if (shielded || damaged) return;

        base.Hit(damage);
        GameManager.Instance.UpdateHealthText();
        StartCoroutine(DisplayPlayerFlash());
    }

    IEnumerator DisplayPlayerFlash() {
        PlayerFlash.GetComponent<MeshRenderer>().enabled = true;
        damaged = true;
        yield return new WaitForSeconds(damageFlashTime);
        PlayerFlash.GetComponent<MeshRenderer>().enabled = false;

        yield return new WaitForSeconds(invincibleTime - damageFlashTime);
        damaged = false;
    }

    public bool Hidden {
        get { return hidden; }
        set {
            hidden = value;

            foreach (Transform child in transform) {
                child.gameObject.SetActive(!value);
            }
        }
    }

    protected override void Die() {
		GameManager.Instance.playDeathSound (true);
        StartCoroutine(GameManager.Instance.gameOver(this));

        Hidden = true;
        Health.Reset();
        GameManager.Instance.UpdateHealthText();
    }

    public void Reset() {
        Hidden = false;
        Health.Reset();
        transform.position = spawn;
        GameManager.Instance.UpdateHealthText();
        CollectSpecial(0);
    }
}
