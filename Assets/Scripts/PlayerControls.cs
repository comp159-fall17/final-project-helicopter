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

    [HideInInspector] public int[] specialAmmo;

    [HideInInspector] public int specialType; //0 = grenade, 1 = shotgun, 2 = ring, 3 = none
    int maxSpecialAmmo;

    public GameObject[] specialWeapons;
    public GameObject[] bulletModifiers; //0 = normal bullet, 1 = double, 2 = triple, 3 = rapid

    protected override bool ShouldShoot {
        get {
            return !hidden && Input.GetMouseButton(0) && WallInWay;
        }
    }

    protected override bool ShouldShootSpecial {
        get {
            return !hidden && Input.GetMouseButtonDown(1) && WallInWay && specialAmmo[specialType] != 0;
        }
    }

    protected override Vector3 Target {
        get {
            // Raycast to corresponding point on screen.
            Ray viewRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            // get distance to player plane
            Plane flat = new Plane(Vector3.up, Vector3.zero);
            float rayDistance;
            flat.Raycast(viewRay, out rayDistance);

            return CopyY(viewRay.GetPoint(rayDistance), Body.position);
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

        upgradeSound = GetComponent<AudioSource>();

        shieldIndicator.enabled = false;
        specialType = 3;

        specialAmmo = new int[4];
        for (int i = 0; i < 3; i++) {
            specialAmmo[i] = specialWeapons[i].GetComponent<SpecialWeapon>().maxAmmo;
        }

        specialAmmo[3] = 0;
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
        } else if (Input.GetKeyDown(KeyCode.Alpha3)) {
            CollectSpecial(3);
        }

        //testing shot modifier collection
        if (Input.GetKeyDown(KeyCode.Alpha4)) {
            CollectModifier(0);
        } else if (Input.GetKeyDown(KeyCode.Alpha5)) {
            CollectModifier(1);
        } else if (Input.GetKeyDown(KeyCode.Alpha6)) {
            CollectModifier(2);
        } else if (Input.GetKeyDown(KeyCode.Alpha7)) {
            CollectModifier(3);
        }
    }

    void playUpgradeSound() {
        AudioSource.PlayClipAtPoint(upgradeSound.clip, Camera.main.transform.position);
    }

    void UpdateInputAxes() {
        inputAxes.x = Input.GetAxis("Horizontal");
        //inputAxes.y;
        inputAxes.z = Input.GetAxis("Vertical");

        inputAxes *= Speed;

        // Dont delete this.
        //transform.rotation = Quaternion.Euler(0.0f, AbsoluteTargetAngle, 0.0f);
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
                    if (specialType == 3) { //no special
                        return;
                    }

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

                specialAmmo[specialType]--;
                GameManager.Instance.UpdateAmmoText();
            } else {
                break;
            }

            yield return new WaitForSeconds(fireDelay);
        }
        shooting = false;
    }

    public void CollectPickup(int type) { //used in InGameShop
        switch (type) {
            case 0:
                CollectHealth();
                break;
            case 1:
                CollectShield();
                break;
            case 2:
                CollectAmmo();
                break;
        }
    }

    void CollectHealth() {
        Health.Heal(GameManager.Instance.healAmount);
        GameManager.Instance.UpdateHealthText();
        playUpgradeSound();
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
        if (specialAmmo[specialType] + GameManager.Instance.ammoRecovery <= maxSpecialAmmo) {
            specialAmmo[specialType] += GameManager.Instance.ammoRecovery;
        } else {
            specialAmmo[specialType] = maxSpecialAmmo;
        }

        GameManager.Instance.UpdateAmmoText();
        playUpgradeSound();
    }

    void CollectMoney() {
        GameManager.Instance.CollectMoney(GameManager.Instance.moneyGain);
    }

    public void CollectSpecial(int type) { //type is 1 to 3, corresponding to the special weapon
        if (type == 0) {
            specialWeapon = null;
            specialType = 3;
            maxSpecialAmmo = 0;
            return;
        }

        specialWeapon = specialWeapons[type - 1];
        specialType = type - 1;
        maxSpecialAmmo = specialWeapon.GetComponent<SpecialWeapon>().maxAmmo;

        GameManager.Instance.UpdateAmmoText();

        ShopManager.Instance.UnlockSpecial(type);
    }

    public void CollectModifier(int type) {
        bullet = bulletModifiers[type];

        if (type == 3) {
            fireDelay = 0.025f;
        } else {
            fireDelay = 0.1f;
        }
    }

    public void ResetAmmo(int type) { //reset the ammo of the given weapon
        specialAmmo[type] = specialWeapons[type].GetComponent<SpecialWeapon>().maxAmmo;
        GameManager.Instance.UpdateAmmoText();
    }

    public void SetDamage() { //called from GameManager after the main shop is closed
        bulletModifiers[0].GetComponent<BulletController>().damage = bulletDamage; //normal bullet

        foreach (BulletController bc in bulletModifiers[1].GetComponentsInChildren<BulletController>()) {
            bc.damage = bulletDamage; //double shot
        }

        foreach (BulletController bc in bulletModifiers[2].GetComponentsInChildren<BulletController>()) {
            bc.damage = bulletDamage; //triple shot
        }

        bulletModifiers[3].GetComponent<BulletController>().damage = bulletDamage / 2.0f; //rapid fire
    }

    public override void Hit(float damage) {
        // avoid all damage while shielded, during invincibility time, or if hidden
        if (shielded || damaged || Hidden) return;

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

            if (value) {
                gameObject.layer = LayerMask.NameToLayer("Default");
            } else {
                gameObject.layer = LayerMask.NameToLayer("Player");
            }
        }
    }

    protected override void Die() {
        if (Hidden) {
            return;
        }

        GameManager.Instance.PlayDeathSound();
        StartCoroutine(GameManager.Instance.GameOver(this));

        Hidden = true;
        Health.Reset();
        GameManager.Instance.UpdateHealthText();
    }

    public void Reset() {
        Hidden = false;
        Health.Reset();
        transform.position = spawn;
        hasShield = false;
        GameManager.Instance.UpdateHealthText();
        CollectModifier(0);
        CollectSpecial(0);

        for (int i = 0; i < 3; i++) {
            specialAmmo[i] = specialWeapons[i].GetComponent<SpecialWeapon>().maxAmmo;
        }
    }
}
