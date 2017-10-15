using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// Controller for players. Should only have as many instantiated as there are players.
/// </summary>
public class PlayerControls : Shooter {
    protected override bool ShouldShoot {
        get {
            return !hidden && Input.GetMouseButton(0) && WallInWay;
        }
    }

    protected override bool ShouldShootSpecial {
        get {
            return !hidden && Input.GetMouseButton(1) && WallInWay && specialAmmo != 0;
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

    Vector3 inputAxes;
    Vector3 spawn;
    Camera follow;

    protected override void Start() {
        base.Start();

        inputAxes = new Vector3(0, 0, 0);
        follow = Camera.main;
        spawn = transform.position;

        ResetAmmo();
        PlayerFlash.SetActive(false);
    }

    protected override void Update() {
        base.Update();
       
        UpdateInputAxes();
        TrackCamera();
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

    public GameObject PlayerFlash;

    void OnTriggerEnter(Collider other) {
        switch (other.gameObject.tag) {
        case "Pickup":
            if (other.gameObject.name.Contains("Health")) {
                CollectHealth();
            } else if (other.gameObject.name.Contains("Shield")) {
                CollectShield();
            } else if (other.gameObject.name.Contains("Ammo")) {
                CollectAmmo();
            }

            Destroy(other.gameObject);
            break;
        //case "Bullet":
            //PlayerFlash.GetComponent<MeshRenderer>().enabled = true;
            //PlayerFlash.GetComponent<BoxCollider>().enabled = true;
            //break;
        }
    }

    void CollectHealth() {
        Health.Heal(GameManager.Instance.healAmount);
    }

    public GameObject shield;

    void CollectShield() {
        Destroy(Instantiate(shield, transform), GameManager.Instance.shieldActiveTime);
    }

    void CollectAmmo() {
        if (specialAmmo + GameManager.Instance.ammoRecovery <= maxSpecialAmmo) {
            specialAmmo += GameManager.Instance.ammoRecovery;
        } else {
            specialAmmo = maxSpecialAmmo;
        }

        GameManager.Instance.UpdateAmmoText();
    }

    public void ResetAmmo() {
        specialAmmo = maxSpecialAmmo;
    }

    public override void Hit(float speed) {
        base.Hit(speed);

        StartCoroutine(DisplayPlayerFlash());
    }

    IEnumerator DisplayPlayerFlash() {
        PlayerFlash.SetActive(true);
        yield return new WaitForSeconds(1f);
        PlayerFlash.SetActive(false);
    }

    bool hidden;
    bool Hidden {
        get { return hidden; }
        set {
            hidden = value;

            PlayerFlash.SetActive(!value);

            foreach (Transform child in transform) {
                child.gameObject.SetActive(!value);
            }
        }
    }

    protected override void Die() {
        StartCoroutine(GameManager.Instance.gameOver(this));

        Hidden = true;
    }

    public void Reset() {
        Hidden = false;

        Health.Reset();
        transform.position = spawn;
    }
}
