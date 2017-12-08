using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : Shooter {
    public GameObject UnlockKey;

    protected override bool ShouldShoot { get { return false; } } 
    protected override bool ShouldShootSpecial { get { return false; } }
    protected override bool WallInWay { get { return false; } }
    protected override Vector3 Target { get { return transform.forward; } }

    protected override void Start() {
        UnlockKey.SetActive(false);
    }

    protected override void Die() {
        UnlockKey.SetActive(true);
        GameManager.Instance.PlayDeathSound();
        Destroy(GameObject.FindGameObjectWithTag("Boss"));
    }
}
